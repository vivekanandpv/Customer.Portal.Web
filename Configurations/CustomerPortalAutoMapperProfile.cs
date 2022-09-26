using AutoMapper;
using Customer.Portal.Web.Models;
using Customer.Portal.Web.ViewModels;

namespace Customer.Portal.Web.Configurations {
    public class CustomerPortalAutoMapperProfile : Profile {
        public CustomerPortalAutoMapperProfile() {
            CreateMap<BankCustomer, BankCustomerViewModel>();
            CreateMap<BankCustomerCreateViewModel, BankCustomer>();
            CreateMap<BankCustomerUpdateViewModel, BankCustomer>();

            CreateMap<Account, AccountViewModel>();
            CreateMap<AccountCreateViewModel, Account>().ForMember(a => a.CustomerId, options => {
                options.Ignore();
            });
            CreateMap<AccountUpdateViewModel, Account>();
        }
    }
}