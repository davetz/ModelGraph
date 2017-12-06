
namespace ModelGraphLibrary
{
    internal interface IValueStore<T>
    {
        int Count { get; }
        void Clear();
        void Remove(Item key);
        bool GetVal(Item key, out T val);
        bool SetVal(Item key, T value);
    }
}
