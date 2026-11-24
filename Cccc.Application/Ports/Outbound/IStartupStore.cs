namespace Cccc.Application
{
    public interface IStartupStore
    {
        string Read();
        void Write(string command);
        void Delete();
    }
}
