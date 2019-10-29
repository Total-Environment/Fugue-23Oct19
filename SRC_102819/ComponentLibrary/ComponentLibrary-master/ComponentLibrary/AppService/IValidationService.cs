using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Validation Service
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Returns the list of valid assembly codes, from the input list of assembly codes.
        /// </summary>
        /// <param name="assemblyCodes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<string>> ValidateAssemblyCodes(List<string> assemblyCodes, string type);
    }
}
