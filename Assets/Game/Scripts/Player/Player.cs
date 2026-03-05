using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Players
{
    public sealed class Player : MonoBehaviour
    {
        public GUID ID { get; private set; }
        public bool IsAlive { get; private set; }

        public string Name => _config.Name;
        public Color MainColor => _config.MainColor;

        private PlayerConfig _config;
        
        public void SetConfig(PlayerConfig config)
        {
            if (config == null) 
                throw new System.ArgumentNullException(nameof(config));
            
            _config = config;
        }
    }
}