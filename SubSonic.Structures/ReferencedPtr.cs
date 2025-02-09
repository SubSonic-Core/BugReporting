namespace SubSonic
{
    public class ReferencedPtr<TType>
         where TType : class
    {
        protected internal int _count = 0;

        private TType? _ptr;

        public ReferencedPtr(TType instance)
        { _ptr = instance; }

        public int RefCount => _count;

        public TType? Ptr => _ptr;

        public static implicit operator TType? (ReferencedPtr<TType> ptr) => ptr?._ptr;
    }
}
