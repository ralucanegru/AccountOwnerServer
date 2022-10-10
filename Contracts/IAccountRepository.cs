using Entities.Helpers;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAccountRepository : IRepositoryBase<Account>
    {
        IEnumerable<Account> AccountsByOwner(Guid ownerId);

        PagedList<Account> GetAccounts(AccountParameters accountParameters);
        Account GetAccountById(Guid accountId);

        void CreateAccount(Account account);
        void UpdateAccount(Account account);

        void DeleteAccount(Account account);
    }
}
