using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Players
{
    public sealed class PlayerService : MonoBehaviour
    {
        [SerializeField] private PlayerCatalog _catalog;
        
        private List<Player> _players = new();
        
        private void OnEnable()
        {
            _players = new List<Player>();

            foreach (PlayerConfig config in _catalog.PlayerConfigs)
            {
                Player player = gameObject.AddComponent<Player>();
                player.SetConfig(config);
                _players.Add(player);
            }
        }

        private void OnDisable()
        {
            foreach (Player player in _players)
            {
                if (player != null)
                    DestroyImmediate(player);
            }
            _players.Clear();
        }

        public Player GetPlayer(string name) => 
            _players.Find(p => p.Name == name);
        
        public int GetCount() => _players.Count;
    }
}