namespace Domain.Interfaces
{
    public interface ICache
    {
        public bool TryGet<T>(string cacheKey, out T value);
        public T Set<T>(string cacheKey, T value);
        public void Remove(string cacheKey);
    }
}
