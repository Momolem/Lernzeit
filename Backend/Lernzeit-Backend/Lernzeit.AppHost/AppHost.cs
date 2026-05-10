
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
string registryEndpoint = "dummy";
string registryRepository = "dummy";
#if !DEBUG
registryEndpoint = builder.AddParameter("RegistryEndpoint");
registryRepository = builder.AddParameter("RegistryRepository");
#endif
var registry = builder.AddContainerRegistry("ghcr", registryEndpoint, registryRepository);

// URLs — configure per deployment via Parameters__BackendUrl / Parameters__FrontendUrl env vars
// BackendUrl is baked into the React bundle at image build time (CRA limitation)
var backendUrl = builder.AddParameter("BackendUrl");
var frontendUrl = builder.AddParameter("FrontendUrl");

var backend = builder
    .AddProject<Projects.Lernzeit_Backend>("lernzeit-backend")
    .WithContainerRegistry(registry)
    .WithReference(db)
    .WithEnvironment("Authentication__Google__ClientId", googleClientId)
    .WithEnvironment("Authentication__Google__ClientSecret", googleClientSecret)
    .WithEnvironment("FrontendUrl", frontendUrl)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "lernzeit_backend";
        service.Ports.Clear();
        service.Expose.Clear();
        service.Expose.Add("8080");
    });

builder
    .AddViteApp("frontend", "../../../frontend")
    .WithEnvironment("REACT_APP_BACKEND_URL", backendUrl)
    .WithContainerRegistry(registry)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "lernzeit_frontend";
    });;
    

builder.Build().Run();
