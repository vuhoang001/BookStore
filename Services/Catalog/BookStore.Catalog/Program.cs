using Aspire.ServiceDefaults;
using BookStore.Catalog.Extensions;
using BookStore.Catalog.Grpc.Services.Book;
using BuildingBlocks.Chassis.Security.Extensions;
using BuildingBlocks.Chassis.Security.Keycloaks;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationService();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddDefaultAuthentication();
builder.Services.AddLogging();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });
}



app.UseExceptionHandler();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(Versions.V1)
    .ReportApiVersions()
    .Build();

app.UseMiddleware<KeycloakTokenIntrospectionMiddleware>();


app.MapEndpoints(apiVersionSet);

app.MapDefaultEndpoints();

app.MapGrpcService<BookService>();
app.MapGrpcHealthChecksService();


app.Run();
