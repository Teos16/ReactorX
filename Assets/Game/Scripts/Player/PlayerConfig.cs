using UnityEngine;

namespace Game.Scripts.Players
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/PlayerConfig")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public bool IsWorld { get; private set; }
        [field: SerializeField] public Color MainColor { get; private set; }
    }
}