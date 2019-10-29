using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
	/// <summary>
	/// </summary>
	public class CPRCoefficientDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CPRCoefficientDto"/> class.
		/// </summary>
		public CPRCoefficientDto()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="CPRCoefficientDto"/> class.
		/// </summary>
		/// <param name="cprCoefficient">The CPR coefficient.</param>
		public CPRCoefficientDto(CPRCoefficient cprCoefficient)
		{
			this.Columns = cprCoefficient.Columns.Select(c => new ColumnDataTypeDto { Key = c.Key, Name = c.Name, Value = c.Value }).ToList();
			this.CPR = cprCoefficient.CPR;
		}

		/// <summary>
		/// Gets or sets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public List<ColumnDataTypeDto> Columns { get; set; }

		/// <summary>
		/// Gets the CPR.
		/// </summary>
		/// <value>The CPR.</value>
		public double CPR { get; set; }

		/// <summary>
		/// To the domain.
		/// </summary>
		/// <returns></returns>
		public CPRCoefficient ToDomain()
		{
			return new CPRCoefficient(Columns.Select(c => new ColumnData(c.Name, c.Key, c.Value)));
		}
	}
}