using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Arkanodia.ObjectPools
{
    // A generic object pool that can manage any component type derived from Unity's Component class
    public abstract class ObjectPool<T> where T : Component
    {
        // The prefab to instantiate objects from
        private T prefab;

        // Factory for creating new instances of objects via Zenject injection
        [Inject] public Factory FactoryPool;

        // Queue to store the objects that are available for reuse (i.e., inactive objects)
        private readonly Queue<T> objectsAvailable = new();

        // List to track objects currently in use (active in the game world)
        public List<T> ObjectInGame { get; set; } = new List<T>();

        // Retrieve an object from the pool, activating it if necessary
        public T Get()
        {
            T obj = GetObject();
            ObjectInGame.Add(obj);  // Add the object to the active list
            return obj;
        }

        // Either retrieve an available object or instantiate a new one if the pool is empty
        private T GetObject()
        {
            if (objectsAvailable.Count == 0)
            {
                // If no available objects, create and return a new one
                return AddObject();
            }

            // Get an inactive object from the pool
            var obj = objectsAvailable.Dequeue();
            obj.gameObject.SetActive(true); // Reactivate the object
            return obj;
        }

        // Create a new object using the Zenject factory, activate it, and return it
        private T AddObject()
        {
            var obj = FactoryPool.Create();
            obj.gameObject.SetActive(true);
            return obj;
        }

        // Return an object to the pool by deactivating it and adding it back to the available queue
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);  // Deactivate the object
            ObjectInGame.Remove(obj);         // Remove it from the active list
            objectsAvailable.Enqueue(obj);    // Enqueue it to the available objects
        }

        // Factory class to allow Zenject to inject the creation of the objects in the pool
        public class Factory : PlaceholderFactory<T>
        {
        }
    }
}
