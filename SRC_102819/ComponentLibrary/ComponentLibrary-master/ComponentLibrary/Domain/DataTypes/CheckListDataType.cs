using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents data type of checklist.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class CheckListDataType : ISimpleDataType
    {
        private readonly ICheckListRepository _repository;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CheckListDataType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListDataType"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public CheckListDataType(ICheckListRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task<object> Parse(object columnData)
        {
            string id;
            if (columnData is Dictionary<string, object>)
            {
                id = (string) GetValueFromDictionary((Dictionary<string, object>) columnData, "Id");
                try
                {
                    var checkList = await _repository.GetById(id);
                    return new CheckListValue(checkList.CheckListId);
                }
                catch (ResourceNotFoundException e)
                {
                    throw new FormatException(e.Message, e);
                }
            }
            id = (string)columnData;
            try
            {
                var checkList = await _repository.GetById(id);
                return new CheckListValue(checkList.CheckListId);
            }
            catch (ResourceNotFoundException e)
            {
                throw new FormatException($"Checklist with id {id} does not exist.", e);
            }
        }

        private static object GetValueFromDictionary(IDictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return dictionary.ContainsKey(key.ToLower()) ? dictionary[key.ToLower()] : null;
        }
    }
}