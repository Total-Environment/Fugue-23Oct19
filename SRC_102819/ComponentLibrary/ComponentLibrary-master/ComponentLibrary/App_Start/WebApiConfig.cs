using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using Elmah.Contrib.WebApi;
using log4net;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NotificationEngine;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.ExceptionHandling;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.SAPServiceReference;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ServiceSAPServiceReference;
using TE.Shared.CloudServiceFramework.Domain;
using TE.Shared.CloudServiceFramework.Infrastructure;


namespace TE.ComponentLibrary.ComponentLibrary
{
	/// <summary>
	/// Represents the configuration of the server.
	/// </summary>
	public static class WebApiConfig
	{
		/// <summary>
		/// Registers the specified configuration.
		/// </summary>
		/// <param name="config">The configuration.</param>
		public static void Register(HttpConfiguration config)
		{
			EnableDependencyInjection(config);
			ConfigureHttpConfiguration(config);
			config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
			BsonClassMap.RegisterClassMap<UnitValue>();
			BsonClassMap.RegisterClassMap<MoneyValue>();
			BsonClassMap.RegisterClassMap<Money>();
			BsonClassMap.RegisterClassMap<RangeValue>();
			BsonClassMap.RegisterClassMap<CheckListValue>();
			BsonClassMap.RegisterClassMap<StaticFile>();
			BsonClassMap.RegisterClassMap<ComponentCompositionDao>();
			BsonSerializer.RegisterSerializer(typeof(decimal), new MongoDBDecimalFieldSerializer());
			BsonSerializer.RegisterSerializer(typeof(Brand), new MongoDBBrandSerializer());
			BsonSerializer.RegisterSerializer(typeof(CPRCoefficient), new MongoDBCPRCoefficientSerializer());
		}

		private static void ConfigureHttpConfiguration(HttpConfiguration config)
		{
			var constraintResolver = new DefaultInlineConstraintResolver();
			constraintResolver.ConstraintMap.Add("validcompositecomponent", typeof(ValidCompositeComponentConstraint));
			config.MapHttpAttributeRoutes(constraintResolver);
			config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
			config.Services.Replace(typeof(IExceptionHandler), new OopsExceptionHandler());
			// config.Filters.Add(new ValidateModelAttribute());
#if !DEBUG
            config.Filters.Add(new RequireHttpsAttribute());
#endif
#if DEBUG
			var cors = new EnableCorsAttribute("http://localhost:8080", "*", "*");
			config.EnableCors(cors);
#endif
			var jsonFormatter = config.Formatters.JsonFormatter;
			jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
			jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
			jsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
			jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
			jsonFormatter.SerializerSettings.Converters.Add(new StaticFileConverter());
			jsonFormatter.SerializerSettings.Converters.Add(new ChecklistValueConverter());
			jsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
			jsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Include;
			jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			// jsonFormatter.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
		}

