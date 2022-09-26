using System;
using Customer.Portal.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Customer.Portal.Web.Filters {
    public class GeneralExceptionHandler : IActionFilter, IOrderedFilter {
        public int Order => Int32.MaxValue - 10;
        
        public void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception is RecordNotFoundException 
                || context.Exception is DomainInvariantException) {
                context.Result = new ObjectResult(new {
                    context.Exception.Message
                }) {
                    StatusCode = 400
                };

                context.ExceptionHandled = true;
            }

            if (!context.ExceptionHandled && context.Exception != null) {
                context.Result = new OkObjectResult(new {
                    context.Exception.Message
                }) {
                    StatusCode = 503
                };
                
                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            
        }
    }
}