using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents data type of static file
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class StaticFileDataType : ISimpleDataType
    {
        private readonly IStaticFileRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileDataType"/> class.
        /// </summary>
        public StaticFileDataType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileDataType"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public StaticFileDataType(IStaticFileRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task<object> Parse(object columnData)
        {
            if (columnData is Dictionary<string, object>)
            {
                try
                {
                    var dictionary = ((Dictionary<string, object>) columnData);
                    string id;
                    if (dictionary.ContainsKey("Id"))
                        id = (string) dictionary["Id"];
                    else
                    {
                        id = (string) dictionary["id"];
                    }
                    return await _repository.GetById(id);
                }
                catch (Exception e)
                {
                    throw new FormatException(e.Message);
                }
            }

            var staticFileString =  (string)columnData;
            try
            {
                var staticFile = await _repository.GetById(staticFileString);
                return staticFile;
            }
            catch (ResourceNotFoundException)
            {
                return staticFileString;
                throw;
            }
        }
    }
}