namespace AdFormCurrencyConversion
{
    public class LoggingMiddlewareCustom
    {
        private readonly RequestDelegate _next;
        public LoggingMiddlewareCustom(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log Request
            Console.WriteLine($"➡️ Request: {context.Request.Method} {context.Request.Path}");

            // Call next middleware in pipeline
            await context.Response.WriteAsJsonAsync(new
            {
                message = "from middleare returning"
            });
            await _next(context);

            // Log Response
            Console.WriteLine($"⬅️ Response: {context.Response.StatusCode}");
        }
    }
}
