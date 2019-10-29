using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class SimpleDataTypeFactoryTests
    {
        public class Fixture
        {
            private readonly Mock<ICheckListRepository> _checkListRepository = new Mock<ICheckListRepository>();
            private readonly Mock<IMasterDataRepository> _masterDataListRepository = new Mock<IMasterDataRepository>();
            private readonly Mock<IStaticFileRepository> _staticFileRepository = new Mock<IStaticFileRepository>();

            public SimpleDataTypeFactory SystemUnderTest()
            {
                return new SimpleDataTypeFactory(
                    _checkListRepository.Object,
                    _masterDataListRepository.Object,
                    _staticFileRepository.Object
                );
            }

            public void ForCheckList()
            {
            }
        }

        [Fact]
        public void New_ShouldReturnFactory_WhichCanGetCheckList_WhenPassedRepositories()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Construct("CheckList", "").Should().NotBeNull();
        }

        [Fact]
        public void New_ShouldReturnFactory_WhichCanGetConstant_WhenPassedRepositories()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Construct("Constant", "primary").Should().NotBeNull();
        }

        [Fact]
        public void New_ShouldReturnFactory_WhichCanGetMasterData_WhenPassedRepositories()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Construct("MasterData", "material_level_3").Should().NotBeNull();
        }

        [Fact]
        public void New_ShouldReturnFactory_WhichCanGetBrand_WhenPassedRepositories()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Construct("Brand", "BCY").Should().NotBeNull();
        }
    }
}