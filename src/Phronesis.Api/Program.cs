using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phronesis.Infrastructure.Persistence;
using Phronesis.Api.Middleware;
using Markdig;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PhronesisDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Phronesis Platform API");
        options.WithTheme(Scalar.AspNetCore.ScalarTheme.DeepSpace);
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => Results.Content(
    """
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Phronesis Platform API</title>
        <style>
            :root { --primary: #2563eb; --bg: #0f172a; --text: #f8fafc; --card: #1e293b; }
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: var(--bg); color: var(--text); display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; }
            .container { text-align: center; background: var(--card); padding: 3rem; border-radius: 12px; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.5); max-width: 600px; }
            h1 { font-size: 2.5rem; margin-bottom: 0.5rem; font-weight: 700; background: linear-gradient(90deg, #3b82f6, #8b5cf6); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
            p { color: #94a3b8; line-height: 1.6; margin-bottom: 2rem; }
            .btn { display: inline-block; background-color: var(--primary); color: white; padding: 0.75rem 1.5rem; text-decoration: none; border-radius: 6px; font-weight: 600; transition: background-color 0.2s; margin: 0.5rem; }
            .btn:hover { background-color: #1d4ed8; }
            .btn-outline { background-color: transparent; border: 1px solid var(--primary); color: var(--primary); }
            .btn-outline:hover { background-color: rgba(37, 99, 235, 0.1); }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>Phronesis Platform</h1>
            <p>Welcome to the foundational API for the Phronesis Digital Education Platform. This scalable backend powers learning, commerce, and virtual tuition.</p>
            <div>
                <a href="/scalar/v1" class="btn">Explore API (Scalar)</a>
                <a href="/docs/api-specification" class="btn btn-outline">Technical Specification</a>
            </div>
        </div>
    </body>
    </html>
    """, "text/html"
)).WithName("LandingPage");

app.MapGet("/docs/api-specification", () =>
{
    var mdPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "docs", "api-endpoint-specification.md");
    if (!File.Exists(mdPath)) return Results.NotFound("Specification document not found.");
    
    var mdContent = File.ReadAllText(mdPath);
    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    var htmlContent = Markdown.ToHtml(mdContent, pipeline);
    
    var page = $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>API Specification</title>
        <style>
            body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; padding: 2rem; max-width: 900px; margin: 0 auto; color: #333; }
            h1, h2, h3, h4 { color: #111; margin-top: 1.5em; }
            table { border-collapse: collapse; width: 100%; margin-bottom: 1rem; }
            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            th { background-color: #f4f4f5; }
            a { color: #2563eb; text-decoration: none; }
            a:hover { text-decoration: underline; }
            .back-link { display: inline-block; margin-bottom: 2rem; color: #64748b; font-weight: 500; }
        </style>
    </head>
    <body>
        <a href="/" class="back-link">← Back to Home</a>
        {{htmlContent}}
    </body>

    </html>
    """;
    
    return Results.Content(page, "text/html");
});

app.Run();

public partial class Program { }
