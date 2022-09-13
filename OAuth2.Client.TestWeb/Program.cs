﻿using Microsoft.AspNetCore.Internal;

using OAuth2;
using OAuth2.Client.Models;

using IHostingEnvironment		= Microsoft.AspNetCore.Hosting.IHostingEnvironment;

var env                         = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
new WebHostBuilder()
	.ConfigureAppConfiguration(cfgBuilder =>
	{
		cfgBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)           // Общая часть настроек
				  .AddJsonFile("appsettings.secret.json", optional: true, reloadOnChange: false)    // Общая часть настроек с паролями, ключами и т.п., что нельзя распространять
				  .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);   // Настройки для текущего окружения: разработка, боевое и т.п.
	})
	.UseKestrel()
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
		var env					= appBuilder.ApplicationServices.GetRequiredService<IHostingEnvironment>();
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
