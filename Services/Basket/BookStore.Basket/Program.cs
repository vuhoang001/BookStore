using Aspire.ServiceDefaults;
using BookStore.Basket.Extensions;
using BookStore.Basket.Infrastructure;
using BuildingBlocks.Chassis.EndPoints;
using BuildingBlocks.Constants.Core;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddApplicationService();
builder.AddPersistenceServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
app.UseExceptionHandler();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(Versions.V1)
    .ReportApiVersions()
    .Build();

app.MapEndpoints(apiVersionSet);

app.UseHttpsRedirection();
app.MapDefaultEndpoints();


app.Run();
