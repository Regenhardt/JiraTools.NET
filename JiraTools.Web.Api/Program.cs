using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    swagger =>
    {
        swagger.EnableAnnotations();
        swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml"), true);
        foreach (var docFile in Assembly.GetEntryAssembly()!
                     .GetReferencedAssemblies()
                     .Select(a => Path.Combine(AppContext.BaseDirectory, $"{a.Name}.xml"))
                     .Where(File.Exists))
        {
            swagger.IncludeXmlComments(docFile);
        }
    });

builder.Services.AddCors(c => c.AddDefaultPolicy(pol =>
    pol
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
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
app.UseSwagger();
app.UseSwaggerUI();

app.UseDeveloperExceptionPage();

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
