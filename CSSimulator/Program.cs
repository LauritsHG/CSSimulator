using CSSimulator.ActorSetup;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment()) builder.Services.AddConsulActorSystem();
else builder.Services.AddActorSystem(builder.Configuration);
builder.Services.AddHostedService<ActorSystemClusterHostedService>();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.UseCors();
app.MapControllers();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
Proto.Log.SetLoggerFactory(loggerFactory);

//app.MapGet("/", () => Task.FromResult("Hello, Proto.Cluster Central System!"));

app.Run();


