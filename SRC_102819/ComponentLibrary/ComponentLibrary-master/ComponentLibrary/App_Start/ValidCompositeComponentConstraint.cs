using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace TE.ComponentLibrary.ComponentLibrary
{
	/// <summary>
	/// </summary>
	/// <seealso cref="System.Web.Http.Routing.IHttpRouteConstraint"/>
	public class ValidCompositeComponentConstraint : IHttpRouteConstraint
	{
		/// <summary>
		/// Determines whether this instance equals a specified route.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="route">The route to compare.</param>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="values">A list of parameter values.</param>
		/// <param name="routeDirection">The route direction.</param>
		/// <returns>True if this instance equals a specified route; otherwise, false.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values,
			HttpRouteDirection routeDirection)
		{
			object value;
			if (values.TryGetValue(parameterName, out value) && value != null)
			{
				if (value is string)
				{
					var stringValue = (string)value;
					String[] validCompositeComponents = { "sfg", "package" };
					return validCompositeComponents.Any(word => word.Equals(stringValue, StringComparison.InvariantCultureIgnoreCase));
				}
			}
			return false;
		}
	}
}