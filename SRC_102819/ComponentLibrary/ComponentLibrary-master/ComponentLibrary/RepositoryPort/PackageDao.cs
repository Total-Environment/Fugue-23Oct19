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
	[BsonSerializer(typeof(CompositeComponentDaoSerializer<PackageDao>))]
	public class PackageDao : CompositeComponentDao
	{
	    /// <summary>
	    /// Short description
	    /// </summary>
	    public const string ShortDescription = "short_description";
		/// <summary>
		/// The code
		/// </summary>
		public const string Code = "package_code";

		/// <summary>
		/// The PKG level2
		/// </summary>
		public const string PkgLevel2 = "pkg_level_2";

		/// <summary>
		/// The PKG level1
		/// </summary>
		public const string PkgLevel1 = "pkg_level_1";

		/// <summary>
		/// Initializes a new instance of the <see cref="PackageDao"/> class.
		/// </summary>
		public PackageDao()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PackageDao"/> class.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		/// <param name="searchKeywords">The search keywords.</param>
		public PackageDao(CompositeComponent sfg, List<string> searchKeywords) : base(sfg, searchKeywords)
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
				Code = Columns[PackageDao.Code].ToString(),
				Group = Columns[PackageDao.PkgLevel1].ToString(),
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