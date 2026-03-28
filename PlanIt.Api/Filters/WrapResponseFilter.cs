using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlanIt.Api.Filters;

public class WrapResponseFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult { Value: not null } objectResult)
        {
            objectResult.Value = new { data = objectResult.Value };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context) { }
}
