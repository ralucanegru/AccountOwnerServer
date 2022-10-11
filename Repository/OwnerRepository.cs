using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Entities.Helpers;

namespace Repository
{
    public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
    {
        public OwnerRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public PagedList<Owner> GetOwners(OwnerParameters ownerParameters)
        {
            // filtering 
            // We are using FindByCondition method to find all the owners with the DateOfBirth between MaxYearOfBirth and MinYearOfBirth. 
            /*
            var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                               o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth)
                           .OrderBy(on => on.Name);

            return PagedList<Owner>.ToPagedList(FindAll().OrderBy(on => on.Name), ownerParameters.PageNumber, ownerParameters.PageSize);
            */

            // searching
            var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

            SearchByName(ref owners, ownerParameters.Name);

            return PagedList<Owner>.ToPagedList(owners.OrderBy(on => on.Name), ownerParameters.PageNumber, ownerParameters.PageSize);
        }

        private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
        {
            // we need to check it the name parameter is actually sent, by doing a simple IsNullOrWhiteSpace
            if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
                return;

            owners = owners.Where(o => o.Name.ToLower().Contains(ownerName.Trim().ToLower()));
        }

        public Owner GetOwnerById(Guid ownerId)
        {
            return FindByCondition(owner => owner.Id.Equals(ownerId)).FirstOrDefault();
        }

        public Owner GetOwnerWithDetails(Guid ownerId)
        {
            return FindByCondition(owner => owner.Id.Equals(ownerId)).Include(ac => ac.Accounts).FirstOrDefault();
        }

        public void CreateOwner(Owner owner)
        {
            Create(owner);
        }

        public void UpdateOwner(Owner owner)
        {
            Update(owner);
        }

        public void DeleteOwner(Owner owner)
        {
            Delete(owner);
        }
    }
}
