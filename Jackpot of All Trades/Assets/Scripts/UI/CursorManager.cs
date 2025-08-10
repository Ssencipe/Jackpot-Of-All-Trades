using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D clickCursor;

    private bool isClickCursor = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetDefaultCursor();
    }

    //for audio
    private void Update()
    {
        // Check if user clicks while the click cursor is active
        if (isClickCursor && Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance?.PlayUI("select");
        }
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        isClickCursor = false;
    }

    //when hovering over interactables like buttons
    public void SetClickCursor()
    {
        Cursor.SetCursor(clickCursor, Vector2.zero, CursorMode.Auto);
        isClickCursor = true;
    }
}
