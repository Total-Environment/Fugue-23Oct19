using System.Collections.Generic;
using System.Reflection;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for Component Data
    /// </summary>
    /// <typeparam name="T">Material / Service</typeparam>
    public class ComponentDataDto<T> where T : new()
    {
        /// <summary>
        /// Get or set Headers.
        /// </summary>
        public IEnumerable<HeaderDto> Headers { get; set; }

        /// <summary>
        /// Get or set service / material group.
        /// </summary>
        public string Group { get; set; }

        private void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
        }

        /// <summary>
        /// Updates Headers on the domain instance.
        /// Fetch the domain instance (material / service)
        /// </summary>
        /// <returns></returns>
        public T GetDomain()
        {
            var component = new T();
            TrySetProperty(component, "Headers", Headers);
            return component;
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
    }
}