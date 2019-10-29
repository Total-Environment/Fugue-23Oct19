using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for checklists
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class CheckListDao : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListDao"/> class.
        /// </summary>
        public CheckListDao()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListDao"/> class.
        /// </summary>
        /// <param name="checkList">The check list.</param>
        public CheckListDao(CheckList checkList)
        {
            CheckList = checkList;
        }

        /// <summary>
        /// Gets or sets the check list.
        /// </summary>
        /// <value>
        /// The check list.
        /// </value>
        [BsonIgnore]
        public virtual CheckList CheckList
        {
            get { return new CheckList {Id = ObjectId.ToString(), CheckListId = CheckListId, Content = Content, Template = Template, Title = Name}; }
            set
            {
                CheckListId = value.CheckListId;
                Content = value.Content;
                Template = value.Template;
                Name = value.Title;
            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public Table Content { get; set; }
        /// <summary>
        /// Gets or sets the check list identifier.
        /// </summary>
        /// <value>
        /// The check list identifier.
        /// </value>
        public string CheckListId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        public string Template { get; set; }
    }
}