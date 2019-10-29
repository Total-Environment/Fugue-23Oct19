using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ExcelImporter.Code.Documents
{
    public class StaticFileInformation
    {
        public string PrimaryHeaderName { get; }
        public string SecondaryHeaderName { get; }
        public List<string> Names { get; }
        public List<StaticFile> StaticFiles { get; private set; }

        public StaticFileInformation(string primaryHeaderName, string secondaryHeaderName, List<string> names)
        {
            Names = names;
            PrimaryHeaderName = primaryHeaderName;
            SecondaryHeaderName = secondaryHeaderName;
        }

        public StaticFileInformation UpdateStaticFileList(List<StaticFile> urlList)
        {
            StaticFiles = urlList;
            return this;
        }

        protected bool Equals(StaticFileInformation other)
        {
            return string.Equals(PrimaryHeaderName, other.PrimaryHeaderName)
                   && string.Equals(SecondaryHeaderName, other.SecondaryHeaderName)
                   && Names.SequenceEqual(other.Names);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StaticFileInformation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PrimaryHeaderName.GetHashCode();
                hashCode = (hashCode * 397) ^ SecondaryHeaderName.GetHashCode();
                hashCode = (hashCode * 397) ^ Names.GetHashCode();
                return hashCode;
            }
        }
    }
}