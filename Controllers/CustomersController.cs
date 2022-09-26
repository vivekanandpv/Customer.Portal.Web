using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Portal.Web.Services;
using Customer.Portal.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Customer.Portal.Web.Controllers {
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase {
        private readonly IBankCustomerService _service;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(IBankCustomerService service, ILoggerFactory loggerFactory) {
            _service = service;
            _logger = loggerFactory.CreateLogger<CustomersController>();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankCustomerViewModel>>> GetAllAsync() {
            return Ok(await _service.GetAllAsync());
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BankCustomerViewModel>> GetByIdAsync(int id) {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<BankCustomerViewModel>> CreateAsync(BankCustomerCreateViewModel viewModel) {
            return Ok(await _service.CreateAsync(viewModel));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BankCustomerViewModel>> UpdateAsync(int id, BankCustomerUpdateViewModel viewModel) {
            return Ok(await _service.UpdateAsync(id, viewModel));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}