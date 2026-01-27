using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(
                Character.GetComponentsInChildren<Collider>()
            );
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            if (CharacterCamera.RotateWithPhysicsMover &&
                Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection =
                    Character.Motor.AttachedRigidbody
                        .GetComponent<PhysicsMover>()
                        .RotationDeltaFromInterpolation *
                    CharacterCamera.PlanarDirection;

                CharacterCamera.PlanarDirection =
                    Vector3.ProjectOnPlane(
                        CharacterCamera.PlanarDirection,
                        Character.Motor.CharacterUp
                    ).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            Vector2 look = Vector2.zero;

            if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null)
            {
                look = Mouse.current.delta.ReadValue();
            }

            float scroll = Mouse.current != null
                ? Mouse.current.scroll.ReadValue().y
                : 0f;

#if UNITY_WEBGL
            scroll = 0f;
#endif

            CharacterCamera.UpdateWithInput(
                Time.deltaTime,
                scroll,
                new Vector3(look.x, look.y, 0f)
            );

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CharacterCamera.TargetDistance =
                    CharacterCamera.TargetDistance == 0f
                        ? CharacterCamera.DefaultDistance
                        : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            Vector2 move = Vector2.zero;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) move.y += 1f;
                if (Keyboard.current.sKey.isPressed) move.y -= 1f;
                if (Keyboard.current.dKey.isPressed) move.x += 1f;
                if (Keyboard.current.aKey.isPressed) move.x -= 1f;
            }

            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs
            {
                MoveAxisForward = move.y,
                MoveAxisRight = move.x,
                CameraRotation = CharacterCamera.Transform.rotation,
                JumpDown = Keyboard.current?.spaceKey.wasPressedThisFrame ?? false,
                CrouchDown = Keyboard.current?.cKey.wasPressedThisFrame ?? false,
                CrouchUp = Keyboard.current?.cKey.wasReleasedThisFrame ?? false
            };

            Character.SetInputs(ref characterInputs);
        }
    }
}
