using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
    /// <summary>
    /// The Controller for 
    /// </summary>
    /// <seealso cref="BaseController" />
    public abstract class ComponentController : BaseController
    {
        /// <summary>
        /// The counter repository
        /// </summary>
        protected readonly ICounterRepository CounterRepository;

        /// <summary>
        /// The master data repository
        /// </summary>
        protected readonly IMasterDataRepository MasterDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentController"/> class.
        /// </summary>
        /// <param name="counterRepository">The counter repository.</param>
        /// <param name="masterDataRepository">The master data repository.</param>
        protected ComponentController(ICounterRepository counterRepository,
            IMasterDataRepository masterDataRepository)
        {
            CounterRepository = counterRepository;
            MasterDataRepository = masterDataRepository;
        }

        /// <summary>
        /// Gets the group column.
        /// </summary>
        /// <value>
        /// The group column.
        /// </value>
        protected abstract string GroupColumn { get; }

        /// <summary>
        /// Adds the new headers and columns.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The component.</param>
        /// <param name="componentDefinition">The component definition.</param>
        /// <returns></returns>
        protected async Task<T> AddNewHeadersAndColumnsIfAny<T>(T component, IComponentDefinition componentDefinition) where T : IComponent
        {
            if (componentDefinition.Headers.Count > 0)
            {
                IList<IHeaderData> headers = new List<IHeaderData>();
                foreach (var componentDefinitionHeader in componentDefinition.Headers)
                {
                    var headerData = component.Headers.FirstOrDefault(h => h.Name == componentDefinitionHeader.Name);
                    IList<IColumnData> columnDatas = new List<IColumnData>();
                    if (headerData != null)
                    {
                        foreach (var columnDefinition in componentDefinitionHeader.Columns)
                        {
                            columnDatas.Add(new ColumnData(columnDefinition.Name, columnDefinition.Key,
                                headerData.Columns.FirstOrDefault(c => c.Name == columnDefinition.Name)?.Value));
                        }
                        headerData.Columns = columnDatas;
                    }
                    else
                    {
                        foreach (var columnDefinition in componentDefinitionHeader.Columns)
                        {
                            columnDatas.Add(new ColumnData(columnDefinition.Name, columnDefinition.Key, null));
                        }
                        headerData = new HeaderData(componentDefinitionHeader.Name, componentDefinitionHeader.Key) { Columns = columnDatas };
                    }
                    headers.Add(headerData);
                }
                component.Headers = headers;
            }
            return component;
        }

        /// <summary>
        /// Converts the json to primitives.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        protected object ConvertJsonToPrimitives(object subject)
        {
            if (subject is JObject)
            {
                return ConvertJsonToPrimitives(((JObject)subject).ToObject<Dictionary<string, object>>());
            }
            var subjectAsDict = subject as Dictionary<string, object>;
            if (subjectAsDict == null)
            {
                return subject;
            }
            var dictionary = new Dictionary<string, object>();
            foreach (var keyValuePair in subjectAsDict)
            {
                if (keyValuePair.Value is JObject)
                {
                    var jObject = (JObject)keyValuePair.Value;
                    dictionary[ToPascal(keyValuePair.Key)] =
                        ConvertJsonToPrimitives(jObject.ToObject<Dictionary<string, object>>());
                }
                else if (keyValuePair.Value is JArray)
                {
                    dictionary[ToPascal(keyValuePair.Key)] = ((JArray)keyValuePair.Value).ToObject<List<object>>()
                        .Select(ConvertJsonToPrimitives).ToList();
                }
                else if (keyValuePair.Value is string && (string)keyValuePair.Value == "NA")
                {
                    dictionary[ToPascal(keyValuePair.Key)] = ColumnDefinition.Na;
                }
                else
                {
                    dictionary[ToPascal(keyValuePair.Key)] = keyValuePair.Value;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Fetches the keywords.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Search keyword should be minimum 3 letter long.
        /// or
        /// Atleast one of the search keyword should be more than 3 letter long.
        /// </exception>
        protected List<string> FetchKeywords(string searchQuery)
        {
            if ((searchQuery == null) || (searchQuery.Length < 3))
                throw new ArgumentException("Search keyword should be minimum 3 letter long.");
            var keywords = searchQuery.Split(' ').Where(k => k.Length > 2).ToList();
            if (!keywords.Any())
                throw new ArgumentException("Atleast one of the search keyword should be more than 3 letter long.");
            return keywords;
        }

        /// <summary>
        /// Formats the search response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count">The count.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="materialSearchDtos">The material search dtos.</param>
        /// <returns></returns>
        protected Dictionary<string, object> FormatSearchResponse<T>(long count, int pageNumber, int batchSize, IList<T> materialSearchDtos, string sortColumn = null, SortOrder? sortOrder = null)
        {
            var result = new Dictionary<string, object>();
            result["recordCount"] = count;
            result["totalPages"] = count % batchSize == 0 ? count / batchSize : count / batchSize + 1;
            result["batchSize"] = batchSize;
            result["pageNumber"] = pageNumber;
            result["items"] = materialSearchDtos;
            if (sortColumn != null)
            {
                result["sortColumn"] = sortColumn;
            }
            if (sortOrder != null)
            {
                result["sortOrder"] = sortOrder.ToString();
            }
            return result;
        }

        /// <summary>
        ///     Gets the Component Level 2 from a component.
        /// </summary>
        /// <param name="component">Dictionary of Component JSON</param>
        /// <exception cref="BetterKeyNotFoundException"></exception>
        /// <returns></returns>
        protected string GetGroup(IDictionary<string, object> component)
        {
            Dictionary<string, object> classification;
            try
            {
                classification = (Dictionary<string, object>)component["Classification"];
            }
            catch (KeyNotFoundException)
            {
                throw new BetterKeyNotFoundException("Classification");
            }

            try
            {
                return (string)classification[GroupColumn];
            }
            catch (KeyNotFoundException)
            {
                throw new BetterKeyNotFoundException(GroupColumn);
            }
        }

        /// <summary>
        /// Convenient method.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected IDictionary<string, object> NormalizeRequest(object request)
        {
            if (request == null)
                throw new ArgumentException("Request is null.");
            return (IDictionary<string, object>)ConvertJsonToPrimitives(request);
        }

        /// <summary>
        /// To the pascal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected string ToPascal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}