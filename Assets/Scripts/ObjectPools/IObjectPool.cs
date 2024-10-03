using System.Collections.Generic;

namespace Arkanodia.ObjectPools
{
    public interface IObjectPool<T>
    {
        public T Get();
        void Return(T objects);
        List<T> ObjectInGame { get; set; }
    }
}