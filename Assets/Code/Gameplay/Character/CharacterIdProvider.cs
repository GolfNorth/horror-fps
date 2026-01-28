using UnityEngine;

namespace Game.Gameplay.Character
{
    public class CharacterIdProvider : MonoBehaviour, ICharacterIdProvider
    {
        [SerializeField] private string _characterId = "player";

        public string CharacterId => _characterId;
    }
}
