using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.ICodePrefixTypeMappingRepository"/>
	public class CodePrefixTypeMappingRepository : ICodePrefixTypeMappingRepository
	{
		private readonly IMongoCollection<CodePrefixTypeMappingDao> _mongoCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="CodePrefixTypeMappingRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		public CodePrefixTypeMappingRepository(IMongoCollection<CodePrefixTypeMappingDao> mongoCollection)
		{
			_mongoCollection = mongoCollection;
		}

		/// <summary>
		/// Adds the specified code prefix type mapping.
		/// </summary>
		/// <param name="codePrefixTypeMapping">The code prefix type mapping.</param>
		/// <returns></returns>
		public async Task Add(CodePrefixTypeMapping codePrefixTypeMapping)
		{
			CodePrefixTypeMappingDao codePrefixTypeMappingDao = new CodePrefixTypeMappingDao(codePrefixTypeMapping);
			var existingCount = await _mongoCollection.CountAsync(dao => dao.CodePrefix == codePrefixTypeMapping.CodePrefix);
			if (existingCount > 0)
				throw new DuplicateResourceException($"Type mapping for code prefix {codePrefixTypeMapping.CodePrefix} already exists.");
			else
				await _mongoCollection.InsertOneAsync(codePrefixTypeMappingDao);
		}

		/// <summary>
		/// Gets the specified code prefix.
		/// </summary>
		/// <param name="codePrefix">The code prefix.</param>
		/// <returns></returns>
		public async Task<CodePrefixTypeMapping> Get(string codePrefix)
		{
			var codePrefixTypeMappingDao =
				(await _mongoCollection.FindAsync(dao => dao.CodePrefix == codePrefix)).FirstOrDefault();
			if (codePrefixTypeMappingDao == null)
				throw new ResourceNotFoundException($"Type mapping for code prefix {codePrefix}");
			return codePrefixTypeMappingDao.ToCodePrefixTypeMapping();
		}
	}
}