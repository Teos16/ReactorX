using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Players
{
    [CreateAssetMenu(fileName = "PlayerCatalog", menuName = "Player/PlayerCatalog")]
    public sealed class PlayerCatalog : ScriptableObject
    {
        [field:SerializeField] public List<PlayerConfig> PlayerConfigs { get; private set;}
        
        public PlayerConfig GetPlayerConfig(string name) => 
            PlayerConfigs.Find(config => config.Name == name);
    }
}