var builder = WebApplication.CreateBuilder(args);

// Configuration
var config = builder.Configuration;
var storagePath = config["Storage:Path"] ?? Path.Combine(builder.Environment.ContentRootPath, "uploads");
var apiKey = config["Security:ApiKey"] ?? "dev-key-change-in-production";

// Ensure storage directory exists
Directory.CreateDirectory(storagePath);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS - allow frontend origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://localhost:5173",
            "https://food.chipmeo.io.vn",
            "https://food.chipmeo.com",
            "https://api.chipmeo.io.vn",
            "https://api.chipmeo.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Store config in DI
builder.Services.AddSingleton(new StorageConfig { Path = storagePath, ApiKey = apiKey });

var app = builder.Build();

// Middleware pipeline
app.UseCors("AllowAll");

// API Key authentication for upload/delete endpoints
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Only require API key for POST and DELETE requests to /api/media
    if (path.StartsWith("/api/media") &&
        (context.Request.Method == "POST" || context.Request.Method == "DELETE"))
    {
        var requestApiKey = context.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (requestApiKey != apiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing API key" });
            return;
        }
    }

    await next();
});

// Serve static files from storage path (for direct file access)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(storagePath),
    RequestPath = ""
});

app.UseAuthorization();
app.MapControllers();

app.Run();

// Config class
public class StorageConfig
{
    public string Path { get; set; } = "";
    public string ApiKey { get; set; } = "";
}
