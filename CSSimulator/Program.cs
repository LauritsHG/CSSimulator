// Add services to the container.
//builder.Services.AddRazorPages();


//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();
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


