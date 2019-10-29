using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents an interface for material data
    /// </summary>
    public interface IService : IComponent
    {
        void UpdateColumn(string columnName, Dictionary<string, object> value);
    }
}