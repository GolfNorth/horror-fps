using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System.Linq;

namespace KinematicCharacterController.Walkthrough.SimpleJumping
{
    public class MyPlayer : MonoBehaviour
    {
        public ExampleCharacterCamera OrbitCamera;
        public Transform CameraFollowPoint;
        public MyCharacterController Character;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            OrbitCamera.SetFollowTransform(CameraFollowPoint);
            OrbitCamera.IgnoredColliders.Clear();
            OrbitCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            Vector2 look = Vector2.zero;
            if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null)
            {
                look = Mouse.current.delta.ReadValue();
            }

            float scrollInput = Mouse.current != null ? -Mouse.current.scroll.ReadValue().y : 0f;
#if UNITY_WEBGL
            scrollInput = 0f;
#endif

            OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, new Vector3(look.x, look.y, 0f));

            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f) ? OrbitCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            Vector2 move = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) move.y += 1f;
                if (Keyboard.current.sKey.isPressed) move.y -= 1f;
                if (Keyboard.current.dKey.isPressed) move.x += 1f;
                if (Keyboard.current.aKey.isPressed) move.x -= 1f;
            }

            characterInputs.MoveAxisForward = move.y;
            characterInputs.MoveAxisRight = move.x;
            characterInputs.CameraRotation = OrbitCamera.Transform.rotation;
            characterInputs.JumpDown = Keyboard.current?.spaceKey.wasPressedThisFrame ?? false;

            Character.SetInputs(ref characterInputs);
        }
    }
}
