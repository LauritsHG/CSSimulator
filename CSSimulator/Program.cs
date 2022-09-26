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
using CSSimulator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddActorSystem();
builder.Services.AddHostedService<ActorSystemClusterHostedService>();
//builder.Services.AddHostedService<ChargerSimulator>();

var app = builder.Build();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
Proto.Log.SetLoggerFactory(loggerFactory);

app.MapGet("/", () => Task.FromResult("Hello, Proto.Cluster Central System!"));

app.Run();


