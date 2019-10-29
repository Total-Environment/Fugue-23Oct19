using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace TE.ComponentLibrary.ComponentLibrary.ExceptionHandling
{
	/// <summary>
	/// </summary>
	/// <seealso cref="System.Web.Http.ExceptionHandling.ExceptionHandler"/>
	public class OopsExceptionHandler : ExceptionHandler
	{
		/// <summary>
		/// When overridden in a derived class, handles the exception synchronously.
		/// </summary>
		/// <param name="context">The exception handler context.</param>
		public override void Handle(ExceptionHandlerContext context)
		{
			context.Result = new TextPlainErrorResult
			{
				Request = context.ExceptionContext.Request,
				Content = "Oops! Sorry! Something went wrong." +
					  "Please contact support@total-environment.com so we can try to fix it."
			};
		}

		private class TextPlainErrorResult : IHttpActionResult
		{
			public HttpRequestMessage Request { get; set; }

			public string Content { get; set; }

			public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
			{
				HttpResponseMessage response =
								 new HttpResponseMessage(HttpStatusCode.InternalServerError);
				response.Content = new StringContent(Content);
				response.RequestMessage = Request;
				return Task.FromResult(response);
			}
		}
	}
}