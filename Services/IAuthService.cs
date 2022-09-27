using System.Threading.Tasks;
using Customer.Portal.Web.ViewModels;

namespace Customer.Portal.Web.Services {
    public interface IAuthService {
        Task Register(UserCreateViewModel user);
        Task<JwtViewModel> Login(LoginViewModel loginViewModel);
    }
}