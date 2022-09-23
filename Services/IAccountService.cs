using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Portal.Web.ViewModels;

namespace Customer.Portal.Web.Services {
    public interface IAccountService {
        Task<IEnumerable<AccountViewModel>> GetAllAsync();
        Task<AccountViewModel> GetByIdAsync(int id);
        Task<IEnumerable<AccountViewModel>> GetByCustomerAsync(int id);
        Task<AccountViewModel> CreateAsync(AccountCreateViewModel account);
        Task<AccountViewModel> UpdateAsync(int id, AccountUpdateViewModel account);
        Task DeleteAsync(int id);
    }
}