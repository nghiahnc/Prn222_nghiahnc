using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Prn222_Nghiahnc_Mvc.Filters
{
    public class SecurityExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var statusCode = 0;

            if (context.Result is StatusCodeResult statusCodeResult)
            {
                statusCode = statusCodeResult.StatusCode;
            }
            else if (context.Result is ObjectResult objectResult && objectResult.StatusCode.HasValue)
            {
                statusCode = objectResult.StatusCode.Value;
            }

            if (statusCode == 403)
            {
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new ViewResult
                {
                    ViewName = "AccessDenied"
                };
            }
        }
    }
}
