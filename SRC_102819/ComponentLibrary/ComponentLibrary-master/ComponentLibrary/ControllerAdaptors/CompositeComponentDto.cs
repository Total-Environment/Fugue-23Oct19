using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
	/// <summary>
	/// The Dto representation of SFG.
	/// </summary>
	public class CompositeComponentDto
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDto"/> class.
		/// </summary>
		public CompositeComponentDto()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDto"/> class.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		public CompositeComponentDto(CompositeComponent sfg)
		{
			Code = sfg.Code;
			Group = sfg.Group;
			Headers = new List<HeaderDataTypeDto>();

			foreach (var headerData in sfg.Headers)
			{
				var headerDefinition =
					sfg.CompositeComponentDefinition.Headers.FirstOrDefault(h => headerData.Key == h.Key);

				var headerDto = new HeaderDataTypeDto()
				{
					Key = headerData.Key,
					Name = headerData.Name,
					Columns = new List<ColumnDataTypeDto>()
				};
				foreach (var column in headerData.Columns)
				{
					var dataTypeDto = new DataTypeDto(); ;
					if (headerDefinition != null)
					{
						var columnDefinition = headerDefinition.Columns.FirstOrDefault(c => column.Key == c.Key);
						if (columnDefinition != null)
							dataTypeDto.SetDomain(columnDefinition.DataType);
					}

					var columnDto = new ColumnDataTypeDto()
					{
						Key = column.Key,
						Name = column.Name,
						Value = column.Value,
						DataType = dataTypeDto
					};

					headerDto.Columns.Add(columnDto);
				}
				Headers.Add(headerDto);
			}

			if (sfg.ComponentComposition != null)
			{
				ComponentComposition = new ComponentCompositionDto(sfg.ComponentComposition);
			}
		}

		/// <summary>
		/// Gets or sets the SFG code.
		/// </summary>
		/// <value>The code.</value>
		public string Code { get; set; }

		/// <summary>
		/// Gets or sets the SFG group.
		/// </summary>
		/// <value>The group.</value>
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public List<HeaderDataTypeDto> Headers { get; set; }

		/// <summary>
		/// Gets or sets the semi finished good composition.
		/// </summary>
		/// <value>The semi finished good composition.</value>
		public ComponentCompositionDto ComponentComposition { get; set; }

		/// <summary>
		/// To the domain.
		/// </summary>
		/// <returns></returns>
		public CompositeComponent ToDomain()
		{
			var compositeComponent = new CompositeComponent
			{
				Code = Code,
				Group = Group,
				Headers = new List<IHeaderData>(),
				ComponentComposition = ComponentComposition.ToDomain()
			};
			foreach (var headerDto in Headers)
			{
				var headerData = new HeaderData(headerDto.Name, headerDto.Key);
				foreach (var column in headerDto.Columns)
				{
					var columnData = new ColumnData(column.Name, column.Key, column.Value);
					headerData.AddColumns(columnData);
				}
				compositeComponent.Headers.Add(headerData);
			}

			return compositeComponent;
		}
	}
}