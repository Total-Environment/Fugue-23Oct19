using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public class CompositeComponentDao
	{
		/// <summary>
		/// The SFG Composition.
		/// </summary>
		public const string Composition = "Composition";

		/// <summary>
		/// Constant for created by
		/// </summary>
		public const string CreatedBy = "created_by";

		/// <summary>
		/// Constant for date created
		/// </summary>
		public const string DateCreated = "date_created";

		/// <summary>
		/// Constant for last amended
		/// </summary>
		public const string DateLastAmended = "date_last_amended";

		/// <summary>
		/// Constant for last amended by
		/// </summary>
		public const string LastAmendedBy = "last_amended_by";

		/// <summary>
		/// Constant for search keywords
		/// </summary>
		public const string SearchKeywords = "SearchKeywords";

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDao"/> class.
		/// </summary>
		public CompositeComponentDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDao"/> class.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		/// <param name="searchKeywords">The search keywords.</param>
		public CompositeComponentDao(CompositeComponent sfg, List<string> searchKeywords)
		{
			Columns = new Dictionary<string, object>();
			Columns.Add("SearchKeywords", searchKeywords);

			foreach (var headerDataDao in sfg.Headers)
				foreach (var columnDataDao in headerDataDao.Columns)
				{
					var columnKey = columnDataDao.Key.ToLower();
					Columns[columnKey] = columnDataDao.Value;
				}

			Columns.Add("Composition", new ComponentCompositionDao(sfg.ComponentComposition));
		}

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The columns.</value>
		public Dictionary<string, object> Columns { get; set; }

		/// <summary>
		/// Gets the headers.
		/// </summary>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <returns></returns>
		protected List<IHeaderData> GetHeaders(ICompositeComponentDefinition sfgDefintion)
		{
			var headerDatas = new List<IHeaderData>();
			foreach (var header in sfgDefintion.Headers)
			{
				var headerData = new HeaderData(header.Name, header.Key);
				foreach (var columnDefinition in header.Columns)
				{
					var columnData = new ColumnData(columnDefinition.Name, columnDefinition.Key, null);
					if (Columns.ContainsKey(columnDefinition.Key))
					{
						columnData.Value = Columns[columnDefinition.Key];
					}
					headerData.AddColumns(columnData);
				}
				headerDatas.Add(headerData);
			}
			return headerDatas;
		}
	}
}