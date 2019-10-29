using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents a material code generator
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IMaterialCodeGenerator" />
    public class MaterialCodeGenerator : IMaterialCodeGenerator
    {
        private readonly ICounterGenerator _counterGenerator;
        private string MaterialIdCounterCollection = "Material";

        /// <summary>
        /// Constructor for Material code Generator
        /// </summary>
        /// <param name="counterRepository"></param>
        public MaterialCodeGenerator(ICounterGenerator counterGenerator)
        {
            _counterGenerator = counterGenerator;
        }

        /// <inheritdoc />
        public async Task<string> Generate(string materialCodePrefix, IMaterial material)
        {
            var materialCode = MaterialCode(material);

            return await _counterGenerator.Generate(materialCodePrefix, materialCode, MaterialIdCounterCollection);
        }

        private static string MaterialCode(IMaterial material)
        {
            return ((dynamic)material).general.material_code.Value;
        }
    }
}