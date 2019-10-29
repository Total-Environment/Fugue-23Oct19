using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Represents the DAO for static files
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class StaticFileDao : Entity
    {
        /// <summary>
        /// Gets or sets the static file identifier.
        /// </summary>
        /// <value>
        /// The static file identifier.
        /// </value>
        public string StaticFileId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileDao"/> class.
        /// </summary>
        public StaticFileDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileDao"/> class.
        /// </summary>
        /// <param name="staticFile">The static file.</param>
        public StaticFileDao(StaticFile staticFile)
        {
            StaticFile = staticFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileDao"/> class.
        /// </summary>
        /// <param name="staticFile">The static file.</param>
        /// <param name="objectId">The object identifier.</param>
        public StaticFileDao(StaticFile staticFile, ObjectId objectId) : this(staticFile)
        {
            ObjectId = objectId;
        }

        /// <summary>
        /// Gets or sets the static file.
        /// </summary>
        /// <value>
        /// The static file.
        /// </value>
        [BsonIgnore]
        public StaticFile StaticFile
        {
            get
            {
                var staticFile = new StaticFile(StaticFileId ?? ObjectId.ToString(), Name);
                return staticFile;
            }

            set
            {
                StaticFileId = value.Id;
                Name = value.Name;
            }
        }
    }
}