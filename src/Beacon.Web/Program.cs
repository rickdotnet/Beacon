using Beacon.Web;

var app = 
    WebApplication.CreateBuilder(args)
        .ConfigureServices()
        .ConfigurePipeline();

app.Run();