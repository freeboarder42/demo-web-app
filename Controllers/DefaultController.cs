using System;
using System.Reflection;
using DemoWebApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DemoWebApp.Controllers
{
	[ApiController]
	public class DefaultController: ControllerBase
	{
		private readonly IConfiguration configuration;
		private readonly ILogger<DefaultController> logger;
		private readonly AppSettings appSettings;

		public DefaultController(IOptions<AppSettings> appOptions, IConfiguration configuration, ILogger<DefaultController> logger)
		{
			this.appSettings = appOptions.Value;
			this.configuration = configuration;
			this.logger = logger;
		}

		[HttpGet, Route("")]
		public IActionResult GetInfo()
		{
			this.logger.LogInformation("GetInfo was called");

			var entryAssembly = Assembly.GetEntryAssembly();
			var version = entryAssembly.GetName().Version;
			var product = entryAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
			var versionString = $"{version.Major}.{version.Minor}.{version.Build}";
			var content = $"<html><head><title>{product}</title></head>" +
				"<body style=\"margin:50px;font-family:Verdana,Helvetica,Arial\">" +
				$"   <h1>{product}</h1>" +
				$"   <div><b>Version</b>: {versionString}</div>" +
				$"   <div style=\"margin-top: 10px\"><b>Environment from app settings</b>: {this.appSettings.Environment}</div>" +
				$"   <div style=\"margin-top: 10px\"><b>Environment from environment variable</b>: {GetEnvironmentFromEnvironmentVariable()}</div>" +
				$"   <div style=\"margin-top: 10px\"><b>Configuration string from app settings</b>: {this.configuration.GetConnectionString("demo")}</div>" +
				"   <div style=\"margin-top: 10px\"><a href=\"/weatherforecast\">Weather Forecast</a></div>" +
				"</body>" +
				"</html>";
			return new ContentResult()
			{
				Content = content,
				ContentType = "text/html",
			};
		}

		private string GetEnvironmentFromEnvironmentVariable()
		{
			var environment = Environment.GetEnvironmentVariable("App:Environment");
			if (!String.IsNullOrEmpty(environment))
			{
				return $"{environment} (colon-separated name)";
			}

			environment = Environment.GetEnvironmentVariable("App.Environment");
			if (!String.IsNullOrEmpty(environment))
			{
				return $"{environment} (dot-separated name)";
			}

			return "NOT FOUND";
		}
	}
}
