using System;

namespace SubSonic.Cache
{
    public class ExpiringCacheValue<TCacheValue>
    {
        protected readonly DateTime _createdOnUtc;

        public ExpiringCacheValue(TimeSpan expiresIn, TCacheValue value) 
        {
            ExpiresIn = expiresIn;
            Value = value;

            _createdOnUtc = DateTime.UtcNow;
        }

        public TimeSpan ExpiresIn { get; }

        public bool IsExpired => _createdOnUtc.Add(ExpiresIn) < DateTime.UtcNow;

        public TCacheValue Value { get; }
    }
}
