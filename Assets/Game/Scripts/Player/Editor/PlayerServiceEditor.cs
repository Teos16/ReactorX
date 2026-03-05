using Game.Scripts.Players;
using UnityEngine;

namespace Game.Scripts.Player.Editor
{
    [ExecuteAlways]
    public class PlayerServiceEditor : MonoBehaviour
    {
        [SerializeField] private PlayerCatalog _catalog;

        public PlayerConfig GetPlayerConfig(string name)
        {
            if (_catalog == null) return null;
            return _catalog.GetPlayerConfig(name);
        }

        public PlayerCatalog Catalog => _catalog;
    }
}