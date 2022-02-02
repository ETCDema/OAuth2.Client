using OAuth2;
using OAuth2.Client.Models;

var builder						= WebApplication.CreateBuilder(args);
var env							= Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg							= builder	.Configuration
											.AddJsonFile("appsettings.json",		optional: true, reloadOnChange: false)	// ����� ����� ��������
											.AddJsonFile("appsettings.secret.json", optional: true, reloadOnChange: false)  // ����� ����� �������� � ��������, ������� � �.�., ��� ������ ��������������
											.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)  // ��������� ��� �������� ���������: ����������, ������ � �.�.
											.Build();

var clientsCfg                  = cfg.GetRequiredSection("OAuth2Clients");
foreach (var clientCfg in clientsCfg.GetChildren())
{
	var opt                     = new Options(clientCfg);
	var ctName                  = clientCfg.Key;
	var ctype					= Type.GetType(ctName, true)!;
	var client					= (IClient)Activator.CreateInstance(ctype, opt)!;
	builder.Services.AddSingleton<IClient>(client);
}

// Add services to the container.
builder.Services.AddControllersWithViews();

var app							= builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
