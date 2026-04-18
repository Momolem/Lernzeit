using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddPostgres("postgres");

var backend = 
#if DEBUG
builder
    .AddProject<Projects.Lernzeit_Backend>("Backend")
    .WithReference(db);
#else
builder.AddDockerfile("Lernzeit-Backend", "../Lernzeit-Backend/Dockerfile");
#endif
builder.AddJavaScriptApp(name: "Frontend", appDirectory: "../../../frontend", "start")
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(backend)
    .WithEnvironment("BROWSER", "none"); // Prevents CRA from opening browser on start
builder.Build().Run();
