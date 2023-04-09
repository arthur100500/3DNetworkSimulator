namespace Interfaces.Factory
{
    public interface IFactory
    {
        public T Create<T>();
    }
}