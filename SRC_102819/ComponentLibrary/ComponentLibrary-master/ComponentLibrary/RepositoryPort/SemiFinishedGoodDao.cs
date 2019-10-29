using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// Stored representation of SFG.
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity"/>
	[BsonSerializer(typeof(CompositeComponentDaoSerializer<SemiFinishedGoodDao>))]
	public class SemiFinishedGoodDao : CompositeComponentDao
	{
		/// <summary>
		/// The SFG code.
		/// </summary>
		public const string Code = "sfg_code";

		/// <summary>
		/// The SFG level2
		/// </summary>
		public const string SfgLevel2 = "sfg_level_2";

		/// <summary>
		/// The SFG level1
		/// </summary>
		public const string SfgLevel1 = "sfg_level_1";

	    /// <summary>
	    /// Short Description
	    /// </summary>
	    public const string ShortDescription = "short_description";


		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodDao"/> class.
		/// </summary>
		public SemiFinishedGoodDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodDao"/> class.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		/// <param name="searchKeywords">The search keywords.</param>
		public SemiFinishedGoodDao(CompositeComponent sfg, List<string> searchKeywords) : base(sfg, searchKeywords)
		{
		}

		/// <summary>
		/// To the composite component.
		/// </summary>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <returns></returns>
		public CompositeComponent ToCompositeComponent(ICompositeComponentDefinition sfgDefintion)
		{
			var sfg = new CompositeComponent()
			{
				Code = Columns[SemiFinishedGoodDao.Code].ToString(),
				Group = Columns[SemiFinishedGoodDao.SfgLevel1].ToString(),
				Headers = GetHeaders(sfgDefintion),
				CompositeComponentDefinition = sfgDefintion
			};
			if (Columns.ContainsKey(Composition))
			{
				sfg.ComponentComposition = ((ComponentCompositionDao)Columns[Composition])
					.ToComponentComposition();
			}
			return sfg;
		}
	}
}