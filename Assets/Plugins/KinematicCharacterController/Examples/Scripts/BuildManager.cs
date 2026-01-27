using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BuildManager : MonoBehaviour
{
    public GameObject WebGLCanvas;
    public GameObject WarningPanel;

    void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLCanvas.SetActive(true);
#endif
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            WarningPanel.SetActive(!WarningPanel.activeSelf);
        }
#endif
    }
}
