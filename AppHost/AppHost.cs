using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//var sql = builder.AddSqlServer("sql").AddDatabase("ERP");

//var rabbit = builder.AddRabbitMQ("rabbit");

//builder.AddProject<Projects.ERP_Identity>("identity").WithReference(sql);

//builder.AddProject<Projects.ERP_OrderService>("orders").WithReference(sql).WithReference(rabbit);

//builder.AddProject<Projects.ReportGateway>("reports").WithReference(sql);

builder.AddProject<ReportGateway>("gateway");

var api = builder.AddProject<BaseSite_Api>("basesite-api");

builder.AddProject<BaseSite_Web>("basesite-web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
