using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using United.Challenge.Api.Models;

namespace United.Challenge.Api.Filters
{
    public class ValidationProblemDetailsFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var resp = new ErrorResponse();
                foreach (var kvp in context.ModelState)
                {
                    var field = kvp.Key;
                    var state = kvp.Value;
                    if (state?.Errors != null)
                    {
                        foreach (var err in state.Errors)
                        {
                            var message = string.IsNullOrWhiteSpace(err.ErrorMessage) ? "Invalid value" : err.ErrorMessage;
                            resp.Errors.Add(new FieldError(field, message));
                        }
                    }
                }

                context.Result = new BadRequestObjectResult(resp);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
