using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Modern .NET 10 OpenAPI support with automatic XML documentation discovery
builder.Services.AddOpenApi();

builder.Services.AddCors(c => c.AddDefaultPolicy(pol =>
    pol
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin()
    ));

builder.Services.AddProblemDetails();

var app = builder.Build();

// Log every request to console
app.Use(async (context, next) =>
{
    Console.WriteLine($"{context.Request.Method} {context.Request.Path}");
    await next();
});

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.MapGet("/", context =>
{
    context.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

app.Run();
