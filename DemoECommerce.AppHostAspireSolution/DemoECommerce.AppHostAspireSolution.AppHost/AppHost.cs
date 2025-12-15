var builder = DistributedApplication.CreateBuilder(args);

// SQL Server resource
//var sql = builder.AddSqlServer("sqlserver")
//            .WithDataVolume()
//            .WithLifetime(ContainerLifetime.Persistent);

// PostgreSQL resource
var postgres = builder.AddPostgres("postgres")
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent);

// Database
var ecommerceDb = postgres.AddDatabase("eCommerceConnection");

builder.AddProject<Projects.DemoECommerce_DbMigrator_SQL>("demoecommerce-dbmigrator-sql")
    .WithReference(ecommerceDb)
    .WithReplicas(1);

builder.AddProject<Projects.ApiGateway_Presentation>("apigateway-presentation");

builder.AddProject<Projects.AuthenticationApi_Presentation>("authenticationapi-presentation")
    .WithReference(ecommerceDb);

builder.AddProject<Projects.OrderApi_Presentation>("orderapi-presentation")
    .WithReference(ecommerceDb);

builder.AddProject<Projects.ProductApi_Presentation>("productapi-presentation")
    .WithReference(ecommerceDb);

builder.Build().Run();
