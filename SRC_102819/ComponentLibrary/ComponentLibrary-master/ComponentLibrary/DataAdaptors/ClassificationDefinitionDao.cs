using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.DataAdaptors
{
    /// <summary>
    /// The DAO for Classification Definition
    /// </summary>
    public class ClassificationDefinitionDao : Entity
    {
        public ClassificationDefinitionDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationDefinitionDao"/> class.
        /// </summary>
        /// <param name="classificationDefinition">The service classification definition.</param>
        /// <param name="componentType"></param>
        public ClassificationDefinitionDao(ClassificationDefinition classificationDefinition, string componentType = "service")
        {
            if (classificationDefinition == null)
                throw new ArgumentException(nameof(classificationDefinition));
            Value = classificationDefinition.Value;
            ComponentType = componentType;
            Description = classificationDefinition.Description;
            ClassificationDefinitionDaos =
                classificationDefinition.ServiceClassificationDefinitions?.Select(
                    x => new ClassificationDefinitionDao(x, componentType)).ToList();
        }

        /// <summary>
        /// Gets or sets the service classification definition daos.
        /// </summary>
        /// <value>
        /// The service classification definition daos.
        /// </value>
        public List<ClassificationDefinitionDao> ClassificationDefinitionDaos { get; set; }

        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        /// <value>
        /// The type of the component.
        /// </value>
        public string ComponentType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Domains this instance.
        /// </summary>
        /// <returns></returns>
        public ClassificationDefinition Domain()
        {
            return new ClassificationDefinition(Value, Description,
                ClassificationDefinitionDaos?.Select(x => x.Domain()).ToList());
        }

        /// <summary>
        /// Merges the new tree path.
        /// </summary>
        /// <param name="newTreePath">The new tree path.</param>
        /// <exception cref="DuplicateResourceException">New service classification definition path already exists.</exception>
        public void MergeNewTreePath(ClassificationDefinitionDao newTreePath)
        {
            var existing = this;
            while (true)
            {
                if (newTreePath == null)
                    throw new DuplicateResourceException($"New {ComponentType} classification definition path already exists.");
                if (existing.ClassificationDefinitionDaos == null)
                    existing.ClassificationDefinitionDaos = new List<ClassificationDefinitionDao>();
                var treeNode =
                    existing.ClassificationDefinitionDaos.FirstOrDefault(
                        node =>
                                string.Equals(node.Value, newTreePath.Value, StringComparison.InvariantCultureIgnoreCase));
                if (treeNode == null)
                {
                    existing.ClassificationDefinitionDaos.Add(newTreePath);
                    return;
                }
                if (
                    !string.Equals(treeNode.Description, newTreePath.Description,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException("Definition for " + newTreePath.Value +
                                                " at the given hierarchy is already available. Duplication or updating the existing definition is not allowed.");
                existing = treeNode;
                newTreePath = newTreePath?.ClassificationDefinitionDaos?.FirstOrDefault();
            }
        }

        /// <summary>
        /// Validates the tree path.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Service Classification Definition Dao has tree structure.
        /// or
        /// Service Classification Definition Dao has deeper than seven levels
        /// </exception>
        public void ValidateTreePath()
        {
            if (!IsSkewedTree())
                throw new ArgumentException($"{ComponentType} Classification Definition Dao has tree structure.");
            if (Depth() > 7)
                throw new ArgumentException($"{ComponentType} Classification Definition Dao has deeper than seven levels");
        }

        private int Depth()
        {
            if (ClassificationDefinitionDaos == null || ClassificationDefinitionDaos.Count == 0)
                return 1;
            var depth = ClassificationDefinitionDaos.First().Depth();
            return 1 + depth;
        }

        private bool HasAtMostOneChild()
        {
            if (ClassificationDefinitionDaos == null)
                return true;
            return ClassificationDefinitionDaos.Count <= 1;
        }

        private bool IsSkewedTree()
        {
            if (ClassificationDefinitionDaos == null || ClassificationDefinitionDaos.Count == 0)
                return true;
            return HasAtMostOneChild() && ClassificationDefinitionDaos.First().IsSkewedTree();
        }
    }
}