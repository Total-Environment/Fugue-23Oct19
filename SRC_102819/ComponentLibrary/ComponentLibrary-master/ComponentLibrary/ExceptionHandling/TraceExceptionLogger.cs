using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.ExceptionHandling;

namespace TE.ComponentLibrary.ComponentLibrary.ExceptionHandling
{
	/// <summary>
	/// </summary>
	/// <seealso cref="System.Web.Http.ExceptionHandling.ExceptionLogger"/>
	public class TraceExceptionLogger : ExceptionLogger
	{
		/// <summary>
		/// Logs the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Log(ExceptionLoggerContext context)
		{
			Trace.TraceError($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss},{context.Exception}");
		}
	}
}