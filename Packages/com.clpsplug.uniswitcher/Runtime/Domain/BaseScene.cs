namespace UniSwitcher.Domain
{
    /// <summary>
    /// Basic <see cref="IScene"/> implementation.
    /// You should extend this class to add static members to avoid typing scene paths.
    /// </summary>
    public abstract class BaseScene : IScene
    {
        private readonly string _rawValue;

        public string RawValue
        {
            get => _rawValue;
        }
        
        protected BaseScene(string rawValue)
        {
            _rawValue = rawValue;
        }
        
        public static bool operator ==(BaseScene a, BaseScene b) 
        { 
            // null == null
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            // If either one is null but NOT both are non-null
            if (ReferenceEquals(a, null) != ReferenceEquals(b, null)) return false;
            // Both are non-null, then use RawValue
            return a.RawValue == b.RawValue;
        }

        public static bool operator !=(BaseScene a, BaseScene b)
        {
            return !(a == b);
        }

        public override bool Equals(object o)
        {
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            return this == (BaseScene)o;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public override string ToString()
        {
            return RawValue;
        }
    }
}