		private static void EnableDependencyInjection(HttpConfiguration config)
		{
			var container = new Container();
			container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

			var mongoDatabase = GetMongoDatabase("MongoServerSettings");
			container.RegisterSingleton(mongoDatabase);
			container.Register(() => mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<CheckListDao>("checkLists"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<CounterDao>("counters"), Lifestyle.Singleton);
			container.Register(() => mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<ServiceDefinitionDao>("serviceDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<StaticFileDao>("staticFile"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<MaterialDao>("materials"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<ServiceDao>("services"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<ClassificationDefinitionDao>("classificationDefinition"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<AssetDefinitionDao>("assetDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<RentalRateDao>("rentalRates"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<BrandDefinitionDao>("brandDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<SemiFinishedGoodDao>("semiFinishedGoods"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<SemiFinishedGoodMappingDao>("semiFinishedGoodMappings"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<SemiFinishedGoodDefinitionDao>("semiFinishedGoodDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<PackageDao>("packages"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<PackageMappingDao>("packageMappings"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<PackageDefinitionDao>("packageDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<CostPriceRatioDefinitionDao>("costPriceRatioDefinitions"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<ProjectDao>("projects"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<CodePrefixTypeMappingDao>("codePrefixTypeMappings"), Lifestyle.Scoped);

			container.Register<IMongoRepository<MasterDataListDao>, MongoRepository<MasterDataListDao>>(Lifestyle.Scoped);
			container.Register<IMongoRepository<CheckListDao>, MongoRepository<CheckListDao>>(Lifestyle.Scoped);
			container.Register<IMongoRepository<CounterDao>, MongoRepository<CounterDao>>(Lifestyle.Singleton);
			container.Register<IMongoRepository<StaticFileDao>, MongoRepository<StaticFileDao>>(Lifestyle.Scoped);
			container.Register<IMasterDataRepository, MasterDataRepository>(Lifestyle.Scoped);
			container.Register<ICheckListRepository, CheckListRepository>(Lifestyle.Scoped);
			container.Register<ICounterRepository, CounterRepository>(Lifestyle.Singleton);
			container.Register<IBrandDefinitionRepository, BrandDefinitionRepository>(Lifestyle.Scoped);
			container.Register<IComponentDefinitionRepository<IMaterialDefinition>, MaterialDefinitionRepository>(Lifestyle.Scoped);
			container.Register<IComponentDefinitionRepository<IServiceDefinition>, ServiceDefinitionRepository>(Lifestyle.Scoped);
			container.Register<IMaterialRepository, MaterialRepository>(Lifestyle.Scoped);
			container.Register<IServiceRepository, ServiceRepository>(Lifestyle.Scoped);
            container.Register<IComponentRepository, ComponentRepository>(Lifestyle.Scoped);
			DependenciesForRateMaster(container, mongoDatabase);
			DependenciesForDependencyDefinition(container, mongoDatabase);
			container.Register<IStaticFileRepository, StaticFileRepository>(Lifestyle.Scoped);
			container.Register<IBlobDownloadService>(() => new AzureBlobDownloadService(Environment.GetEnvironmentVariable("StaticFileAzureConnectionString")));
			container.RegisterSingleton<ISapSyncer, SapSyncer>();
			container.Register<IServiceSapSyncer, ServiceSapSyncer>(Lifestyle.Singleton);
			container.Register<IServiceAndCompositeComponentSapSyncer, ServiceAndCompositeComponentSapSyncer>(Lifestyle.Singleton);
			container.Register<ICompositeComponentSapSyncer, CompositeComponentSapSyncer>(Lifestyle.Singleton);
			container.Register<IDataTypeFactory, DataTypeFactory>(Lifestyle.Scoped);
			container.Register<ISimpleDataTypeFactory, SimpleDataTypeFactory>(Lifestyle.Scoped);
			container.Register<IClassificationDefinitionRepository, ClassificationDefinitionRepository>(Lifestyle.Scoped);
			container.Register<IComponentDefinitionRepository<AssetDefinition>, AssetDefinitionRepository>(Lifestyle.Scoped);
			container.Register<IMongoRepository<RentalRateDao>, MongoRepository<RentalRateDao>>(Lifestyle.Scoped);
			container.Register<IRentalRateRepository, RentalRateRepository>(Lifestyle.Scoped);
			container.RegisterWebApiControllers(config);
			container.Register<IDocumentRepository, DocumentRepository>(Lifestyle.Scoped);
			container.Register<IBlobStorageService>(() => new AzureBlobStorageService(ConfigurationManager.AppSettings["StaticFileAzureConnectionString"]), Lifestyle.Scoped);
			container.Register<IMaterialService, MaterialService>(Lifestyle.Scoped);
			container.Register<IMaterialCodeGenerator, MaterialCodeGenerator>(Lifestyle.Scoped);
			container.Register<ICounterGenerator, CounterGenerator>(Lifestyle.Scoped);
			container.Register<IMaterialBuilder, MaterialBuilder>(Lifestyle.Scoped);
			container.Register<IFilterCriteriaBuilder, FilterCriteriaBuilder>(Lifestyle.Scoped);
			container.Register<IDependencyValidator, DependencyValidator>(Lifestyle.Scoped);
			container.Register<IHeaderColumnDataValidator, HeaderColumnDataValidator>(Lifestyle.Scoped);
			container.Register<IBrandCodeGenerator, BrandCodeGenerator>(Lifestyle.Scoped);

			container.Register<ICompositeComponentService, CompositeComponentService>(Lifestyle.Scoped);
			container.Register<ICompositeComponentBuilder, CompositeComponentBuilder>(Lifestyle.Scoped);
			container.Register<ICompositeComponentRepository, CompositeComponentRepository>(Lifestyle.Scoped);
			container.Register<ICompositeComponentMappingRepository, CompositeComponentMappingRepository>(Lifestyle.Scoped);
			container.Register<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>, CompositeComponentDefinitionRepository>(Lifestyle.Scoped);

			container.Register<IClassificationDefinitionBuilder, ClassificationDefinitionBuilder>(Lifestyle.Scoped);

			container.Register<ICompositeComponentValidator, CompositeComponentValidator>(Lifestyle.Scoped);
			container.Register<IComponentCoefficientValidatorFactory, ComponentCoefficientValidatorFactory>(Lifestyle.Scoped);
			container.Register<ICompositeComponentDataBuilder, CompositeComponentDataBuilder>(Lifestyle.Scoped);
			container.Register<IComponentCoefficientBuilderFactory, ComponentCoefficientBuilderFactory>(Lifestyle.Scoped);

			container.Register<ICostPriceRatioDefinitionRepository, CostPriceRatioDefinitionRepository>(Lifestyle.Scoped);
			container.Register<ICostPriceRatioService, CostPriceRatioService>(Lifestyle.Scoped);
			container.Register<ICostPriceRatioRepository, CostPriceRatioRepository>(Lifestyle.Scoped);
			container.Register<ICostPriceRatioValidatorFactory, CostPriceRatioValidatorFactory>(Lifestyle.Scoped);
			container.Register<ICostPriceRatioBuilderFactory, CostPriceRatioBuilderFactory>(Lifestyle.Scoped);
			container.Register<IColumnDataValidator, ColumnDataValidator>(Lifestyle.Scoped);
			container.Register<IColumnDataBuilder, ColumnDataBuilder>(Lifestyle.Scoped);
			container.Register<ICostPriceRatioFilterFactory, CostPriceRatioFilterFactory>(Lifestyle.Scoped);

            container.Register<IValidationService, ValidationService>(Lifestyle.Scoped);

			container.Register<IProjectRepository, ProjectRepository>(Lifestyle.Scoped);
			container.Register<IPriceService, PriceService>(Lifestyle.Scoped);

			container.Register<ICodePrefixTypeMappingRepository, CodePrefixTypeMappingRepository>(Lifestyle.Scoped);
            container.RegisterSingleton<INotifier,Notifier>();
		    container.RegisterSingleton<MaterialMaster_OutService>();
		    container.RegisterSingleton<ServiceMaster_OutService>();
            container.Register<IBlobService>(
                () => new AzureService(ConfigurationManager.AppSettings["StaticFileAzureConnectionString"]));


            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
		}

		private static void DependenciesForDependencyDefinition(Container container, IMongoDatabase mongoDatabase)
		{
			container.Register(() => mongoDatabase.GetCollection<DependencyDefinitionDao>("dependencyDefinition"), Lifestyle.Scoped);
			container.Register<IDependencyDefinitionRepository, DependencyDefinitionRepository>(Lifestyle.Scoped);
		}

		private static void DependenciesForRateMaster(Container container, IMongoDatabase mongoDatabase)
		{
			container.Register(() => mongoDatabase.GetCollection<ExchangeRateDao>("exchangeRates"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<MaterialRateDao>("materialRates"), Lifestyle.Scoped);
			container.Register(() => mongoDatabase.GetCollection<ServiceRateDao>("serviceRates"), Lifestyle.Scoped);
			container.Register<IMaterialRateService, MaterialRateService>(Lifestyle.Scoped);
			container.Register<IMaterialRateRepository, MaterialRateRepository>(Lifestyle.Scoped);
			container.Register<IMongoRepository<MaterialRateDao>, MongoRepository<MaterialRateDao>>(Lifestyle.Scoped);
			container.Register<IServiceRateService, ServiceRateService>(Lifestyle.Scoped);
			container.Register<IServiceRateRepository, ServiceRateRepository>(Lifestyle.Scoped);
			container.Register<IMongoRepository<ServiceRateDao>, MongoRepository<ServiceRateDao>>(Lifestyle.Scoped);
			container.Register<IMongoRepository<ExchangeRateDao>, MongoRepository<ExchangeRateDao>>(Lifestyle.Scoped);
			container.Register<IExchangeRateRepository, ExchangeRateRepository>(Lifestyle.Scoped);
            container.Register<IBank, Bank>(Lifestyle.Scoped);
		}

		private static IMongoDatabase GetMongoDatabase(string connectionStringKey)
		{
			var url = new MongoUrl(ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString);
			MongoClient mongoClient;
			if (url.AuthenticationSource == null)
			{
				mongoClient = new MongoClient(url);
			}
			else
			{
				var settings = new MongoClientSettings
				{
					Server = url.Server,
					UseSsl = true,
					SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 }
				};

				var identity = new MongoInternalIdentity(url.AuthenticationSource, url.Username);
				var evidence =
					new PasswordEvidence(url.Password);

				settings.Credentials = new List<MongoCredential>
				{
					new MongoCredential("MONGODB-CR", identity, evidence)
				};

				settings.ConnectTimeout = TimeSpan.FromSeconds(30);
				settings.SocketTimeout = TimeSpan.Zero;
				mongoClient = new MongoClient(settings);
			}

			var mongoDatabase = mongoClient.GetDatabase(url.DatabaseName);
			return mongoDatabase;
		}
	}
}