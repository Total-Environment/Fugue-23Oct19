using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class MaterialCostPriceRatioValidatorTests
	{
		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCodeIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(0);
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, null, null, null, "xyz", null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("xyz is not valid. Please enter a valid material code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidLevel1IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("materialClassifications", new List<string> { "Material Level 1" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary" }) }));
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary1", "A&C", "A", null, null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid material level1 value.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidLevel2IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("materialClassifications", new List<string> { "Material Level 1", "Material Level 2" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C" }) }));
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C1", "A", null, null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1 and level2 values.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidLevel3IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("materialClassifications", new List<string> { "Material Level 1", "Material Level 2", "Material Level 3" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C", "A" }) }));
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", "A1", null, null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1, level2 and level3 values.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenOnlyLevel1IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", null, null, null, null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("CPR cannot be defined only at material level1.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination1IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", null, "ALM0001", null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1, level2, level3, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination2IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", null, "ALM0001", "POARR");

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1, level2, level3, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination3IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", "A", "ALM0001", null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1, level2, level3, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination4IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", "A", "ALM0001", "POARR");

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1, level2, level3, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination5IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, null, null, null, null, "POARR");

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of material level1, level2, level3, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidProjectCodeIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			mockProjectDataRepository.Setup(m => m.Find("0044")).Throws(new ResourceNotFoundException("0044"));
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, null, null, null, "ALM0001", "0044");

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("0044 is not valid. Please enter a valid project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenValidProjectCodeIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			mockProjectDataRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(new Project("0043", "Windmills of Your Mind", "WoYM"));
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, null, null, null, "ALM0001", "0043");

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination1IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("materialClassifications", new List<string> { "Material Level 1", "Material Level 2", "Material Level 3" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C", "A" }) }));
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", "A", null, null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination2IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("materialClassifications", new List<string> { "Material Level 1", "Material Level 2", "Material Level 3" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C", "A" }) }));
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, "Primary", "A&C", null, null, null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination3IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Material, null, null, null, "ALM0001", null);

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination4IsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			mockProjectDataRepository.Setup(m => m.Find(It.IsAny<string>()))
				.ReturnsAsync(new Project("0043", "Windmills of Your Mind", "WoYM"));
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository =
				new Mock<IDependencyDefinitionRepository>();
			MaterialCostPriceRatioValidator materialCostPriceRatioValidator =
				new MaterialCostPriceRatioValidator(mockMaterialRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today.AddDays(1),
				ComponentType.Material, null, null, null, "ALM0001", "0043");

			var result = await materialCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}
	}
}