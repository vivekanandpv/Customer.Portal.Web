using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Portal.Web.Services;
using Customer.Portal.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Customer.Portal.Web.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase {
        private readonly IAccountService _service;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService service, ILoggerFactory loggerFactory) {
            _service = service;
            _logger = loggerFactory.CreateLogger<AccountsController>();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountViewModel>>> GetAllAsync() {
            return Ok(await _service.GetAllAsync());
        }
        
        [HttpGet("by-customer/{id:int}")]
        public async Task<ActionResult<IEnumerable<AccountViewModel>>> GetByCustomerAsync(int id) {
            return Ok(await _service.GetByCustomerAsync(id));
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccountViewModel>> GetByIdAsync(int id) {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<AccountViewModel>> CreateAsync(AccountCreateViewModel viewModel) {
            return Ok(await _service.CreateAsync(viewModel));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AccountViewModel>> UpdateAsync(int id, AccountUpdateViewModel viewModel) {
            return Ok(await _service.UpdateAsync(id, viewModel));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}