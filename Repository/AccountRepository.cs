using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Helpers;

namespace Repository
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Account> AccountsByOwner(Guid ownerId)
        {
            return FindByCondition(a => a.OwnerId.Equals(ownerId)).ToList();
        }

        public void CreateAccount(Account account)
        {
            Create(account);
        }

        public void DeleteAccount(Account account)
        {
            Delete(account);
        }

        public Account GetAccountById(Guid accountId)
        {
            return FindByCondition(account => account.Id.Equals(accountId)).FirstOrDefault();
        }

        public PagedList<Account> GetAccounts(AccountParameters accountParameters)
        {
            return PagedList<Account>.ToPagedList(FindAll().OrderBy(on => on.AccountType), accountParameters.PageNumber, accountParameters.PageSize);
        }

        public void UpdateAccount(Account account)
        {
            Update(account);
        }
    }
}
