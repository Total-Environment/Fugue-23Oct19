using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MongoDB.Bson;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller
{
    /// <summary>
    /// Service data controller for creating and updating service data.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController"/>
    [RoutePrefix("services")]
    public class ServiceController : ComponentController
    {
        private const string Service = "service";
        private readonly int _batchSize;
        private readonly IComponentDefinitionRepository<IServiceDefinition> _definitionRepository;
        private readonly IFilterCriteriaBuilder _filterCriteriaBuilder;
        private readonly IClassificationDefinitionRepository _serviceClassificationDefinitionRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceSapSyncer _serviceSapSyncer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceController"/> class.
        /// </summary>
        /// <param name="definitionRepository">The definition repository.</param>
        /// <param name="counterRepository">The counter repository.</param>
        /// <param name="serviceRepository">The service repository.</param>
        /// <param name="masterDataRepository">The master data repository.</param>
        /// <param name="serviceSapSyncer">The service sap syncer.</param>
        /// <param name="serviceClassificationDefinitionRepository">
        /// The service classification definition repository.
        /// </param>
        /// <param name="filterCriteriaBuilder"></param>
        public ServiceController(IComponentDefinitionRepository<IServiceDefinition> definitionRepository,
            ICounterRepository counterRepository, IServiceRepository serviceRepository,
            IMasterDataRepository masterDataRepository,
            IServiceSapSyncer serviceSapSyncer,
            IClassificationDefinitionRepository serviceClassificationDefinitionRepository,
            IFilterCriteriaBuilder filterCriteriaBuilder)
            : base(counterRepository, masterDataRepository)
        {
            _definitionRepository = definitionRepository;
            _serviceRepository = serviceRepository;
            _serviceSapSyncer = serviceSapSyncer;
            _serviceClassificationDefinitionRepository = serviceClassificationDefinitionRepository;
            _filterCriteriaBuilder = filterCriteriaBuilder;
            _batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
        }

        /// <summary>
        /// Gets the group column.
        /// </summary>
        /// <value>The group column.</value>
        protected override string GroupColumn => "Service Level 1";

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IHttpActionResult&gt;.</returns>
        [ComponentLibraryAuthorize(Permissions = "View Service properties")]
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id, [FromUri] bool? dataType = false)
        {
            try
            {
                var service = await _serviceRepository.Find(id);
                var serviceWithNewHeadersAndColumns =
                    await AddNewHeadersAndColumnsIfAny(service, service.ComponentDefinition);

                var serviceWithDefinitionHeader =
                    await AddClassificationDefinitionHeader(serviceWithNewHeadersAndColumns);

                if (dataType != null && dataType.Value)
                {
                    var responseWithDataTypes = new ServiceWithDataTypeDto(serviceWithNewHeadersAndColumns);
                    return Ok(responseWithDataTypes);
                }

                var response = new ServiceDto(serviceWithDefinitionHeader);
                return Ok(response);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
        }

        [ComponentLibraryAuthorize(Permissions = "Service SAP Sync")]
        [Route("{serviceCode}/sync")]
        [HttpPost]
        public async Task<IHttpActionResult> SyncToSap(string serviceCode)
        {
            IService service;
            var isSynced = false;
            try
            {
                service = await _serviceRepository.Find(serviceCode);
            }
            catch (ResourceNotFoundException exception)
            {
                LogToElmah(exception);
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            var serviceForSap = service as Service;
            isSynced = await _serviceSapSyncer.Sync(serviceForSap, true);

            if (isSynced)
            {
                return Ok($"Service : {serviceCode} is synced to SAP");
            }
            return StatusCode(HttpStatusCode.BadGateway);
        }

        [ComponentLibraryAuthorize(Permissions = "Old - View Service properties")]
        [Route("old/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOld(string id)
        {
            var result = await Get(id);
            if (result is OkNegotiatedContentResult<ServiceDto>)
            {
                var dto = ((OkNegotiatedContentResult<ServiceDto>) result).Content;
                return Ok(dto.Headers.ToDictionary(h => h.Name, h => h.Columns.ToDictionary(c => c.Name, c => c.Value)));
            }
            return result;
        }

        /// <summary>
        /// Gets the services by group and column.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get services by parameters")]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByGroupAndColumnName(string group, string columnName, int pageNumber, int batchSize)
        {
            columnName = columnName.ToLowerInvariant();
            IServiceDefinition serviceDefinition;
            try
            {
                serviceDefinition = await _definitionRepository.Find(group);
            }
            catch (ResourceNotFoundException resourceNotFoundException)
            {
                LogToElmah(resourceNotFoundException);
                return BadRequest(resourceNotFoundException.Message);
            }

            IHeaderDefinition headerDefinition = null;
            IColumnDefinition columnDefinition = null;
            foreach (var serviceDefinitionHeader in serviceDefinition.Headers)
            {
                foreach (var serviceDefinitionHeaderColumn in serviceDefinitionHeader.Columns)
                {
                    if (string.Equals(serviceDefinitionHeaderColumn.Key, columnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        headerDefinition = serviceDefinitionHeader;
                        columnDefinition = serviceDefinitionHeaderColumn;
                        break;
                    }
                }
            }
            if (columnDefinition == null)
            {
                return BadRequest(columnName + " is not valid column in the service definition of " + group + " group.");
            }
            if (!(columnDefinition.DataType is StaticFileDataType) && !(columnDefinition.DataType is CheckListDataType) &&
                 (!(columnDefinition.DataType is ArrayDataType) ||
                  (!(((ArrayDataType)columnDefinition.DataType).DataType is StaticFileDataType) &&
                   !(((ArrayDataType)columnDefinition.DataType).DataType is CheckListDataType))))
            {
                return BadRequest(columnName + " is neithter static file data type nor check list data type.");
            }

            var totalCount = await _serviceRepository.GetTotalCountByGroupAndColumnName(@group, columnName);
            var services = await _serviceRepository.GetByGroupAndColumnName(group, columnName, pageNumber, batchSize);
            var serviceDocumentDtos = services.Select(m => new ServiceDocumentDto(m, headerDefinition.Name, columnDefinition.Name)).ToList();
            return Ok(FormatSearchResponse(totalCount, pageNumber, batchSize, serviceDocumentDtos));
        }

        /// <summary>
        /// Gets the services by group and column name and key word.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="columnKey">Name of the column.</param>
        /// <param name="keyWord">The key word.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="batchSize">Batch size.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get services by parameters")]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByGroupAndColumnKeyAndKeyWord(string group, string columnKey, string keyWord, int pageNumber, int batchSize)
        {
            columnKey = columnKey.ToLowerInvariant();
            List<string> keywords;
            try
            {
                keywords = FetchKeywords(keyWord);
            }
            catch (ArgumentException argumentException)
            {
                LogToElmah(argumentException);
                return BadRequest(argumentException.Message);
            }

            IServiceDefinition serviceDefinition;
            try
            {
                serviceDefinition = await _definitionRepository.Find(group);
            }
            catch (ResourceNotFoundException resourceNotFoundException)
            {
                LogToElmah(resourceNotFoundException);
                return BadRequest(resourceNotFoundException.Message);
            }

            IHeaderDefinition headerDefinition = null;
            IColumnDefinition columnDefinition = null;
            foreach (var serviceDefinitionHeader in serviceDefinition.Headers)
            {
                foreach (var serviceDefinitionHeaderColumn in serviceDefinitionHeader.Columns)
                {
                    if (string.Equals(serviceDefinitionHeaderColumn.Key, columnKey, StringComparison.InvariantCultureIgnoreCase))
                    {
                        headerDefinition = serviceDefinitionHeader;
                        columnDefinition = serviceDefinitionHeaderColumn;
                        break;
                    }
                }
            }
            if (columnDefinition == null)
            {
                return BadRequest(columnKey + " is not valid column in the service definition of " + group + " group.");
            }
            if (!(columnDefinition.DataType is StaticFileDataType) && !(columnDefinition.DataType is CheckListDataType) &&
                (!(columnDefinition.DataType is ArrayDataType) ||
                 (!(((ArrayDataType)columnDefinition.DataType).DataType is StaticFileDataType) &&
                  !(((ArrayDataType)columnDefinition.DataType).DataType is CheckListDataType))))
            {
                return BadRequest(columnKey + " is neithter static file data type nor check list data type.");
            }

            var totalCount = await _serviceRepository.GetTotalCountByGroupAndColumnNameAndKeyWords(@group, columnKey, keywords);
            var services = await _serviceRepository.GetByGroupAndColumnNameAndKeyWords(group, columnKey, keywords, pageNumber, batchSize);
            var serviceDocumentDtos = services.Select(m => new ServiceDocumentDto(m, headerDefinition.Name, columnDefinition.Name)).ToList();
            return Ok(FormatSearchResponse(totalCount, pageNumber, batchSize, serviceDocumentDtos));
        }

        /// <summary>
        /// List services by latest created date
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="details">if set to <c>true</c> [details].</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get services by parameters")]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> ListServices(bool details, int batchSize = -1, int pageNumber = 1)
        {
            List<Service> services;
            List<string> keywords = new List<string> { };

            batchSize = batchSize == -1 ? _batchSize : batchSize;
            try
            {
                services = await _serviceRepository.ListComponents(pageNumber, batchSize);
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                LogToElmah(ex);
                return NotFound();
            }
            var count = await _serviceRepository.Count(keywords, string.Empty);
            var serviceDtos = await Task.WhenAll(services.Select(async m =>
                    new ServiceWithDataTypeDto(await AddNewHeadersAndColumnsIfAny(m, m.ComponentDefinition))));
            return Ok(FormatSearchResponse(count, pageNumber, batchSize, serviceDtos));
        }

        /// <summary>
        /// Creates the service using specified data.
        /// </summary>
        /// <remarks>
        /// Data is a set of key value pair. e.g. <![CDATA[ { "Classification": { "Service Level 1":
        /// "Secondary", "Service Level 2": "Electrical", "Service Level 3": "Cable & Wire", "Service
        /// Level 4": "Cable", "Service Level 5": "Low Tension", "Service Level 6": null },
        /// "General": { "Short Description": "PVC Insulated Cable", "Shade Description": "Red",
        /// "Unit Of Measure": "m", "SAP Code": "1002529", "Part of eBuild Library": false,
        /// "Generic": true, "Manufactured": true, "Service Status": "Active" }, "Purchase": {},
        /// "Planning": { "PO Lead Time in Days": "3", "Delivery Lead Time in Days": "5", "Maintain
        /// Lot": false }, "Quality": { "Governing Standard": [ "IS 1554 (Part 1)" ] }, "System
        /// Logs": {}, "Specifications": { "Grade": "Heavy Duty", "Length": null, "Width": null,
        /// "Diameter": null, "Sweep": null, "Power Consumption": null, "Fan Speed": null, "Air
        /// Delivery": null, "Number of Blades": null, "Flame Retardant": null, "Rated Current":
        /// null, "Phase": null, "Number of Modules": null, "Rated Input Voltage": { "Type": "V",
        /// "Value": "1100" }, "Number of Cores": "2", "Cross Sectional Area": { "Type": "mm²",
        /// "Value": "1.5" }, "Core Service": "Copper", "Number of Pins": null, "Insulation Type":
        /// "PVC Insulated", "Sheath Service": null, "Nominal Dimensions of Armour - Wire": null }]]>
        /// </remarks>
        /// <returns>Http Result object</returns>
        /// <response code="200">
        /// Response Body: <![CDATA[ "Classification": { "Service Level 1": "Secondary", "Service
        /// Level 2": "Electrical", "Service Level 3": "Cable & Wire", "Service Level 4": "Cable",
        /// "Service Level 5": "Low Tension", "Service Level 6": null }, "General": { "Short
        /// Description": "PVC Insulated Cable", "Shade Description": "Red", "Unit Of Measure": "m",
        /// "SAP Code": "1002529", "Part of eBuild Library": false, "Generic": true, "Manufactured":
        /// true, "Service Status": "Active" }, "Purchase": {}, "Planning": { "PO Lead Time in Days":
        /// "3", "Delivery Lead Time in Days": "5", "Maintain Lot": false }, "Quality": { "Governing
        /// Standard": [ "IS 1554 (Part 1)" ] }, "System Logs": {}, "Specifications": { "Grade":
        /// "Heavy Duty", "Length": null, "Width": null, "Diameter": null, "Sweep": null, "Power
        /// Consumption": null, "Fan Speed": null, "Air Delivery": null, "Number of Blades": null,
        /// "Flame Retardant": null, "Rated Current": null, "Phase": null, "Number of Modules": null,
        /// "Rated Input Voltage": { "Type": "V", "Value": "1100" }, "Number of Cores": "2", "Cross
        /// Sectional Area": { "Type": "mm²", "Value": "1.5" }, "Core Service": "Copper", "Number of
        /// Pins": null, "Insulation Type": "PVC Insulated", "Sheath Service": null, "Nominal
        /// Dimensions of Armour - Wire": null }, "id": "ELC000026"]]>
        /// </response>
        [ComponentLibraryAuthorize(Permissions = "Old - Create Service properties")]
        [Route("old")]
        [HttpPost]
        public async Task<IHttpActionResult> PostOld(Dictionary<string, object> serviceDto)
        {
            if (serviceDto == null)
                return BadRequest("JSON is malformed.");
            try
            {
                var dictionaryService = (IDictionary<string, object>)ConvertJsonToPrimitives(serviceDto);
                var group = GetGroup(dictionaryService);
                var definition = await _definitionRepository.Find(group);
                var service = await definition.Parse<Service>(dictionaryService, null);
                var generalHeader = dictionaryService["General"];
                if (generalHeader.GetType() == typeof(Dictionary<string, object>))
                {
                    var seriveCode = ((Dictionary<string, object>)generalHeader)["Service Code"];
                    if (seriveCode == null)
                    {
                        var counter = await CounterRepository.NextValue("Service");
                        service.Id = $"{definition.Code}{counter:D4}";
                    }
                    else
                    {
                        service.Id = seriveCode.ToString();
                        var groupCode = seriveCode.ToString().Substring(0, 3);
                        if (groupCode != definition.Code)
                        {
                            throw new InvalidDataException($"Group code of service Code: {service.Id} is invalid.");
                        }

                        var currentCounter = await CounterRepository.CurrentValue("Service");
                        var isNumeric = seriveCode.ToString().Remove(0, 3).All(char.IsDigit);
                        if (seriveCode.ToString().Remove(0, 3).Length < 4 || !isNumeric)
                        {
                            throw new InvalidDataException($"service Code: {service.Id} is invalid.");
                        }
                        var code = int.Parse(seriveCode.ToString().Remove(0, 3).TrimStart('0'));
                        if (code > currentCounter)
                        {
                            await CounterRepository.Update(code, "Service");
                        }
                    }
                }
                service.Group = group;
                await _serviceRepository.Add(service);

                if (ConfigurationManager.AppSettings["syncToSAP"] == "true")
                    Task.Factory.StartNew(() => _serviceSapSyncer.Sync(service, false));

                Trace.TraceInformation($"Inserted Service {service.Id}");

                var requestUri = Request != null
                    ? Request.RequestUri != null ? Request.RequestUri.ToString() : string.Empty
                    : string.Empty;
                return Created(requestUri + service.Id,
                    new ServiceDto(await _serviceRepository.Find(service.Id)));
            }
            catch (InvalidDataException e)
            {
                LogToElmah(e);
                Trace.TraceWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                LogToElmah(e);
                Trace.TraceWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                LogToElmah(e);
                Trace.TraceWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (BetterKeyNotFoundException e)
            {
                LogToElmah(e);
                Trace.TraceWarning($"{e.Key} was not found");
                return BadRequest($"{e.Key} was not found.");
            }
            catch (ResourceNotFoundException e)
            {
                LogToElmah(e);
                Trace.TraceWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (FormatException e)
            {
                LogToElmah(e);
                Trace.TraceWarning(e.Message);
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                LogToElmah(e);
                Trace.TraceWarning(e.Message);
                return BadRequest(e.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]ServiceDto serviceDto)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary = serviceDto.Headers.ToDictionary(h => h.Name,
                h => (object) h.Columns.ToDictionary(c => c.Name, c => c.Value));
            return await PostOld(dictionary);
        }

        /// <summary>
        /// Searches within group by keywords and return results after applying filters, pagination
        /// and sorting.
        /// </summary>
        /// <param name="serviceSearchRequest">The search within group request.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get services by parameters")]
        [Route("searchwithingroup")]
        [HttpPost]
        public async Task<IHttpActionResult> Post_SearchWithinGroup([FromBody]ServiceSearchRequest serviceSearchRequest)
        {
            var batchSize = serviceSearchRequest.BatchSize;
            List<Service> services;
            long count;
            try
            {
                var keywords = !serviceSearchRequest.IgnoreSearchQuery ? FetchKeywords(serviceSearchRequest.SearchQuery) : new List<string>();
                var serviceDefinition = await _definitionRepository.Find(serviceSearchRequest.GroupName);
                var filterCriteria = _filterCriteriaBuilder.Build(serviceDefinition, serviceSearchRequest.FilterDatas, serviceSearchRequest.GroupName, keywords);
                services =
                    await _serviceRepository.Search(filterCriteria, serviceSearchRequest.PageNumber, batchSize, serviceSearchRequest.SortColumn, serviceSearchRequest.SortOrder);
                count = await _serviceRepository.Count(filterCriteria);
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                LogToElmah(ex);
                return NotFound();
            }

            var serviceDtos = await Task.WhenAll(services.Select(async m =>
                new ServiceWithDataTypeDto(await AddNewHeadersAndColumnsIfAny(m, m.ComponentDefinition))));
            return
                Ok(FormatSearchResponse(count, serviceSearchRequest.PageNumber, batchSize, serviceDtos,
                    serviceSearchRequest.SortColumn, serviceSearchRequest.SortOrder));
        }

        /// <summary>
        /// Puts the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="serviceDto">The service.</param>
        /// <returns>Task&lt;IHttpActionResult&gt;.</returns>
        [ComponentLibraryAuthorize(Permissions = "Old - Edit Service properties")]
        [Route("old/{serviceId}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutOld(string serviceId, Dictionary<string, object> serviceDto)
        {
            try
            {
                var dictionaryService = (IDictionary<string, object>)ConvertJsonToPrimitives(serviceDto);
                var group = GetGroup(dictionaryService);
                var definition = await _definitionRepository.Find(group);
                var service = await definition.Parse<Service>(dictionaryService, null);
                service.Id = serviceId;
                await _serviceRepository.Update(service);

                if (ConfigurationManager.AppSettings["syncToSAP"] == "true")
                    await Task.Factory.StartNew(() => _serviceSapSyncer.Sync(service, true));
                return Ok(new ServiceDto(await _serviceRepository.Find(service.Id)));
            }
            catch (BetterKeyNotFoundException e)
            {
                return BadRequest($"{e.Key} was not found.");
            }
            catch (ResourceNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (FormatException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [ComponentLibraryAuthorize(Permissions = "Edit Service properties")]
        [Route("{serviceId}")]
        [HttpPut]
        public async Task<IHttpActionResult> Put(string serviceId,[FromBody] ServiceDto serviceDto)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary = serviceDto.Headers.ToDictionary(h => h.Name,
                h => (object)h.Columns.ToDictionary(c => c.Name, c => c.Value));
            return await PutOld(serviceId, dictionary);
        }

        /// <summary>
        /// Searches the specified service level1.
        /// </summary>
        /// <param name="serviceLevel1">The service level1.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page of the recordset.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get services by parameters")]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(string searchKeyword, int pageNumber, string serviceLevel1 = "",
            string sortColumn = "", SortOrder sortOrder = SortOrder.Ascending)
        {
            if ((searchKeyword == null) || (serviceLevel1 == null))
                return BadRequest("Invalid Request");
            var searchKeywords = searchKeyword.Split(' ').ToList();
            if (searchKeywords.Any(k => k.Length < 3))
                return BadRequest("The search keyword should be greater than 3 characters");

            try
            {
                var serviceLevel1Exists = await MasterDataRepository.Exists("service_level_1", serviceLevel1);
                if (!serviceLevel1Exists)
                    LogToElmah(new ArgumentException($"Invalid service level 1 value '{serviceLevel1}'."));

                var batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
                List<Service> services;
                if (string.IsNullOrEmpty(serviceLevel1))
                    services = await _serviceRepository.Search(searchKeywords, pageNumber, batchSize);
                else
                    services = await _serviceRepository.Search(searchKeywords, serviceLevel1, pageNumber, batchSize,
                        sortColumn, sortOrder);
                var servicesWithNewHeadersAndColumnsAndClassificationHeader = new List<Service>();
                foreach (var service in services)
                {
                    var serviceWithNewHeadersAndColumns =
                        await AddNewHeadersAndColumnsIfAny(service, service.ComponentDefinition);
                    //var serviceWithClassificationDefinitionHeader =
                    //    await AddClassificationDefinitionHeader(serviceWithNewHeadersAndColumns);
                    servicesWithNewHeadersAndColumnsAndClassificationHeader.Add(
                        serviceWithNewHeadersAndColumns);
                }
                var serviceSearchDtos =
                    servicesWithNewHeadersAndColumnsAndClassificationHeader.Select(x => new ServiceDto(x)).ToList();
                var count = await GetRecordCount(serviceLevel1, searchKeywords);

                var result = FormatSearchResponse(count, pageNumber, batchSize, serviceSearchDtos, sortColumn, sortOrder);

                return Ok(result);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Searches the in group.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get services by parameters")]
        [Route("group-search")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchInGroup(string searchQuery)
        {
            List<string> response;
            try
            {
                var keywords = FetchKeywords(searchQuery);
                response = await _serviceRepository.SearchInGroup(keywords);
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                LogToElmah(ex);
                return NotFound();
            }
            return Ok(response);
        }

        /// <summary>
        /// Updates the rates.
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        [ComponentLibraryAuthorize(Permissions = "Update Last Purchase rates for services")]
        [Route("{serviceCode}/rates")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateRates(string serviceCode, RateDto request)
        {
            try
            {
                request.IsValidState();
                var service = await _serviceRepository.Find(serviceCode);
                service.UpdateColumn("last purchase rate", ConvertToDictionary(request.LastPurchaseRate));
                service.UpdateColumn("wt. avg. purchase rate",
                    ConvertToDictionary(request.WeightedAveragePurchaseRate));
                await _serviceRepository.Update(service);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException)
            {
                Trace.TraceError($"Service not found with code {serviceCode}");
                return NotFound();
            }
            return Ok($"Updated rates of service with serviceCode: {serviceCode}.");
        }

        /// <summary>
        /// Get all rates
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Service Rate History")]
        [HttpPost]
        [Route("rates")]
        public async Task<IHttpActionResult> GetAllRates([FromBody] List<FilterData> filters)
        {
            try
            {
                var appliedOnFilter = filters.FirstOrDefault(filter => filter.ColumnKey == "AppliedOn");
                if (appliedOnFilter == null)
                {
                    return BadRequest("AppliedOn cannot be empty");
                }
                var rates = await _serviceRepository.GetAllRates(filters);

                return Ok(await Task.WhenAll(rates.Select(rate => new ServiceRateSearchResultDto().WithDomain(rate))));
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (NullReferenceException)
            {
                return BadRequest();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<IService> AddClassificationDefinitionHeader(IService serviceWithNewHeadersAndColumns)
        {

            var serviceClassificationHeader =
                serviceWithNewHeadersAndColumns.Headers.FirstOrDefault(header => header.Key == "classification");
            var column = serviceClassificationHeader?.Columns.FirstOrDefault(c => c.Key == "service_level_1");
            if (column != null)
            {
                var serviceLevel1 = Convert.ToString(column.Value);
                if (!string.IsNullOrWhiteSpace(serviceLevel1))
                {
                    var serviceClassificationDefinitionDao =
                        await _serviceClassificationDefinitionRepository.Find(serviceLevel1, Service);
                    if (serviceClassificationDefinitionDao != null)
                    {
                        var serviceClassificationDefinitionDomain = serviceClassificationDefinitionDao.Domain();
                        var serviceClassificationDefinitions = new List<ClassificationDefinition>
                        {
                            serviceClassificationDefinitionDomain
                        };

                        var classificationDefinitionHeaderColumns = new List<IColumnData>();
                        foreach (var columnData in serviceClassificationHeader.Columns)
                        {
                            var serviceClassificationDefinition =
                                serviceClassificationDefinitions?.FirstOrDefault(
                                    s =>
                                        s.Value.Equals(Convert.ToString(columnData.Value),
                                            StringComparison.InvariantCultureIgnoreCase));
                            var definition = serviceClassificationDefinition?.Description;
                            var classificationDefinitionColumn = new ColumnData(columnData.Name + " Definition", columnData.Key+ "_definition",
                                definition);
                            classificationDefinitionHeaderColumns.Add(classificationDefinitionColumn);
                            serviceClassificationDefinitions =
                                serviceClassificationDefinition?.ServiceClassificationDefinitions;
                        }
                        var classificationDefinitionHeader = new HeaderData("Classification Definition", "classification_definition")
                        {
                            Columns = classificationDefinitionHeaderColumns
                        };
                        var headers = serviceWithNewHeadersAndColumns.Headers.ToList();
                        headers.Add(classificationDefinitionHeader);
                        serviceWithNewHeadersAndColumns.Headers = headers;
                    }
                }
            }
            return serviceWithNewHeadersAndColumns;
        }

        private Dictionary<string, object> ConvertToDictionary(MoneyDto moneyDto)
        {
            return new Dictionary<string, object>
            {
                {"Amount", moneyDto.Amount.ToString(CultureInfo.InvariantCulture)},
                {"Currency", moneyDto.Currency}
            };
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <param name="serviceLevel1">The service level1.</param>
        /// <param name="searchKeywords">The search keywords.</param>
        /// <returns>Total records count in collection.</returns>
        private async Task<int> GetRecordCount(string serviceLevel1, List<string> searchKeywords)
        {
            var memoryCache = MemoryCache.Default;
            var keywords = string.Join("_", searchKeywords.ToArray());
            var key = $"{serviceLevel1}_{keywords}";
            if (memoryCache.Contains(key))
                return (int)memoryCache.Get(key);
            var count = await _serviceRepository.Count(searchKeywords, serviceLevel1);
            memoryCache.Add(key, count, DateTimeOffset.UtcNow.AddHours(1));
            return count;
        }
    }
}