using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MoveMouse : MonoBehaviour
{
  void Start()
  {
     Debug.Log("MoveMouse!");
    return;

    // Make cursor visible and free
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    // Example: move cursor to center of screen
    Vector2 screenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    WarpCursor(screenPos);
  }

  public void WarpCursor(Vector2 screenPosition)
  {
    if (Mouse.current == null) return;

    // Move the cursor
    Mouse.current.WarpCursorPosition(screenPosition);

    // Update Input System state (important if "Both" input handling)
    InputState.Change(Mouse.current.position, screenPosition);
  }
}
