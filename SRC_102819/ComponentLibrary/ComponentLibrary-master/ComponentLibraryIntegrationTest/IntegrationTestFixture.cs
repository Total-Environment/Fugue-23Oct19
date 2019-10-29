using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Money = TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
	public class Disposable : IDisposable
	{
		private readonly Action _action;

		public Disposable(Action action)
		{
			_action = action;
		}

		public void Dispose()
		{
			_action.Invoke();
		}
	}

	public class IntegrationTestFixture<T>
	{
		public readonly IWebClient<T> Client;

		public IntegrationTestFixture()
		{
			Server = IntegrationTestServer.GetInstance();
			DropDatabase();
			SeedDatabase(Server.MongoDatabase);
			Client = new RestClient<T>(Server.GetClient());
		}

		public IntegrationTestServer Server { get; }

		public void DropCollection(string collectionName)
		{
			var databaseName = Server.MongoDatabase.DatabaseNamespace.DatabaseName;
			var mongoClient = Server.MongoDatabase.Client;

			mongoClient.GetDatabase(databaseName).DropCollection(collectionName);
			MemoryCache.Default.Dispose();
		}

		public void DropDatabase()
		{
			var databaseName = Server.MongoDatabase.DatabaseNamespace.DatabaseName;
			var mongoClient = Server.MongoDatabase.Client;

			mongoClient.DropDatabase(databaseName);
		}

		public void ResetDatabase()
		{
			DropDatabase();
			SeedDatabase(Server.MongoDatabase);
		}

		public IDisposable SeedDependency()
		{
			Server.MongoDatabase.RunCommand(
				new JsonCommand<BsonDocument>(File.ReadAllText("SeedData/Dependency.json")));
			return new Disposable(() => { Server.MongoDatabase.DropCollection("dependencyDefinition"); });
		}

		private static void SeedBrandDefinition(IMongoDatabase mongoDatabase)
		{
			var brandDefinition = new BrandDefinition("Generic Brand", new List<ISimpleColumnDefinition>
			{
				new SimpleColumnDefinition("Brand Code", "Brand Code", new StringDataType()),
				new SimpleColumnDefinition("Manufacturer's Name", "Brand Code", new StringDataType()),
				new SimpleColumnDefinition("Series", "Brand Code", new StringDataType())
			});

			mongoDatabase.GetCollection<BrandDefinitionDao>("brandDefinitions")
				.InsertOne(new BrandDefinitionDao(brandDefinition));
		}

		private static void SeedCostPriceRatioDefinition(IMongoDatabase mongoDatabase)
		{
			var costPriceRatioDefinition = new CostPriceRatioDefinition("CostPriceRatioDefinition", new List<ISimpleColumnDefinition>
			{
				new SimpleColumnDefinition("Coefficient 1", "coefficient1", new UnitDataType("%")),
				new SimpleColumnDefinition("Coefficient 2", "coefficient2", new UnitDataType("%")),
				new SimpleColumnDefinition("Coefficient 3", "coefficient3", new UnitDataType("%")),
				new SimpleColumnDefinition("Coefficient 4", "coefficient4", new UnitDataType("%")),
				new SimpleColumnDefinition("Coefficient 5", "coefficient5", new UnitDataType("%")),
				new SimpleColumnDefinition("Coefficient 6", "coefficient6", new UnitDataType("%")),
				new SimpleColumnDefinition("Coefficient 7", "coefficient7", new UnitDataType("%")),
				new SimpleColumnDefinition("Profit Margin", "coefficient8", new UnitDataType("%")),
			});

			mongoDatabase.GetCollection<CostPriceRatioDefinitionDao>("costPriceRatioDefinitions")
				.InsertOne(new CostPriceRatioDefinitionDao(costPriceRatioDefinition));
		}

		private static void SeedDatabase(IMongoDatabase mongoDatabase)
		{
			SeedMasterDataListDao(mongoDatabase);
			SeedBrandDefinition(mongoDatabase);
			SeedCostPriceRatioDefinition(mongoDatabase);
			SeedMaterialDefinition(mongoDatabase);
			SeedServiceDefinition(mongoDatabase);
			SeedMaterialRate(mongoDatabase);
			SeedMaterial(mongoDatabase);
			SeedService(mongoDatabase);
			SeedServiceClassificationDefinition(mongoDatabase);
			SeedStaticFile(mongoDatabase);
			SeedCheckList(mongoDatabase);
			SeedSfgDefinition(mongoDatabase);
			SeedSfg(mongoDatabase);
			SeedCostPriceRatio(mongoDatabase);
		}

		private static void SeedCostPriceRatio(IMongoDatabase mongoDatabase)
		{
			// Material
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(5, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Material, "Primary", "AluminiumAndCopper", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(10, "%")) }),
							DateTime.Today, ComponentType.Material, "Primary", "AluminiumAndCopper", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(15, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Material, "Primary", "AluminiumAndCopper", "Aluminium", null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(20, "%")) }),
							DateTime.Today.AddDays(-1), ComponentType.Material, "Primary", "AluminiumAndCopper", "Aluminium", null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(25, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Material, "Primary", "AluminiumAndCopper", "Aluminium", "ALM000001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(30, "%")) }),
							DateTime.Today.AddDays(-2), ComponentType.Material, "Primary", "AluminiumAndCopper", "Aluminium", "ALM000001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(35, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Material, "Primary", "AluminiumAndCopper", "Aluminium", "ALM000001", "0053")));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(40, "%")) }),
							DateTime.Today.AddDays(-3), ComponentType.Material, "Primary", "AluminiumAndCopper", "Aluminium", "ALM000001", "0053")));
			// SemiFinishedGood
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(5, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", null, null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(10, "%")) }),
							DateTime.Today, ComponentType.SFG, "FLOORING|DADO|PAVIOUR", null, null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(15, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", "Flooring", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(20, "%")) }),
							DateTime.Today.AddDays(-1), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", "Flooring", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(25, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(30, "%")) }),
							DateTime.Today.AddDays(-2), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(35, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", "0053")));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(40, "%")) }),
							DateTime.Today.AddDays(-3), ComponentType.SFG, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", "0053")));
			// Service
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(5, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Service, "FLOORING|DADO|PAVIOUR", null, null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(10, "%")) }),
							DateTime.Today, ComponentType.Service, "FLOORING|DADO|PAVIOUR", null, null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(15, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Service, "FLOORING|DADO|PAVIOUR", "Flooring", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(20, "%")) }),
							DateTime.Today.AddDays(-1), ComponentType.Service, "FLOORING|DADO|PAVIOUR", "Flooring", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(25, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Service, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(30, "%")) }),
							DateTime.Today.AddDays(-2), ComponentType.Service, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(35, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Service, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", "0053")));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(40, "%")) }),
							DateTime.Today.AddDays(-3), ComponentType.Service, "FLOORING|DADO|PAVIOUR", "Flooring", null, "FLR000001", "0053")));
			// Package
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(5, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Package, "HVAC", null, null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(10, "%")) }),
							DateTime.Today, ComponentType.Package, "HVAC", null, null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(15, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Package, "HVAC", "VRV", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(20, "%")) }),
							DateTime.Today.AddDays(-1), ComponentType.Package, "HVAC", "VRV", null, null, null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(25, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Package, "HVAC", "VRV", null, "PHV0001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(30, "%")) }),
							DateTime.Today.AddDays(-2), ComponentType.Package, "HVAC", "VRV", null, "PHV0001", null)));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(35, "%")) }),
							DateTime.Today.AddDays(2), ComponentType.Package, "HVAC", "VRV", null, "PHV0001", "0053")));
			mongoDatabase.GetCollection<CostPriceRatioDao>("costPriceRatios")
				.InsertOne(
					new CostPriceRatioDao(
						new CostPriceRatio(
							new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(40, "%")) }),
							DateTime.Today.AddDays(-3), ComponentType.Package, "HVAC", "VRV", null, "PHV0001", "0053")));
		}

		private static void SeedSfg(IMongoDatabase mongoDatabase)
		{
			var componentCoefficients = new List<ComponentCoefficient>
			{
				new ComponentCoefficient("J001", 20,
					new List<WastagePercentage> {new WastagePercentage("Drivel", 40)}, "cm", "Aluminium Sheet",
					ComponentType.Material),
				new ComponentCoefficient("FDP0001", 12, new List<WastagePercentage> {new WastagePercentage("", 30)}, "m",
					"Masonry & Plaster", ComponentType.Service)
			};
			var sfg = new CompositeComponent
			{
				Code = "FLY400001",
				Group = "Semi Finished Good",
				Headers = new List<IHeaderData> { new HeaderData("General", "general")
				{
					Columns = new List<IColumnData>
					{
						new ColumnData("SFG Code", "sfg_code", "FLY400001")
					}},
					new HeaderData("Classifications", "classification")
					{
					   Columns = new List<IColumnData> {new ColumnData("SFG Level", "sfg_level_1", "Semi Finished Good")}
					}
				},
				ComponentComposition = new ComponentComposition(componentCoefficients)
			};

			mongoDatabase.GetCollection<SemiFinishedGoodDao>("semiFinishedGoods").InsertOne(new SemiFinishedGoodDao(sfg, null));
		}

		private static void SeedSfgDefinition(IMongoDatabase mongoDatabase)
		{
		    var exchangeRateRepository =
		        new ExchangeRateRepository(mongoDatabase.GetCollection<ExchangeRateDao>("exchangeRates"));
            mongoDatabase.GetCollection<SemiFinishedGoodDefinitionDao>("semiFinishedGoodDefinitions").InsertOne(new SemiFinishedGoodDefinitionDao
			{
				ObjectId = ObjectId.Parse("5822bf16d354cb493420ff63"),
				Name = "Generic SFG",
				Code = "FLY",
				Headers = new List<HeaderDefinitionDto>
				{
					new HeaderDefinitionDto {
						Name = "General",
						Key = "general",
						Columns = new List<ColumnDefinitionDto> {new ColumnDefinitionDto {
							Name = "SFG Code",
							Key = "sfg_code",
							DataType = new DataTypeDto { Name = "String"},
							IsSearchable = true}
						}
					},
					new HeaderDefinitionDto {
						Name = "Classification",
						Key = "classification",
						Columns = new List<ColumnDefinitionDto> {new ColumnDefinitionDto {
							Name = "SFG Level 1",
							Key = "sfg_level_1",
							DataType = new DataTypeDto { Name = "String"},
							IsSearchable = true}
						}
					}
				}
			});
		}

		private static void SeedMaterial(IMongoDatabase mongoDatabase)
		{
		    var exchangeRateRepository =
		        new ExchangeRateRepository(mongoDatabase.GetCollection<ExchangeRateDao>("exchangeRates"));
            mongoDatabase.GetCollection<MaterialDao>("materials").InsertOne(new MaterialDao
			{
				ObjectId = ObjectId.Parse("5822bf16d354cb493420ff63"),
				Columns = new Dictionary<string, object>
				{
					{"material_code", "MCLT0101"},
					{"material_name", "Clay Material"},
					{"material_level_1", "Primary"},
					{"material_level_2", "Clay Material"},
					{"material_level_3", "Brick"},
					{"group", "Clay Material"},
					{"date_created", DateTime.Today},
					{"created_by", "TE"},
					{"date_last_amended", DateTime.Today},
					{"last_amended_by", "TE"},
					{"SearchKeywords",new List<string>{}},
                    {"procurement_rate_threshold", "12" }
				}
			});

            var materialRateDao =
		        new MaterialRateDao(new MaterialRate(DateTime.Today, "Hyderabad", "J001",
		            new Money.Money(40m, "INR", new Money.Bank(exchangeRateRepository)), 10, 20, 0, 0, 10, 0, 0, "IMPORT"));

            var secondMaterialRateDao = new MaterialRateDao(new MaterialRate(DateTime.Today, "Hyderabad", "J001",
                    new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "DOMESTIC INTER-STATE"));

			mongoDatabase.GetCollection<MaterialDao>("materials").InsertOne(new MaterialDao
			{
				ObjectId = ObjectId.Parse("5822bf16d354cb493420ff63"),
				Columns = new Dictionary<string, object>
				{
					{"material_code", "J001"},
                    {"material_name", "Clay Material"},
                    {"material_level_1", "Primary"},
                    {"material_level_2", "Clay Material"},
                    {"material_level_3", "Brick"},
                    {"group", "Clay Material"},
                    {"date_created", DateTime.Today},
                    {"created_by", "TE"},
                    {"date_last_amended", DateTime.Today},
                    {"last_amended_by", "TE"},
                    {"SearchKeywords",new List<string>{}},
                    {"rates", new List<MaterialRateDao> {materialRateDao, secondMaterialRateDao}}
				}
			});

		
            var thirdMaterialRateDao = new MaterialRateDao(new MaterialRate(Convert.ToDateTime("2016-12-27"), "Banglore", "J002",
                    new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "DOMESTIC INTER-STATE"));

            var fourthMaterialRateDao = new MaterialRateDao(new MaterialRate(Convert.ToDateTime("2016-12-25"), "Hyderabad", "J002",
                    new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "DOMESTIC INTER-STATE"));

			mongoDatabase.GetCollection<MaterialDao>("materials").InsertOne(new MaterialDao
			{
				ObjectId = ObjectId.Parse("5822bf16d354cb493420ff63"),
				Columns = new Dictionary<string, object>
				{
					{"material_code", "J002"},
					{"material_name", "Clay Material" },
					{"rates", new List<MaterialRateDao> {thirdMaterialRateDao, fourthMaterialRateDao}}
				}
			});
		}

		private static void SeedCheckList(IMongoDatabase mongoDatabase)
		{
			mongoDatabase.GetCollection<CheckListDao>("checkLists").InsertOne(new CheckListDao()
			{
				CheckListId = "MGPO01",
				Name = "Something"
			});
		}

		private static void SeedMasterDataListDao(IMongoDatabase mongoDatabase)
		{
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff63"),
					Name = "Country"
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff64"),
					Name = "material_level_1",
					Values =
						new List<MasterDataValue> { new MasterDataValue("Primary"), new MasterDataValue("Secondary") }
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
			   .InsertOne(new MasterDataListDao
			   {
				   ObjectId = ObjectId.Parse("5822bf16d354cb293420af64"),
				   Name = "location",
				   Values =
					   new List<MasterDataValue> { new MasterDataValue("Hyderabad") }
			   });
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
			   .InsertOne(new MasterDataListDao
			   {
				   ObjectId = ObjectId.Parse("5822bf16d354cb293420af63"),
				   Name = "type_of_purchase",
				   Values =
					   new List<MasterDataValue> { new MasterDataValue("Domestic Interstate") }
			   });
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff62"),
					Name = "material_level_2",
					Values =
						new List<MasterDataValue>
						{
							new MasterDataValue("Clay Material"),
							new MasterDataValue("Fenestration")
						}
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff61"),
					Name = "service_level_1",
					Values =
						new List<MasterDataValue>
						{
							new MasterDataValue("TestService"),
							new MasterDataValue("FLOORING | DADO | PAVIOUR")
						}
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff70"),
					Name = "service_level_2",
					Values =
						new List<MasterDataValue> { new MasterDataValue("Flooring") }
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff71"),
					Name = "service_level_3",
					Values =
						new List<MasterDataValue> { new MasterDataValue("Natural Stone") }
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff72"),
					Name = "service_level_4",
					Values =
						new List<MasterDataValue> { new MasterDataValue("Kota Blue") }
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff99"),
					Name = "rental_unit",
					Values =
						new List<MasterDataValue> { new MasterDataValue("Daily"), new MasterDataValue("Monthly") }
				});
			mongoDatabase.GetCollection<MasterDataListDao>("masterDataLists")
				.InsertOne(new MasterDataListDao
				{
					ObjectId = ObjectId.Parse("5822bf16d354cb293420ff00"),
					Name = "status",
					Values =
						new List<MasterDataValue> { new MasterDataValue("Inactive"), new MasterDataValue("Approved") }
				});
		}

		private static void SeedMaterialDefinition(IMongoDatabase mongoDatabase)
		{
			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(
					new MaterialDefinitionDao(new MaterialDefinition("SattarMaterial")
					{
						Code = "SAT",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Material Level 1", "material_level_1",
											new StringDataType())
									}),
								new HeaderDefinition("General", "General", new List<IColumnDefinition>
								{
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Short Description", "short_description",
										new StringDataType()),
									new ColumnDefinition("Material Status", "material_status", new StringDataType())
								}),
                                new HeaderDefinition("Purchase", "purchase", new List<IColumnDefinition>
                                {
                                    new ColumnDefinition("Pur. Rate Threshold", "procurement_rate_threshold", new StringDataType()),
                                })
                            }
					}));

			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(
					new MaterialDefinitionDao(new MaterialDefinition("TestMaterial")
					{
						Code = "TST",
						Name = "Clay Material",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Material Level 1", "material_level_1",
											new StringDataType(), true),
										new ColumnDefinition("Material Level 2", "material_level_2",
											new StringDataType(), true)
									}),
								new HeaderDefinition("General", "general", new List<IColumnDefinition>
								{
									new ColumnDefinition("Material Name", "material_name", new StringDataType()),
									new ColumnDefinition("Material Code", "material_code", new StringDataType()),
									new ColumnDefinition("Short Description", "short_description",
										new StringDataType()),
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Image", "image", new StaticFileDataType()),
									new ColumnDefinition("Generic", "generic", new BooleanDataType()),
									new ColumnDefinition("Po Lead Time In Days", "po_lead_time_in_days",
										new IntDataType()),
									new ColumnDefinition("Last Purchase Rate", "last_purchase_rate",
										new MoneyDataType()),
									new ColumnDefinition("Length", "length", new UnitDataType("mm")),
									new ColumnDefinition("Approved Vendors", "approved_vendors",
										new ArrayDataType(new StringDataType())),
									new ColumnDefinition("Width", "width", new RangeDataType("mm")),
									new ColumnDefinition("Height", "height", new DecimalDataType()),
									new ColumnDefinition("Can be Used as an Asset", "can_be_used_as_an_asset",
										new BooleanDataType())
								}),
								new HeaderDefinition("Specifications", "specifications", new List<IColumnDefinition>
								{
									new ColumnDefinition("Application Area", "application_area", new StringDataType(),
										true)
								})
							}
					}));
			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(
					new MaterialDefinitionDao(new MaterialDefinition("TestMaterial")
					{
						Code = "FNS",
						Name = "Fenestration",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Material Level 1", "material_level_1",
											new StringDataType(), true),
										new ColumnDefinition("Material Level 2", "material_level_2",
											new StringDataType(), true)
									}),
								new HeaderDefinition("General", "general", new List<IColumnDefinition>
								{
									new ColumnDefinition("HSN Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Material Code", "material_code", new StringDataType()),
									new ColumnDefinition("Can be Used as an Asset", "can_be_used_as_an_asset",
										new BooleanDataType())
								}),
								new HeaderDefinition("Specifications", "specifications", new List<IColumnDefinition>
								{
									new ColumnDefinition("Fenestration Type", "fenestration_type", new StringDataType(),
										true),
									new ColumnDefinition("Length", "length", new UnitDataType("mm"))
								})
							}
					}));

			var counterRepository =
				new CounterRepository(
					new MongoRepository<CounterDao>(mongoDatabase.GetCollection<CounterDao>("counters")));
			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(new MaterialDefinitionDao(new MaterialDefinition("TestMaterial")
				{
					Code = "GNR",
					Name = "Generic Material",
					Headers =
						new List<IHeaderDefinition>
						{
							new HeaderDefinition("Classification", "classification",
								new List<IColumnDefinition>
								{
									new ColumnDefinition("Material Level 1", "material_level_1", new StringDataType()),
									new ColumnDefinition("Material Level 2", "material_level_2", new StringDataType())
								}),
							new HeaderDefinition("General", "general", new List<IColumnDefinition>
							{
								new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
								new ColumnDefinition("Material Name", "material_name", new StringDataType()),
								new ColumnDefinition("Short Description", "short_description", new StringDataType()),
								new ColumnDefinition("Material Status", "material_status", new StringDataType())
							}),
							new HeaderDefinition("Purchase", "purchase", new List<IColumnDefinition>()
							{
								new ColumnDefinition("Approved Brands", "approved_brands", new BrandDataType(
									new BrandDefinition("Generic Brand", new List<ISimpleColumnDefinition>()),
									"BSY",
									new BrandCodeGenerator(counterRepository),
									counterRepository
								))
							})
						}
				}));

			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(
					new MaterialDefinitionDao(new MaterialDefinition("Machines")
					{
						Code = "MCH",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Material Level 1", "material_level_1",
											new StringDataType(), true),
										new ColumnDefinition("Material Level 2", "material_level_2",
											new StringDataType(), true)
									}),
								new HeaderDefinition("General", "general", new List<IColumnDefinition>
								{
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Material Code", "material_code", new StringDataType()),
									new ColumnDefinition("Can be Used as an Asset", "can_be_used_as_an_asset",
										new BooleanDataType())
								}),
								new HeaderDefinition("Specifications", "specifications", new List<IColumnDefinition>
								{
									new ColumnDefinition("Application Area", "application_area", new StringDataType(),
										true)
								})
							}
					}));
			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(
					new MaterialDefinitionDao(new MaterialDefinition("Some Material")
					{
						Code = "SME",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Material Level 1", "material_level_1",
											new ConstantDataType("Primary"), true),
										new ColumnDefinition("Material Level 2", "material_level_2",
											new ConstantDataType("Some Material"), true)
									}),
								new HeaderDefinition("General", "general", new List<IColumnDefinition>
								{
									new ColumnDefinition("Material Name", "material_name", new StringDataType()),
									new ColumnDefinition("Short Description", "short_description",
										new StringDataType()),
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Material Code", "material_code", new StringDataType(), true),
									new ColumnDefinition("Image", "image", new ArrayDataType(new StaticFileDataType())),
								}),
								new HeaderDefinition("Specifications", "specifications", new List<IColumnDefinition>
								{
									new ColumnDefinition("Application Area", "application_area", new StringDataType(),
										true)
								})
							}
					}));
			mongoDatabase.GetCollection<MaterialDefinitionDao>("materialDefinitions")
				.InsertOne(
					new MaterialDefinitionDao(new MaterialDefinition("New Material")
					{
						Code = "SME",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Material Level 1", "material_level_1",
											new ConstantDataType("Primary"), true),
										new ColumnDefinition("Material Level 2", "material_level_2",
											new ConstantDataType("New Material"), true)
									}),
								new HeaderDefinition("General", "general", new List<IColumnDefinition>
								{
									new ColumnDefinition("Material Name", "material_name", new StringDataType()),
									new ColumnDefinition("Short Description", "short_description",
										new StringDataType()),
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Material Code", "material_code", new StringDataType()),
									new ColumnDefinition("Image", "image", new ArrayDataType(new StaticFileDataType())),
									new ColumnDefinition("general PO Terms and Conditions",
										"general_po_terms_conditions", new CheckListDataType()),
								}),
								new HeaderDefinition("Specifications", "specifications", new List<IColumnDefinition>
								{
									new ColumnDefinition("Application Area", "application_area", new StringDataType(),
										true)
								})
							}
					}));

			mongoDatabase.GetCollection<AssetDefinitionDao>("assetDefinitions")
				.InsertOne(
					new AssetDefinitionDao(new AssetDefinition("Machines")
					{
						Code = "MCH",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Maintenance", "maintenance",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Some Column", "some_column", new StringDataType(), true)
									})
							}
					}));
            
		    var exchangeRateRepository =
		        new ExchangeRateRepository(mongoDatabase.GetCollection<ExchangeRateDao>("exchangeRates"));
            var coefficients = new List<ICoefficient>
			{
				new SumCoefficient("Location variance", new Money.Money(10m, "INR", new Money.Bank(exchangeRateRepository)))
			};

            var materialRate = new MaterialRate(DateTime.Today, "Hyderabad", "J001",
                   new Money.Money(40m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "IMPORT");

           
			mongoDatabase.GetCollection<MaterialRateDao>("materialRates")
				.InsertOne(
					new MaterialRateDao(materialRate));
		}

		private static void SeedMaterialRate(IMongoDatabase mongoDatabase)
		{
		    var exchangeRateRepository =
		        new ExchangeRateRepository(mongoDatabase.GetCollection<ExchangeRateDao>("exchangeRates"));
            var materialRate = new MaterialRate(DateTime.Today, "Hyderabad", "J001",
                  new Money.Money(40m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "IMPORT");

            mongoDatabase.GetCollection<MaterialRateDao>("materialRates")
				.InsertOne(
					new MaterialRateDao(materialRate));

			var secondMaterialRate = new MaterialRate(DateTime.Today, "Hyderabad", "J001",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "DOMESTIC INTER-STATE");

           
			var thirdMaterialRate = new MaterialRate(Convert.ToDateTime("2016-12-27"), "Banglore", "J002",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "DOMESTIC INTER-STATE");

			var fourthMaterialRate = new MaterialRate(Convert.ToDateTime("2016-12-25"), "Hyderabad", "J002",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 10, 10, 5, 8, 0, 0, 0, "DOMESTIC INTER-STATE");
            
			mongoDatabase.GetCollection<MaterialRateDao>("materialRates").InsertMany(new List<MaterialRateDao>
			{
				new MaterialRateDao(thirdMaterialRate),
				
				new MaterialRateDao(secondMaterialRate),
				
				new MaterialRateDao(fourthMaterialRate),
			});
		}

		private static void SeedService(IMongoDatabase mongoDatabase)
		{
			var serviceDefinition = new ServiceDefinition("TestService");
			var headerData = new List<IHeaderData>
			{
				new HeaderData("Classification", "classification")
				{
					Columns = new List<IColumnData>
					{
						new ColumnData("Service Level 1", "service_level_1", "TestService"),
						new ColumnData("Service Level 2", "service_level_2", "TestAnotherService")
					}
				},
				new HeaderData("General", "general")
				{
					Columns = new List<IColumnData>
					{
						new ColumnData("HSM Code", "hsm_code", "TestService"),
						new ColumnData("Service Name", "service_name", "TestAnotherService"),
						new ColumnData("Image", "image", null),
						new ColumnData("Service Code", "service_code", ""),
						new ColumnData("Short Description", "short_description", "ABC"),
					}
				},
				new HeaderData("Planning", "planning")
				{
					Columns = new List<IColumnData>
					{
						new ColumnData("SO Lead Time in Days", "so_lead_time_in_days", 10),
						new ColumnData("Vendor Mobilisation Time in Days", "vendor_mobilisation_time_in_days", 10),
						new ColumnData("Minimum Order Quantity", "minimum_order_quantity", 10)
					}
				},
				new HeaderData("Quality", "quality")
				{
					Columns = new List<IColumnData>
					{
						new ColumnData("Governing Standard", "governing_standard", "10"),
						new ColumnData("Quality Checklist", "quality_checklist", null),
						new ColumnData("Safety Requirements", "safety_requirements", null)
					}
				},
				new HeaderData("Purchase", "purchase")
				{
					Columns = new List<IColumnData>
					{
						new ColumnData("Method of Measurement", "method_of_measurement", null),
						new ColumnData("General SO Terms & Conditions", "general_so_terms_and_conditions", null),
						new ColumnData("Special SO Terms & Conditions", "special_so_terms_and_conditions", null),
						new ColumnData("Last Purchase Rate", "last_purchase_rate", null),
						new ColumnData("Weighted Average Purchase Rate", "weighted_average_purchase_rate", null),
						new ColumnData("Procurement Rate Threshold %", "procurement_rate_threshold_%", null),
						new ColumnData("Approved Vendors", "approved_vendors", null)
					}
				}
			};
            
		    var exchangeRateRepository =
		        new ExchangeRateRepository(mongoDatabase.GetCollection<ExchangeRateDao>("exchangeRates"));

            var j001 = new Service(headerData, serviceDefinition) { Id = "J001" };
			var j001Dao = new ServiceDao(j001);

			var fdp0001 = new Service(headerData, serviceDefinition) { Id = "FDP0001" };
			var fdp0001Dao = new ServiceDao(fdp0001);

			var mclt0001 = new Service(headerData, serviceDefinition) { Id = "MCLT0001" };
			var mclt0001Dao = new ServiceDao(mclt0001);

			SeedRatesForJ001(j001Dao, exchangeRateRepository);
			SeedRatesForFDP0001(fdp0001Dao, exchangeRateRepository);

			mongoDatabase.GetCollection<ServiceDao>("services")
				.InsertMany(new List<ServiceDao> { j001Dao, fdp0001Dao, mclt0001Dao });
		}

		private static void SeedServiceClassificationDefinition(IMongoDatabase mongoDatabase)
		{
			var serviceClassificationDefinition = new ClassificationDefinition("TestService",
				"TestService - description", null);
			mongoDatabase.GetCollection<ClassificationDefinitionDao>("serviceClassificatoinDefinition")
				.InsertOne(
					new ClassificationDefinitionDao(serviceClassificationDefinition)
				);
		}

		private static void SeedServiceDefinition(IMongoDatabase mongoDatabase)
		{
			mongoDatabase.GetCollection<ServiceDefinitionDao>("serviceDefinitions")
				.InsertOne(
					new ServiceDefinitionDao(new ServiceDefinition("Test Service")
					{
						Code = "SAT",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "Classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Service Level 1", "service_level_1",
											new StringDataType()),
										new ColumnDefinition("Service Level 2", "service_level_2", new StringDataType())
									}),
								new HeaderDefinition("General", "General", new List<IColumnDefinition>
								{
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Short Description", "short_description",
										new StringDataType()),
									new ColumnDefinition("Service Status", "service_status", new StringDataType()),
									new ColumnDefinition("Service Name", "service_name", new StringDataType()),
									new ColumnDefinition("Image", "image", new StaticFileDataType()),
									new ColumnDefinition("Service Code", "service_code", new StringDataType())
								})
							}
					}));

			mongoDatabase.GetCollection<ServiceDefinitionDao>("serviceDefinitions")
				.InsertOne(
					new ServiceDefinitionDao(new ServiceDefinition("TestService")
					{
						Code = "TST",
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("Classification", "Classification",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("Service Level 1", "service_level_1", new StringDataType(),
											true),
										new ColumnDefinition("Service Level 2", "service_level_2", new StringDataType(),
											true)
									}),
								new HeaderDefinition("General", "General", new List<IColumnDefinition>
								{
									new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
									new ColumnDefinition("Service Name", "service_name", new StringDataType()),

									new ColumnDefinition("Short Description", "short_description",
										new StringDataType()),
									new ColumnDefinition("Image", "image", new StaticFileDataType()),

									new ColumnDefinition("Service Code", "service_code", new StringDataType())
								}),
								new HeaderDefinition("Planning", "Planning", new List<IColumnDefinition>
								{
									new ColumnDefinition("SO Lead Time in Days", "so_lead_time_in_days",
										new IntDataType()),
									new ColumnDefinition("Vendor Mobilisation Time in Days",
										"vendor_mobilisation_time_in_days", new IntDataType()),
									new ColumnDefinition("Minimum Order Quantity", "minimum_order_quantity",
										new IntDataType())
								}),
								new HeaderDefinition("Quality", "Quality", new List<IColumnDefinition>
								{
									new ColumnDefinition("Governing Standard", "governing_standard",
										new StringDataType()),
									new ColumnDefinition("Quality Checklist", "quality_checklist",
										new CheckListDataType(
											new CheckListRepository(new TestMongoRepository<CheckListDao>()))),
									new ColumnDefinition("Safety Requirements", "safety_requirements",
										new StaticFileDataType(
											new StaticFileRepository(new TestMongoRepository<StaticFileDao>())))
								}),
								new HeaderDefinition("Purchase", "Purchase", new List<IColumnDefinition>
								{
									new ColumnDefinition("Method of Measurement", "method_of_measurement",
										new StaticFileDataType(
											new StaticFileRepository(new TestMongoRepository<StaticFileDao>()))),
									new ColumnDefinition("General SO Terms & Conditions",
										"general_so_terms_and_conditions",
										new CheckListDataType(
											new CheckListRepository(new TestMongoRepository<CheckListDao>()))),
									new ColumnDefinition("Special SO Terms & Conditions",
										"special_so_terms_and_conditions",
										new CheckListDataType(
											new CheckListRepository(new TestMongoRepository<CheckListDao>()))),
									new ColumnDefinition("Last Purchase Rate", "last_purchase_rate",
										new MoneyDataType()),
									new ColumnDefinition("Weighted Average Purchase Rate",
										"weighted_average_purchase_rate", new MoneyDataType()),
									new ColumnDefinition("Procurement Rate Threshold %", "procurement_rate_threshold_%",
										new UnitDataType("%")),
									new ColumnDefinition("Approved Vendors", "approved_vendors",
										new ArrayDataType(new StringDataType()))
								})
							}
					}));

			mongoDatabase.GetCollection<ServiceDefinitionDao>("serviceDefinitions")
				.InsertOne(new ServiceDefinitionDao(new ServiceDefinition("Generic Service")
				{
					Code = "GNR",
					//Name = "Generic Service",
					Headers =
						new List<IHeaderDefinition>
						{
							new HeaderDefinition("Classification", "Classification",
								new List<IColumnDefinition>
								{
									new ColumnDefinition("Service Level 1", "service_level_1", new StringDataType()),
									new ColumnDefinition("Service Level 2", "service_level_2", new StringDataType())
								}),
							new HeaderDefinition("General", "General", new List<IColumnDefinition>
							{
								new ColumnDefinition("HSM Code", "hsm_code", new StringDataType()),
								new ColumnDefinition("Short Description", "short_description", new StringDataType()),
								new ColumnDefinition("Service Status", "service_status", new StringDataType()),

								new ColumnDefinition("Short Description", "short_description", new StringDataType()),

								new ColumnDefinition("Image", "image", new StaticFileDataType()),
								new ColumnDefinition("Service Code", "service_code", new StringDataType())
							}),
							new HeaderDefinition("Planning", "Planning", new List<IColumnDefinition>
							{
								new ColumnDefinition("SO Lead Time in Days", "so_lead_time_in_days", new IntDataType()),
								new ColumnDefinition("Vendor Mobilisation Time in Days",
									"vendor_mobilisation_time_in_days", new IntDataType()),
								new ColumnDefinition("Minimum Order Quantity", "minimum_order_quantity",
									new IntDataType())
							}),
							new HeaderDefinition("Quality", "Quality", new List<IColumnDefinition>
							{
								new ColumnDefinition("Governing Standard", "governing_standard", new StringDataType()),
								new ColumnDefinition("Quality Checklist", "quality_checklist",
									new CheckListDataType(
										new CheckListRepository(new TestMongoRepository<CheckListDao>()))),
								new ColumnDefinition("Safety Requirements", "safety_requirements",
									new StaticFileDataType(
										new StaticFileRepository(new TestMongoRepository<StaticFileDao>())))
							}),
							new HeaderDefinition("Purchase", "Purchase", new List<IColumnDefinition>
							{
								new ColumnDefinition("Method of Measurement", "method_of_measurement",
									new StaticFileDataType(
										new StaticFileRepository(new TestMongoRepository<StaticFileDao>()))),
								new ColumnDefinition("General SO Terms & Conditions", "general_so_terms_and_conditions",
									new CheckListDataType(
										new CheckListRepository(new TestMongoRepository<CheckListDao>()))),
								new ColumnDefinition("Special SO Terms & Conditions", "special_so_terms_and_Conditions",
									new CheckListDataType(
										new CheckListRepository(new TestMongoRepository<CheckListDao>()))),
								new ColumnDefinition("Last Purchase Rate", "last_purchase_rate", new MoneyDataType()),
								new ColumnDefinition("Weighted Average Purchase Rate", "weighted_average_purchase_rate",
									new MoneyDataType()),
								new ColumnDefinition("Procurement Rate Threshold %", "procurement_rate_threshold_%",
									new UnitDataType("%")),
								new ColumnDefinition("Approved Vendors", "approved_vendors",
									new ArrayDataType(new StringDataType()))
							})
						}
				}));
		}

		private static List<ICoefficient> GetCoefficients()
		{
			return new List<ICoefficient>
			{
				new PercentageCoefficient("Location variance", 40)
			};
		}

		private static void SeedRatesForFDP0001(ServiceDao service, ExchangeRateRepository exchangeRateRepository)
		{
            var thirdServiceRate = new ServiceRate(DateTime.Today, "Hyderabad", "FDP0001",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 40, 0, "DOMESTIC INTRA-STATE");

			var fourthServiceRate = new ServiceRate(Convert.ToDateTime("2016-02-12"), "Banglore", "FDP0001",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 40, 0, "DOMESTIC INTER-STATE");

			var fifthServiceRate = new ServiceRate(Convert.ToDateTime("2016-02-13"), "Hyderabad", "FDP0001",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 40, 0, "IMPORT");

            var rates = new List<ServiceRateDao>
			{
				new ServiceRateDao(thirdServiceRate),

				new ServiceRateDao(fourthServiceRate),

				new ServiceRateDao(fifthServiceRate)
			};

			rates.ForEach(r => service.AddRate(r));
		}

		private static void SeedRatesForJ001(ServiceDao service, IExchangeRateRepository exchangeRateRepository)
		{
			IEnumerable<ICoefficient> finalCoefficients = GetCoefficients();

			var serviceRate = new ServiceRate(DateTime.Today, "Hyderabad", "J001",
                  new Money.Money(40m, "INR", new Money.Bank(exchangeRateRepository)), 40, 0, "IMPORT");
            
			var secondServiceRate = new ServiceRate(DateTime.Today, "Hyderabad", "J001",
                  new Money.Money(50m, "INR", new Money.Bank(exchangeRateRepository)), 40, 0, "DOMESTIC INTER-STATE");

			var rates = new List<ServiceRateDao>
			{
				new ServiceRateDao(serviceRate),
				new ServiceRateDao(secondServiceRate)
			};
			rates.ForEach(r => service.AddRate(r));
		}

		private static void SeedStaticFile(IMongoDatabase mongoDatabase)
		{
			mongoDatabase.GetCollection<StaticFileDao>("staticFile")
				.InsertOne(new StaticFileDao(new StaticFile("id", "Test.png")));
		}
	}

	internal class TestMongoRepository<T> : IMongoRepository<T> where T : Entity
	{
		public Type ElementType { get; }

		public Expression Expression { get; }

		public IQueryProvider Provider { get; }

		public Task<T> Add(T entity)
		{
			throw new NotImplementedException();
		}

		public Task Delete(string id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<T>> FindAllBy(Expression<Func<T, bool>> f)
		{
			throw new NotImplementedException();
		}

		public Task<T> FindBy(string columnName, object value)
		{
			throw new NotImplementedException();
		}

		public Task<T> FindBy(Expression<Func<T, bool>> f)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetById(string id)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Task<T> Increment(T entity, string value)
		{
			throw new NotImplementedException();
		}

		public Task<T> Update(T entity)
		{
			throw new NotImplementedException();
		}
	}
}