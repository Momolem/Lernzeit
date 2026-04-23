
using Aspire.Hosting.Docker.Resources.ServiceNodes;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard();

var db = builder
    .AddPostgres("postgres")
    .PublishAsDockerComposeService((r, s) => { s.Name = "lernzeit_postgres"; });

var googleClientId = builder.AddParameter("GoogleClientId", secret: true);
var googleClientSecret = builder.AddParameter("GoogleClientSecret", secret: true);

// Container registry — set RegistryEndpoint to e.g. ghcr.io and
// RegistryRepository to your GitHub org/user, e.g. ghcr.io/your-org
var registryEndpoint = builder.AddParameter("RegistryEndpoint");
var registryRepository = builder.AddParameter("RegistryRepository");
var registry = builder.AddContainerRegistry("ghcr", registryEndpoint, registryRepository);

var backend = builder
    .AddProject<Projects.Lernzeit_Backend>("Backend")
    .WithContainerRegistry(registry)
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
    });

// REACT_APP_BACKEND_URL is baked into the React bundle at image build time (CRA limitation).
// Set via Parameters__BackendUrl env var in CI, or via user secrets locally.
var backendUrl = builder.AddParameter("BackendUrl");

var frontend = builder.AddDockerfile("frontend", "../../../frontend")
    .WithContainerRegistry(registry)
    .WithHttpEndpoint(port: 3000, targetPort: 80, name: "http")
    .WithBuildArg("REACT_APP_BACKEND_URL", backendUrl)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "lernzeit_frontend";
        service.Ports.Add("3000:80");
    });

builder.Build().Run();
