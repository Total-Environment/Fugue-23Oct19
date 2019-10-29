using System;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <inheritdoc/>
	public class CompositeComponentBuilder : ICompositeComponentBuilder
	{
		private readonly ICounterGenerator _counterGenerator;
		private readonly IHeaderColumnDataValidator _headerColumnDataValidator;
		private readonly ICompositeComponentMappingRepository _compositeComponentMappingRepository;
		private readonly ICompositeComponentValidator _compositeComponentValidator;
		private readonly ICompositeComponentDataBuilder _compositeComponentDataBuilder;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentBuilder"/> class.
		/// </summary>
		/// <param name="compositeComponentMappingRepository">The SFG mapping repository.</param>
		/// <param name="counterGenerator">The counter generator.</param>
		/// <param name="headerColumnDataValidator">The header column data validator.</param>
		/// <param name="compositeComponentValidator">The semi finished good composition validator.</param>
		/// <param name="compositeComponentDataBuilder">
		/// The semi finished good composition data builder.
		/// </param>
		public CompositeComponentBuilder(ICompositeComponentMappingRepository compositeComponentMappingRepository,
			ICounterGenerator counterGenerator,
			IHeaderColumnDataValidator headerColumnDataValidator,
			ICompositeComponentValidator compositeComponentValidator,
			ICompositeComponentDataBuilder compositeComponentDataBuilder)
		{
			_compositeComponentMappingRepository = compositeComponentMappingRepository;
			_counterGenerator = counterGenerator;
			_headerColumnDataValidator = headerColumnDataValidator;
			_compositeComponentValidator = compositeComponentValidator;
			_compositeComponentDataBuilder = compositeComponentDataBuilder;
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> CloneFromService(IService service, ICompositeComponentDefinition sfgDefintion,
			IComponentComposition componentComposition)
		{
			var sfgMapping = await _compositeComponentMappingRepository.Get("sfg");
			var data = sfgMapping.MapServiceTodata(service, sfgDefintion);

			if (
				!componentComposition.ComponentCoefficients.Any(
					c => c.ComponentType == ComponentType.Service && c.Code == ((Service)service).Id))
				throw new ArgumentException($"fromService {((Service)service).Id} is not part of SFG composition data.");

			//return await Create("sfg", sfgDefintion,
			//	new CompositeComponent { Headers = data, ComponentComposition = componentComposition });

			var validSfgCompositionData = await _compositeComponentValidator.Validate("sfg", componentComposition);
			if (validSfgCompositionData.Item1 == false)
				throw new ArgumentException($"Invalid component composition data.{validSfgCompositionData.Item2}");

			var componentCompositionData =
				await _compositeComponentDataBuilder.BuildData(componentComposition);

			var sfg = new CompositeComponent
			{
				Headers = data,
				ComponentComposition = componentCompositionData,
				CompositeComponentDefinition = sfgDefintion
			};

			try
			{
				await SetCode("sfg", sfg);
			}
			catch (ArgumentException ae)
			{
				throw new ArgumentException(
					$"The Service you have selected as 'Primary' is not of a valid SFG group. Please choose choose a valid value (or) contact the Admin for help.");
			}
			return sfg;
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Create(string type, ICompositeComponentDefinition sfgDefintion,
			CompositeComponent sfgTobeCreated)
		{
			var validationResult = _headerColumnDataValidator.Validate(sfgDefintion.Headers, sfgTobeCreated.Headers);
			if (!validationResult.Item1)
				throw new ArgumentException($"Invalid header column data.{validationResult.Item2}");

			var validSfgCompositionData = await _compositeComponentValidator.Validate(type, sfgTobeCreated.ComponentComposition);
			if (validSfgCompositionData.Item1 == false)
				throw new ArgumentException($"Invalid component composition data.{validSfgCompositionData.Item2}");

			var headerColumnDataBuilder = new HeaderColumnDataBuilder();
			var headerColumnData = await headerColumnDataBuilder.BuildData(sfgDefintion.Headers,
				sfgTobeCreated.Headers);

			var componentCompositionData =
				await _compositeComponentDataBuilder.BuildData(sfgTobeCreated.ComponentComposition);

			var sfg = new CompositeComponent
			{
				Headers = headerColumnData,
				ComponentComposition = componentCompositionData,
				CompositeComponentDefinition = sfgDefintion
			};
			await SetCode(type, sfg);
			return sfg;
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Update(string type, ICompositeComponentDefinition compositeComponentDefinition,
			CompositeComponent compositeComponentTobeUpdated)
		{
			var validationResult = _headerColumnDataValidator.Validate(compositeComponentDefinition.Headers, compositeComponentTobeUpdated.Headers);
			if (!validationResult.Item1)
				throw new ArgumentException($"Invalid header column data.{validationResult.Item2}");

			var validSfgCompositionData = await _compositeComponentValidator.Validate(type, compositeComponentTobeUpdated.ComponentComposition);
			if (validSfgCompositionData.Item1 == false)
				throw new ArgumentException($"Invalid SFG composition data.{validSfgCompositionData.Item2}");

			var headerColumnDataBuilder = new HeaderColumnDataBuilder();
			var headerColumnData = await headerColumnDataBuilder.BuildData(compositeComponentDefinition.Headers,
				compositeComponentTobeUpdated.Headers);

			var componentCompositionData =
				await _compositeComponentDataBuilder.BuildData(compositeComponentTobeUpdated.ComponentComposition);

			var sfg = new CompositeComponent
			{
				Code = compositeComponentTobeUpdated.Code,
				Headers = headerColumnData,
				ComponentComposition = componentCompositionData,
				CompositeComponentDefinition = compositeComponentDefinition
			};

			// Setting up system logs
			if (compositeComponentTobeUpdated.Headers.Find(c => c.Key == "system_logs") != null)
			{
				((ColumnData)((dynamic)sfg).system_logs.date_created).Value =
					((ColumnData)((dynamic)compositeComponentTobeUpdated).system_logs.date_created).Value;

				((ColumnData)((dynamic)sfg).system_logs.created_by).Value =
					((ColumnData)((dynamic)compositeComponentTobeUpdated).system_logs.created_by).Value;
			}

			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
				((ColumnData)((dynamic)sfg).general.sfg_code).Value = compositeComponentTobeUpdated.Code;
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
				((ColumnData)((dynamic)sfg).general.package_code).Value = compositeComponentTobeUpdated.Code;
			else
				throw new NotSupportedException(type + " is not supported.");
			return sfg;
		}

		private async Task SetCode(string type, CompositeComponent compositeComponent)
		{
			var compositeComponentMapping = await _compositeComponentMappingRepository.Get(type);
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var sfgLevel1Value = ((ColumnData)((dynamic)compositeComponent).classification.sfg_level_1).Value.ToString();
				var sfgCode = compositeComponent.Code;
				string sfgPrefix;
				if (compositeComponentMapping.GroupCodeMapping.ContainsKey(sfgLevel1Value))
				{
					sfgPrefix = compositeComponentMapping.GroupCodeMapping[sfgLevel1Value];
				}
				else
				{
					throw new ArgumentException($"No code is not mapped to level1 value: {sfgLevel1Value}.");
				}
				var generatedSfgCode = await _counterGenerator.Generate(sfgPrefix, sfgCode, Keys.CounterCollections.SfgKey);
				compositeComponent.Code = generatedSfgCode;
				compositeComponent.Group = sfgLevel1Value;
				((ColumnData)((dynamic)compositeComponent).general.sfg_code).Value = generatedSfgCode;
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var packageLevel1Value = ((ColumnData)((dynamic)compositeComponent).classification.pkg_level_1).Value.ToString();
				var packageCode = compositeComponent.Code;
				string packagePrefix;
				if (compositeComponentMapping.GroupCodeMapping.ContainsKey(packageLevel1Value))
				{
					packagePrefix = compositeComponentMapping.GroupCodeMapping[packageLevel1Value];
				}
				else
				{
					throw new ArgumentException($"No code is not mapped to level1 value: {packageLevel1Value}.");
				}
				var generatedPackageCode = await _counterGenerator.Generate(packagePrefix, packageCode,
					Keys.CounterCollections.PackageKey);
				compositeComponent.Code = generatedPackageCode;
				compositeComponent.Group = packageLevel1Value;
				((ColumnData)((dynamic)compositeComponent).general.package_code).Value = generatedPackageCode;
			}
			else
				throw new NotSupportedException(type + " is not supported.");
		}
	}
}