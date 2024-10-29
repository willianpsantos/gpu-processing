using GPU.Placeholders.Processing.WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
