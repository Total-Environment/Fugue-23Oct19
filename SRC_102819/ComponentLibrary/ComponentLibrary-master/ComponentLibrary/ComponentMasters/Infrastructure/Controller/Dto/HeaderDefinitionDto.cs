using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Represents a header defintion dto.
    /// </summary>
    public class HeaderDefinitionDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDefinitionDto"/> class.
        /// </summary>
        /// <param name="headerDefinition">The header definition.</param>
        public HeaderDefinitionDto(IHeaderDefinition headerDefinition)
        {
            SetDomain(headerDefinition);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDefinitionDto"/> class.
        /// </summary>
        public HeaderDefinitionDto()
        {
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<ColumnDefinitionDto> Columns { get; set; }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public List<string> Dependencies { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        /// <returns></returns>
        public async Task<IHeaderDefinition> GetDomain(IDataTypeFactory factory,
            IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            var tasks = Columns.Select(c => c.GetDomain(factory));
            var columns = await Task.WhenAll(tasks);
            List<DependencyDefinition> dependencies = null;
            if (Dependencies != null)
            {
                var dependencyTasks = Dependencies.Select(dependencyDefinitionRepository.GetDependencyDefinition).ToList();
                dependencies = (await Task.WhenAll(dependencyTasks)).ToList();
            }
            //May be remove after migration, dont want to set keys in all jsons
            if (Key == null || Key.Contains(' '))
            {
                Key = Name.Replace(' ', '_').ToLower();
            }
            return new HeaderDefinition(Name, Key, columns, dependencies);
        }

        /// <summary>
        /// Sets the domain.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetDomain(IHeaderDefinition value)
        {
            Columns = value.Columns.Select(c => new ColumnDefinitionDto(c)).ToList();
            Name = value.Name;
            Key = value.Key;
            Dependencies = value.Dependency?.Select(d => d.Name).ToList();
        }
    }
}