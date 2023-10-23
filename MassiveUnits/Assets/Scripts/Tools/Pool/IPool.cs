public interface IPool<T>
{
    void Prewarm(int amount);
    T Request();
    void Return(T obj);
}
public interface IPoolObject
{
    void OnSpawn();
    void OnDespawn();
    void OnCreated();
}