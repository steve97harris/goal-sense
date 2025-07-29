using UnityEngine;
using System.Collections.Generic;

namespace Framework.Services
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool;
        private readonly List<T> _activeObjects;

        public ObjectPool(T prefab, Transform parent, int initialSize)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new Queue<T>();
            _activeObjects = new List<T>();

            // Create initial pool
            for (int i = 0; i < initialSize; i++)
                CreateNewObject();
        }

        private void CreateNewObject()
        {
            T obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        public T Get()
        {
            if (_pool.Count == 0)
                CreateNewObject();

            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            _activeObjects.Add(obj);
            
            return obj;
        }

        public void Return(T obj)
        {
            if (obj is IPoolable poolableObj)
                poolableObj.OnDespawn();
            
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
            _activeObjects.Remove(obj);
        }

        public void ReturnAll()
        {
            foreach (var obj in _activeObjects.ToArray())
                Return(obj);
        }
    }
}