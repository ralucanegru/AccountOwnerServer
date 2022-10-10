using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountOwnerServer1.Controllers
{
    //Web API routing routes the incoming HTTP requests to the particular action method inside the Web API controller
    [Route("api/owner")]
    [ApiController]

    //Controllers will be responsible for handling requests, model validation, and returning responses to the frontend part of the application.
    public class OwnerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public OwnerController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        // [FromQuery] -> point out that we’ll be using query parameters to define which page and how many owners we are requesting
        public IActionResult GetOwners([FromQuery] OwnerParameters ownerParameters)
        {
            var owners = _repository.Owner.GetOwners(ownerParameters);

            var metadata = new
            {
                owners.TotalCount,
                owners.PageSize,
                owners.CurrentPage,
                owners.HasNext,
                owners.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInfo($"Returned {owners.TotalCount} owners from database.");

            return Ok(owners);
        }

        [HttpGet("{id}", Name = "OwnerById")]
        public IActionResult GetOwnerById(Guid id)
        {
            try
            {
                var owner = _repository.Owner.GetOwnerById(id);

                if (owner is null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");

                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    var ownerResult = _mapper.Map<OwnerDto>(owner);

                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOwnerById action: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/account")]
        public IActionResult GetOwnerWithDetails(Guid id)
        {
            try
            {
                var owner = _repository.Owner.GetOwnerWithDetails(id);
                if (owner == null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with details for id: {id}");

                    var ownerResult = _mapper.Map<OwnerDto>(owner);
                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOwnerWithDetails action: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateOwner([FromBody] OwnerForCreationDto owner)
        {
            try
            {
                if (owner is null)
                {
                    _logger.LogError("Owner object sent from client is null.");
                    return BadRequest("Owner object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid owner object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var ownerEntity = _mapper.Map<Owner>(owner);

                _repository.Owner.CreateOwner(ownerEntity);

                _repository.Save();

                var createdOwner = _mapper.Map<OwnerDto>(ownerEntity);

                return CreatedAtRoute("OwnerById", new { id = createdOwner.Id }, createdOwner);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public IActionResult UpdateOwner(Guid id, [FromBody] OwnerForUpdateDto owner)
        {
            try
            {
                if (owner is null)
                {
                    _logger.LogError("Owner object sent from client is null.");

                    return BadRequest("Owner object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid owner object sent from client.");

                    return BadRequest("Invalid model object");
                }
                var ownerEntity = _repository.Owner.GetOwnerById(id);

                if (ownerEntity is null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");

                    return NotFound();
                }
                _mapper.Map(owner, ownerEntity);

                _repository.Owner.UpdateOwner(ownerEntity);

                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteOwner(Guid id)
        {
            try
            {
                var owner = _repository.Owner.GetOwnerById(id);

                if (owner == null)
                {
                    _logger.LogError($"Owner with id: {id}, hasn't been found in db.");

                    return NotFound();
                }
                _repository.Owner.DeleteOwner(owner);

                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }

    }
}
