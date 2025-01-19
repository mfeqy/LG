var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.Use(async (context, next) =>
{
    string authHeader = context.Request.Headers["Authorization"]!;

    if (string.IsNullOrWhiteSpace(authHeader))
    {
        context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Authentication is required.");
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

        if (username != "Check" || password != "CKECK@2025")
        {
            throw new UnauthorizedAccessException();
        }

        // Redirect only if the request is for the root ("/"), otherwise continue normal execution
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("Licenses/LicensePage");
            return; // Prevent further middleware execution
        }

        await next(context); // Explicitly pass context
    }
    catch
    {
        // Request authentication if validation fails
        context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Invalid username or password.");
    }
});

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
