using Aspire.ServiceDefaults;
using BookStore.Catalog.Extensions;
using BookStore.Catalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationService();
builder.AddPersistenceServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseExceptionHandler();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(Versions.V1)
    .ReportApiVersions()
    .Build();

app.MapEndpoints(apiVersionSet);

app.UseHttpsRedirection();
app.MapDefaultEndpoints();


app.Run();
