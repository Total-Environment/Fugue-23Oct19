using System;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using Elmah;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
	/// <summary>
	/// The API controller with Elmah Logs
	/// </summary>
	/// <seealso cref="System.Web.Http.ApiController"/>
	public class BaseController : ApiController
	{
		/// <summary>
		/// Logs to elmah.
		/// </summary>
		/// <param name="e">The e.</param>
		protected void LogToElmah(Exception e)
		{
			ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
			Trace.TraceError($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss},{e}");
		}
	}
}