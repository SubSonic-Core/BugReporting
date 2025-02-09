using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubSonic
{
    public class DisposableObjectPtr<TType>
        : IDisposable
        where TType : class, IDisposableObject
    {
        private ReferencedPtr<TType> _referencePtr;

        public DisposableObjectPtr(TType ptr)
        {
            if (ptr == null)
            {
                throw new ArgumentNullException(nameof(ptr));
            }

            _referencePtr = new ReferencedPtr<TType>(ptr);

            if (ptr != null)
            {
                ptr.Dispose = Dispose;
            }
        }

        public TType? Ptr => _referencePtr;

        public int ReferenceCount => _referencePtr?.RefCount ?? default;

        protected bool Disposed => _referencePtr?.Ptr == null;

        public void Dispose()
        {
            if(!Disposed)
            {
                if (Interlocked.Decrement(ref _referencePtr._count) == 0)
                {
                    if (Ptr != null)
                    {
                        Ptr.Callback?.Invoke();

                        Ptr.Dispose = null!;
                    }

                    _referencePtr = null!;
                }
            }
        }

        public DisposableObjectPtr<TType> IncrementReferenceCount()
        {
            if (_referencePtr != null)
            {
                Interlocked.Increment(ref _referencePtr._count);
            }

            return this;
        }

        public bool TryIncrementReferenceCount(out TType? ptr)
        {
            ptr = null;

            if (_referencePtr != null)
            {
                int previousRefCount;
                do
                {
                    previousRefCount = _referencePtr._count;
                } while (previousRefCount != Interlocked.CompareExchange(ref _referencePtr._count, previousRefCount + 1, previousRefCount));

                ptr = this;
            }

            return ptr != null;
        }

        public static implicit operator DisposableObjectPtr<TType>(TType ptr) => new DisposableObjectPtr<TType>(ptr);
#pragma warning disable CS8603 // possible null reference return
        public static implicit operator TType(DisposableObjectPtr<TType> objectPtr) => objectPtr?.Ptr;
#pragma warning restore CS8603
    }
}
