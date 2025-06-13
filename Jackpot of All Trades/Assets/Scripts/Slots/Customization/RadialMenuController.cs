using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class RadialMenuController : MonoBehaviour
{
    [Header("Line Visuals")]
    [Tooltip("Display for dividing lines in menu")]
    public GameObject linePrefab; // Prefab with a LineRenderer attached
    public Gradient lineColor; // Set this in the Inspector for custom line gradient
    [SerializeField] private float defaultLineWidth = 0.05f; // Serialized for easy tweaking in Inspector
    private float lineWidth;

    [Header("Sprite Visuals")]
    [Tooltip("Display for sprites in menu")]
    public GameObject spritePrefab; // Prefab for the sprite holder (with a SpriteRenderer)
    [SerializeField] private Transform spritesParent; // Drag an empty GameObject in the Inspector
    public Sprite[] sprites; // Array of sprites for the radial menu
    public Color normalColor, highlightedColor, selectedColor;
    [SerializeField] private float spriteDistanceFactor = 0.6f; // Controls how far sprites are from the center
    [SerializeField] private float baseScale = 0.2f; // Base size for sprites
    [SerializeField] private float scalingExponent = 0.7f; // Controls the rate of shrinking
    public float radius = 2.5f; // Radius of the circle
    private Vector3 spriteScale;

    [Header("Data Storage")]
    [Tooltip("DO NOT TOUCH")]
    public int selectedSpriteIndex = -1; // Index of the currently selected sprite (-1 means none)
    public TextMeshProUGUI selectedSpriteText; // Reference to the Text element

    private Vector2 moveInput; // Tracks mouse position relative to screen center

    private List<GameObject> lines = new List<GameObject>();
    private List<GameObject> spriteObjects = new List<GameObject>(); // Store generated sprite GameObjects

    void Start()
    {
        lineWidth = defaultLineWidth; // Initialize lineWidth
        GenerateDividingLines();
        GenerateSprites();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            spritePrefab.SetActive(true);
        }

        if (spritePrefab.activeInHierarchy && IsMouseOverCircle())
        {
            // Calculate moveInput based on the mouse's position relative to screen center
            moveInput.x = Input.mousePosition.x - (Screen.width / 2f);
            moveInput.y = Input.mousePosition.y - (Screen.height / 2f);

            // Calculate the angle of the mouse position
            float angle = Mathf.Atan2(moveInput.y, -moveInput.x) * Mathf.Rad2Deg;
            angle = (angle + 360) % 360; // Normalize angle to [0, 360)

            // Calculate the segment size
            int segmentSize = 360 / sprites.Length;

            // Calculate which segment the mouse is hovering over
            int highlightedIndex = Mathf.FloorToInt(angle / segmentSize);

            // Debugging output for highlight detection
            //Debug.Log($"Mouse Angle: {angle}, Highlighted Index: {highlightedIndex}");

            // Highlight the detected segment
            HighlightSprites(highlightedIndex);

            // Handle selection logic
            HandleSpriteSelection(highlightedIndex);
        }
        else
        {
            // Clear highlights when the mouse is not over the circle
            ClearHighlights();
        }
    }

    void HighlightSprites(int highlightedIndex)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteRenderer spriteRenderer = spriteObjects[i].GetComponent<SpriteRenderer>();
            if (i == highlightedIndex)
            {
                // Highlight the sprite in the detected segment
                highlightedColor.a = 1f; // Ensure highlighted color is fully opaque
                spriteRenderer.color = highlightedColor;
            }
            else if (i == selectedSpriteIndex)
            {
                //Special color to highlight the selected sprite
                selectedColor.a = 1f;
                spriteRenderer.color = selectedColor;
            }
            else
            {
                // Reset the color for other segments
                normalColor.a = 1f; // Ensure normal color is fully opaque
                spriteRenderer.color = normalColor;
            }
        }
    }

    void ClearHighlights()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteRenderer spriteRenderer = spriteObjects[i].GetComponent<SpriteRenderer>();
            if (i == selectedSpriteIndex)
            {
                selectedColor.a = 1f;
                spriteRenderer.color = selectedColor; // Keep selected sprite highlighted
            }
            else
            {
                normalColor.a = 1f;
                spriteRenderer.color = normalColor;
            }
        }
    }

    void HandleSpriteSelection(int highlightedIndex)
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            if (highlightedIndex != selectedSpriteIndex)
            {
                selectedSpriteIndex = highlightedIndex;

                // Update the text element
                if (selectedSpriteText != null)
                {
                    selectedSpriteText.text = $"Selected Sprite: {selectedSpriteIndex}";
                }

                Debug.Log($"Selected Sprite Index Updated: {selectedSpriteIndex}");
            }
            else
            {
                Debug.Log($"Click detected, but selectedSpriteIndex remains unchanged: {selectedSpriteIndex}");
            }
        }
    }
    bool IsMouseOverCircle()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        float distanceFromCenter = Vector3.Distance(Vector3.zero, mouseWorldPosition); // Assume the radial menu is centered at world origin
        return distanceFromCenter <= radius;
    }

    void GenerateDividingLines()
    {
        // Clear existing lines if any
        foreach (var line in lines)
        {
            Destroy(line);
        }
        lines.Clear();

        int segmentSize = 360 / sprites.Length;
        float angleStep = 360f / sprites.Length;

        for (int i = 0; i < sprites.Length; i++)
        {
            // Adjust line position to align with the boundary of each segment
            float angle = (i * angleStep) + (angleStep / (sprites.Length % 2 + 1f)); //aligns properly when there is an even number in sprites array
            float radian = angle * Mathf.Deg2Rad;

            Vector3 startPosition = Vector3.zero; // Center of the circle
            Vector3 endPosition = new Vector3(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0f);

            GameObject line = Instantiate(linePrefab, spritePrefab.transform);
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);

            // Explicitly set line width
            lineWidth = defaultLineWidth;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            // Apply color gradient if available
            if (lineColor != null)
            {
                lineRenderer.colorGradient = lineColor;
            }

            lines.Add(line);
        }
    }

    public void GenerateSprites()
    {
        // Debug log to track sprite generation
        Debug.Log("Regenerating sprites...");

        // Clear all existing sprite objects
        foreach (GameObject spriteObject in spriteObjects)
        {
            Destroy(spriteObject);
        }
        spriteObjects.Clear();

        // Ensure there are sprites to generate
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("No sprites found to generate.");
            return;
        }

        // Validate spritePrefab
        if (spritePrefab == null)
        {
            Debug.LogError("SpritePrefab is null! Please assign it in the Inspector.");
            return;
        }

        // Ensure parent for sprites
        if (spritesParent == null)
        {
            spritesParent = this.transform; // Default to current object if no parent is assigned
        }

        // Calculate angles and positions
        float angleStep = 360f / sprites.Length; // Angle between sprites
        for (int i = 0; i < sprites.Length; i++)
        {
            float angle = (i * angleStep) + ((angleStep / 2) /*- ((angleStep / 2) * (sprites.Length % 2))*/); // Center the sprite in its segment
            float radian = angle * Mathf.Deg2Rad;

            Vector3 position = new Vector3(
                -Mathf.Cos(radian) * radius * spriteDistanceFactor, // X-position
                Mathf.Sin(radian) * radius * spriteDistanceFactor, // Y-position
                0f // Z-position
            );
            // Instantiate sprite object
            GameObject spriteObj = Instantiate(spritePrefab, spritesParent);
            if (spriteObj == null)
            {
                Debug.LogError($"Failed to instantiate spritePrefab for sprite at index {i}.");
                continue;
            }

            // Set position and scale
            float scaleFactor = baseScale / Mathf.Pow(sprites.Length, scalingExponent);
            spriteScale = new Vector3(scaleFactor, scaleFactor, 1f);
            spriteObj.transform.localPosition = position;
            spriteObj.transform.localScale = spriteScale;

            // Configure SpriteRenderer
            SpriteRenderer spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError($"SpriteRenderer missing on instantiated object at index {i}.");
                continue;
            }

            // Set sprite and color
            spriteRenderer.sprite = sprites[i];
            spriteRenderer.color = (i == selectedSpriteIndex) ? selectedColor : normalColor; // saves color state from before then applies color if regenerating
            spriteRenderer.sortingOrder = 10;

            // Add sprite to tracking list
            spriteObjects.Add(spriteObj);
        }

        Debug.Log("Sprites regenerated successfully.");
    }
}

/*
TODO: 
- some logic surrounding the debug logs for highlighting and selecting sprites seems to be flawed
- change line color somehow
- sprites don't generate until cursor over menu
- tab actually opens/closes menu
*/