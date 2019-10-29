using System;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceAndCompositeComponentRequest
    {
        private string _shortDescription = "";
        private string _gstApplicability  = "";
        public string Code { get; set; }

        public string UnitOfMeasure { get; set; }

        public DateTime CreatedAt { get; set; }

        // Changing this to C#7 syntax causes issues. Please don't or try to fix them.
        public string ShortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value ?? ""; }
        }

        public bool Update { get; set; }

        public string SACCode { get; set; }

        public string GSTApplicability
        {
            get { return _gstApplicability; }
            set { _gstApplicability = value ?? ""; } // Should never be null
        }

        public string ComponentType { get; set; }

        protected bool Equals(ServiceAndCompositeComponentRequest other)
        {
            return string.Equals(_shortDescription, other._shortDescription) && string.Equals(_gstApplicability, other._gstApplicability) && string.Equals(Code, other.Code) && string.Equals(UnitOfMeasure, other.UnitOfMeasure) && CreatedAt.Equals(other.CreatedAt) && Update == other.Update && string.Equals(SACCode, other.SACCode) && string.Equals(ComponentType, other.ComponentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceAndCompositeComponentRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_shortDescription != null ? _shortDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_gstApplicability != null ? _gstApplicability.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UnitOfMeasure != null ? UnitOfMeasure.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CreatedAt.GetHashCode();
                hashCode = (hashCode * 397) ^ Update.GetHashCode();
                hashCode = (hashCode * 397) ^ (SACCode != null ? SACCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ComponentType != null ? ComponentType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}