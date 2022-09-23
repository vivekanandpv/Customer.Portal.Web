using Microsoft.AspNetCore.Mvc;


namespace Customer.Portal.Web.Controllers {
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase {
        [HttpGet]
        public IActionResult Get() {
            return Ok("Hello, world!");
        }
    }
}