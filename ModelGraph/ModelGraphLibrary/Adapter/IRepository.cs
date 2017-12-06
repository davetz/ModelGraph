
namespace ModelGraphLibrary
{
    public interface IRepository
    {
        string Name { get; }
        string FullName { get; }
        void Read(Chef chef);
        void Write(Chef chef);
    }
}
