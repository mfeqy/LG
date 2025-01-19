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
        // Request authentication if no Authorization header is provided
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

        // Proceed to the next middleware if authentication succeeds
        await next.Invoke();
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
