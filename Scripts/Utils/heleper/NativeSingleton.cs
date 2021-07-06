
namespace AnilTools
{
    public class NativeSingleton <T> where T : new ()
    {
        private T _instance;
        public T instance
        {
            get
            {
                if (_instance == null) _instance = new T();
                return _instance;
            }
        }
    }
}
