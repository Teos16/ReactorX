using System;
using System.Collections.Generic;

namespace Game.Scripts.Tile
{
    public class Entity
    {
        private int ID;
        
        private readonly Dictionary<string, object> _components = new();

        public string Add(object component)
        {
            string key = Guid.NewGuid().ToString();
            _components.Add(key, component);
            return key;
        }

        public void Remove(string key) => _components.Remove(key);

        public void Remove(object component)
        {
            string keyToRemove = null;
            foreach (var kv in _components)
                if (kv.Value == component)
                {
                    keyToRemove = kv.Key;
                    break;
                }
            if (keyToRemove != null)
                _components.Remove(keyToRemove);
        }

        public T Get<T>(string key) where T : class
        {
            if (_components.TryGetValue(key, out var comp))
                return comp as T;
            return null;
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            foreach (var obj in _components.Values)
            {
                if (obj is T t)
                    yield return t;
            }
        }
    }
}