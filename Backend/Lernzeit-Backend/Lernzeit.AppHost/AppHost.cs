using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddPostgres("postgres");

var googleClientId = builder.AddParameter("GoogleClientId", secret: true);
var googleClientSecret = builder.AddParameter("GoogleClientSecret", secret: true);

var frontend = builder.AddJavaScriptApp(name: "Frontend", appDirectory: "../../../frontend", "start")
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WithEnvironment("BROWSER", "none"); // Prevents CRA from opening browser on start

var backend = builder
    .AddProject<Projects.Lernzeit_Backend>("Backend")
    .WithReference(db)
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Authentication__Google__ClientSecret", googleClientSecret)
    .WithEnvironment("FrontendUrl", frontend.GetEndpoint("http"));

frontend.WithReference(backend)
    .WithEnvironment("REACT_APP_BACKEND_URL", backend.GetEndpoint("https"));

builder.Build().Run();