
using Aspire.Hosting.Docker.Resources.ServiceNodes;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard();

var db = builder
    .AddPostgres("postgres")
    .PublishAsDockerComposeService((r, s) => { s.Name = "lernzeit_postgres"; });

var googleClientId = builder.AddParameter("GoogleClientId", secret: true);
var googleClientSecret = builder.AddParameter("GoogleClientSecret", secret: true);

// The backend URL must be baked into the React app at build time (CRA limitation).
// Set this to the publicly reachable URL of the backend, e.g. http://localhost:8080
var backendUrl = builder.AddParameter("BackendUrl");

var backend = builder
    .AddProject<Projects.Lernzeit_Backend>("Backend")
    .WithReference(db)
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Authentication__Google__ClientSecret", googleClientSecret)
    .WithEnvironment("FrontendUrl", "http://localhost:3000")
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "lernzeit_backend";
        service.Ports.Add("8080:8080");
        service.Environment["HTTP_PORTS"] = "8080";
        service.Expose.Clear();
        service.Expose.Add("8080");
        service.Build = new Build
        {
            Context = "../../..",
            Dockerfile = "../../../Backend/Lernzeit-Backend/Lernzeit-Backend/Dockerfile",
        };
        service.Image = null;
    });

var frontend = builder.AddDockerfile("frontend", "../../../frontend")
    .WithHttpEndpoint(port: 3000, targetPort: 80, name: "http")
    .WithBuildArg("REACT_APP_BACKEND_URL", backendUrl)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "lernzeit_frontend";
        service.Ports.Add("3000:80");
        service.Build = new Build
        {
            Context = "../../../frontend",
            Args = new Dictionary<string, string>
            {
                ["REACT_APP_BACKEND_URL"] = "${BACKENDURL}"
            }
        };
        service.Image = null;
    });

builder.Build().Run();
