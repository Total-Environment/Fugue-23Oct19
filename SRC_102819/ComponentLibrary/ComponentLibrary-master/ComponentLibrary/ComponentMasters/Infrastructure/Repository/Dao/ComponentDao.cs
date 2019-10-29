using System;
using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Class ComponentDao.
    /// </summary>
    /// <seealso cref="Entity"/>
    public abstract class ComponentDao : Entity, IEquatable<ComponentDao>
    {
        /// <summary>
        /// Constant for can be used as an asset
        /// </summary>
        public const string CanBeUsedAsAnAsset = "can_be_used_as_an_asset";

        /// <summary>
        /// Constant for created by
        /// </summary>
        public const string CreatedBy = "created_by";

        /// <summary>
        /// Constant for date created
        /// </summary>
        public const string DateCreated = "date_created";

        /// <summary>
        /// Constant for last amended
        /// </summary>
        public const string DateLastAmended = "date_last_amended";

        /// <summary>
        /// Constant for group
        /// </summary>
        public const string Group = "group";

        /// <summary>
        /// Constant for last amended by
        /// </summary>
        public const string LastAmendedBy = "last_amended_by";

        /// <summary>
        /// Constant for material level2
        /// </summary>
        public const string MaterialLevel2 = "material_level_2";

        /// <summary>
        /// Constant for material name
        /// </summary>
        public const string MaterialName = "material_name";

        /// <summary>
        /// Constant for search keywords
        /// </summary>
        public const string SearchKeywords = "SearchKeywords";

        /// <summary>
        /// Constant for service level1
        /// </summary>
        public const string ServiceLevel1 = "service_level_1";

        /// <summary>
        /// Constant for component DAO code
        /// </summary>
        public const string ServiceCode = "service_code";

        /// <summary>
        /// Constant for short description
        /// </summary>
        public const string ShortDescription = "short_description";

        /// <summary>
        /// Constant for component DAO code
        /// </summary>
        protected string ComponentDaoCode = "component_code";

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public Dictionary<string, object> Columns { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ComponentDao)obj);
        }

        bool IEquatable<ComponentDao>.Equals(ComponentDao other)
        {
            return Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (Columns != null ? Columns[ComponentDaoCode].GetHashCode() : 0);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        protected bool Equals(ComponentDao other)
        {
            return Equals(Columns[ComponentDaoCode], other.Columns[ComponentDaoCode]);
        }
    }
}