﻿using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TE.ComponentLibrary.ComponentLibrary
{
	/// <summary>
	/// </summary>
	/// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute"/>
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// Occurs before the action method is invoked.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (actionContext.ModelState.IsValid == false)
			{
				actionContext.Response = actionContext.Request.CreateErrorResponse(
					HttpStatusCode.BadRequest, actionContext.ModelState);
			}
		}
	}
}