using System;
using System.Collections.Generic;
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
	public class PackageCostPriceRatioValidatorTests
	{
		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCodeIsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			mockPackageRepository.Setup(m => m.Count("package", It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(0);
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, null, "xyz", null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("xyz is not valid. Please enter a valid package code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidLevel1IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("packageClassifications", new List<string> { "Package Level 1" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary" }) }));
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary1", "A&C", null, null, null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid package level1 value.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidLevel2IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("packageClassifications", new List<string> { "Package Level 1", "Package Level 2" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C" }) }));
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary", "A&C1", null, null, null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1 and level2 values.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenOnlyLevel3IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary", "A&C", "A", null, null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("CPR cannot be defined at package level3.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination1IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary", "A&C", null, "ALM0001", null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination2IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary", "A&C", null, "ALM0001", "POARR");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination3IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, "A&C", "A", "ALM0001", null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination4IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, "A&C", "A", "ALM0001", "POARR");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination5IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, null, null, "POARR");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination6IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectDataRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, "A", "ALM0001", "POARR");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidCombination7IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, "A", null, null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid combination of package level1, level2, code and project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenInvalidProjectCodeIsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			mockPackageRepository.Setup(m => m.Count("package", It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Find("0044")).Throws(new ResourceNotFoundException("0044"));
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, null, "ALM0001", "0044");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("0044 is not valid. Please enter a valid project code.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenValidProjectCodeIsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			mockPackageRepository.Setup(m => m.Count("package", It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Find(It.IsAny<string>()))
				.ReturnsAsync(new Project("0043", "Windmills of Your Mind", "WoYM"));
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, null, "ALM0001", "0043");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination1IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("packageClassifications", new List<string> { "Package Level 1", "Package Level 2", "Package Level 3" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C", "A" }) }));
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary", null, null, null, null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination2IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			mockDependencyDefinitionRepository.Setup(m => m.GetDependencyDefinition(It.IsAny<string>()))
				.ReturnsAsync(new DependencyDefinition("packageClassifications", new List<string> { "Package Level 1", "Package Level 2", "Package Level 3" },
					new List<DependentBlock> { new DependentBlock(new[] { "Primary", "A&C", "A" }) }));
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, "Primary", "A&C", null, null, null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination3IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			mockPackageRepository.Setup(m => m.Count("package", It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, null, "ALM0001", null);

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidCombination4IsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			mockPackageRepository.Setup(m => m.Count("package", It.IsAny<Dictionary<string, Tuple<string, object>>>())).ReturnsAsync(1);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Find(It.IsAny<string>()))
				.ReturnsAsync(new Project("0043", "Windmills of Your Mind", "WoYM"));
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			PackageCostPriceRatioValidator packageCostPriceRatioValidator =
				new PackageCostPriceRatioValidator(mockPackageRepository.Object, mockProjectRepository.Object,
					mockDependencyDefinitionRepository.Object);
			CostPriceRatio costPriceRatioData = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()), DateTime.Today.AddDays(1),
				ComponentType.Package, null, null, null, "ALM0001", "0043");

			var result = await packageCostPriceRatioValidator.Validate(costPriceRatioData);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo(string.Empty);
		}
	}
}