using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Middleware;

public class ValidateMongoId {
    private readonly RequestDelegate _next;

    public ValidateMongoId(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}