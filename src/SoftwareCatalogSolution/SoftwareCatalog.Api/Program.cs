using FluentValidation;
using Marten;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using SoftwareCatalog.Api.Catalog;
using SoftwareCatalog.Api.Technicians;
using SoftwareCatalog.Api.Vendors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Above this line is configuring the "internals" of our API Project.

// This is saying use the "System" time provider, anywhere we need an instance of the TimeProvider

builder.Services.AddScoped<IValidator<CatalogItemRequestModel>, CatalogItemRequestModelValidator>();
builder.Services.AddScoped<IValidator<TechnicianBeforeDBCreation>, AddTechnicianRequestValidator>();

builder.Services.AddVendors();
builder.Services.AddSingleton<TimeProvider>((_) => TimeProvider.System);

var connectionString = builder.Configuration.GetConnectionString("database")
    ?? throw new Exception("Yo need a connection string");


builder.Services.AddMarten(config =>
{
    config.Connection(connectionString);
}).UseLightweightSessions();

builder.Services.AddFluentValidationRulesToSwagger();
var app = builder.Build(); // THE LINE IN THE SAND
// Everything after this line is configuring how the web server handles incoming requests/responses
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // ASPNETCORE_ENVIRONMENT=Development
{
    app.UseSwagger(); // the json OPEN API spec
    app.UseSwaggerUI(); // GET /swagger/index.html - an html web page that lets you visualize the spec for this api.
}

app.UseAuthentication();
app.UseAuthorization();
// Make Some Change
app.MapControllers(); // this will scan your entire project for any controllers, use the attributes (HttpGet, etc.) to create
// a "route table" - like a phone book. Reflection (the ability to have code look at itself)

app.MapVendors();

app.Run(); // a blocking infinite for loop.

// I will explain this later if you:
// a) don't get it
// b) care. (it is really an unfulfilling answer)

public partial class Program { }