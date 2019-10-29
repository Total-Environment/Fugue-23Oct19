using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure
{
    /// <summary>
    /// Represents a data type factory
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.Domain.IDataTypeFactory" />
    public class DataTypeFactory : IDataTypeFactory
    {
        private readonly IBrandDefinitionRepository _brandDefinitionRepository;
        private readonly IBrandCodeGenerator _brandCodeGenerator;
        private readonly ISimpleDataTypeFactory _simpleDefinitionRepository;
        private readonly ICounterRepository _counterRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeFactory"/> class.
        /// </summary>
        /// <param name="simpleDefinitionRepository">The simple definition repository.</param>
        /// <param name="brandDefinitionRepository">The brand definition repository.</param>
        /// <param name="brandCodeGenerator">The brand code generator.</param>
        /// <param name="counterRepository">The counter repository.</param>
        public DataTypeFactory(ISimpleDataTypeFactory simpleDefinitionRepository, IBrandDefinitionRepository brandDefinitionRepository, IBrandCodeGenerator brandCodeGenerator, ICounterRepository counterRepository)
        {
            _simpleDefinitionRepository = simpleDefinitionRepository;
            _brandDefinitionRepository = brandDefinitionRepository;
            _brandCodeGenerator = brandCodeGenerator;
            _counterRepository = counterRepository;
        }

        /// <summary>
        /// Constructs the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="subType">Type of the sub.</param>
        /// <returns></returns>
        public async Task<IDataType> Construct(string dataType, object subType)
        {
            switch (dataType)
            {
                case "Brand":
                    var brandDefinition = await _brandDefinitionRepository.FindBy("Generic Brand");
                    return new BrandDataType(brandDefinition, (string)subType, _brandCodeGenerator, _counterRepository);

                default:
                    return await _simpleDefinitionRepository.Construct(dataType, subType);
            }
        }
    }
}