using Game.Core.Configuration;
using UnityEngine;

namespace Game.Player.Configs
{
    /// <summary>
    /// Configuration for player camera/look parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerLookConfig", menuName = "Game/Player/Look Config")]
    public class PlayerLookConfig : GameConfig
    {
        [Header("Sensitivity")]
        [SerializeField] private float _horizontalSensitivity = 2f;
        [SerializeField] private float _verticalSensitivity = 2f;

        [Header("Pitch Constraints")]
        [SerializeField] private float _minPitch = -89f;
        [SerializeField] private float _maxPitch = 89f;

        public float HorizontalSensitivity => _horizontalSensitivity;
        public float VerticalSensitivity => _verticalSensitivity;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
    }
}
