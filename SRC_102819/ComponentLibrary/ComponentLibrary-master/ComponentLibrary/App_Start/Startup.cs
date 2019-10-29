using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using Swashbuckle.Application;
using TE.ComponentLibrary.ComponentLibrary;
using TE.ComponentLibrary.ComponentLibrary.App_Start;

[assembly: OwinStartup(typeof(Startup))]

namespace TE.ComponentLibrary.ComponentLibrary
{
	/// <summary>
	/// The startup class for
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Configurations the specified application.
		/// </summary>
		/// <param name="app">The application.</param>
		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();
			config.Filters.Add(new ElmahHandleErrorApiAttribute());
		    if (ConfigurationManager.AppSettings["ENABLE_AUTH"] == "true")
		    {
		        config.Filters.Add(new ComponentLibraryAuthorizeAttribute());
		        ConfigureOAuth(app);
            }
			WebApiConfig.Register(config);
			app.UseWebApi(config);
			GlobalConfiguration.Configuration.EnableSwagger(c =>
			{
				c.SingleApiVersion("v1", "Component Library API");
				c.UseFullTypeNameInSchemaIds();
				c.IncludeXmlComments(string.Format(@"{0}\bin\ComponentLibrary.XML",
					System.AppDomain.CurrentDomain.BaseDirectory));
				c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			}).EnableSwaggerUi(c => c.DisableValidator());
			GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
		}

		/// <summary>
		/// Configures the OAuth.
		/// </summary>
		/// <param name="app">The application.</param>
		public void ConfigureOAuth(IAppBuilder app)
		{
			var tokenValidationParameter = new TokenValidationParameters
			{
				ValidAudience = ConfigurationManager.AppSettings["Audience"]
			};
			app.UseWindowsAzureActiveDirectoryBearerAuthentication(
			new WindowsAzureActiveDirectoryBearerAuthenticationOptions
			{
				TokenValidationParameters = tokenValidationParameter,
				Tenant = ConfigurationManager.AppSettings["Tenant"]
			});
		}
	}
}