using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Middleware.Example;

public class ValidateMongoId {
    private readonly RequestDelegate _next;

    public ValidateMongoId(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        context.Request.RouteValues.TryGetValue("id", out var id);

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}