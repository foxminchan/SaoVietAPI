namespace Domain.Interfaces
{
    public interface ICache
    {
        public void Remove(string cacheKey);
        public T Set<T>(string cacheKey, T value);
        public bool TryGet<T>(string cacheKey, out T value);
    }
}
