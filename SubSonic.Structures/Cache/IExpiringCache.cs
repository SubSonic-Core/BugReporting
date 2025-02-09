namespace SubSonic.Cache
{
    public interface IExpiringCache<TCacheKey, TCacheValue>
        : ICache<TCacheKey, TCacheValue>
    {
        void RemoveExpiredValues();
    }
}
