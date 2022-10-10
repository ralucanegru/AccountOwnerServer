using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountOwnerServer1.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public AccountController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAccounts([FromQuery] AccountParameters accountParameters)
        {
            var accounts = _repository.Account.GetAccounts(accountParameters);

            var metadata = new
            {
                accounts.TotalCount,
                accounts.PageSize,
                accounts.CurrentPage,
                accounts.HasNext,
                accounts.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            _logger.LogInfo($"Returned {accounts.TotalCount} accounts from database.");

            return Ok(accounts);
        }

        [HttpGet("{id}", Name = "AccountById")]
        public IActionResult GetAccountById(Guid id)
        {
            try
            {
                var account = _repository.Account.GetAccountById(id);

                if (account is null)
                {
                    _logger.LogError($"Account with id: {id}, hasn't been found in db.");

                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned account with id: {id}");

                    var accountResult = _mapper.Map<AccountDto>(account);

                    return Ok(accountResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOwnerById action: {ex.Message}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}
