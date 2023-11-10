using Microsoft.AspNetCore.Internal;

using OAuth2;
using OAuth2.Client.Models;

var env                         = _getArgValue(args, HostDefaults.EnvironmentKey)
								?? Environment.GetEnvironmentVariable("ASPNETCORE_"+WebHostDefaults.EnvironmentKey.ToUpper())
								?? "Production";

Console.WriteLine("Use environment="+env);

var cfg							= new ConfigurationBuilder()
									.SetBasePath(Directory.GetCurrentDirectory())
									.AddJsonFile("appsettings.json",				optional: true, reloadOnChange: false)	// Общая часть настроек
									.AddJsonFile("appsettings.secret.json",			optional: true, reloadOnChange: false)  // Общая часть настроек с паролями, ключами и т.п., что нельзя распространять
									.AddJsonFile($"appsettings.{env}.json",			optional: true, reloadOnChange: false)	// Настройки для текущего окружения: разработка, боевое и т.п.
									.AddJsonFile($"appsettings.{env}.secret.json",	optional: true, reloadOnChange: false)
									.Build();

new WebHostBuilder()
	.ConfigureAppConfiguration(cfgBuilder =>
	{
		cfgBuilder.Sources.Clear();								// Сносим то, что создано автоматом. TODO: возможно стоит как-то иначе собирать host?
		cfgBuilder.AddConfiguration(cfg);						// Добавляем ранее созданную конфигурацию
	})
	.UseKestrel(opt =>
	{
		opt.Configure(cfg.GetSection("Kestrel"));
	})
	.ConfigureServices((ctx, services) =>
	{
		var clientsCfg                  = ctx.Configuration.GetSection("OAuth2Clients");
		foreach (var clientCfg in clientsCfg.GetChildren())
		{
			var opt                     = new Options(clientCfg);
			var ctName                  = clientCfg.Key;
			var ctype                   = Type.GetType(ctName, true)!;
			var client                  = (IClient)Activator.CreateInstance(ctype, opt)!;
			services.AddSingleton<IClient>(client);
		}

		services.AddMvc();
	})
	.Configure(appBuilder =>
	{
#if NET4
		var env					= appBuilder.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
#else
		var env					= appBuilder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
#endif
		if (env.IsDevelopment())
		{
			appBuilder.UseExceptionHandler("/Home/Error");
		}

#if NET4
		appBuilder
				.UseStaticFiles()
				.UseEndpointRouting()
				.UseEndpoint();
#else
		appBuilder
				.UseStaticFiles()
				.UseRouting()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapControllers();
				});
#endif
	})
	.Build()
	.Run();

static string? _getArgValue(string[] args, string name)
{
	if (args!=null && args.Length>0)
	{
		name					= "--"+name+"=";
		foreach (string arg in args)
		{
			if (arg.StartsWith(name))
#pragma warning disable IDE0057 // Использовать оператор диапазона
				return arg.Substring(name.Length);
#pragma warning restore IDE0057 // Использовать оператор диапазона
		}
	}

	return null;
}
