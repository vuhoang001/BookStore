var builder = DistributedApplication.CreateBuilder(args);

var catalogApi = builder.AddProject<Projects.BookStore_Catalog>(Services.Catalog);

builder.Build().Run();
