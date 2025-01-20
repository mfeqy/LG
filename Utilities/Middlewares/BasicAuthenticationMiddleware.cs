namespace Youxel.Check.LicensesGenerator.Utilities.Middlewares
{
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public BasicAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"]!;

            if (string.IsNullOrWhiteSpace(authHeader))
            {
                await RespondWithAuthenticationChallenge(context);
                return;
            }

            try
            {
                // Parse and validate the Authorization header
                string encodedUsernamePassword = authHeader.Split(' ')[1];
                string decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                string[] usernamePassword = decodedUsernamePassword.Split(':');
                string username = usernamePassword[0];
                string password = usernamePassword[1];

                if (username != _configuration["Authentication:Username"] || password != _configuration["Authentication:Password"])
                {
                    throw new UnauthorizedAccessException();
                }

                // Redirect only if the request is for the root ("/"), otherwise continue normal execution
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("Licenses/Index");
                    return; // Prevent further middleware execution
                }

                await _next(context); // Pass the context to the next middleware
            }
            catch
            {
                // Respond with authentication challenge if validation fails
                await RespondWithAuthenticationChallenge(context);
            }
        }

        private async Task RespondWithAuthenticationChallenge(HttpContext context)
        {
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authentication is required.");
        }
    }

}
