using UnityEngine;
using UnityEngine.UI;

public class WheelSpriteSwap : MonoBehaviour
{
    public RadialMenuController radialMenu; // Reference to the RadialMenuController script
    public Button[] buttons; // Array of buttons in the UI
    public Sprite[] buttonSprites; // Array of sprites corresponding to each button

    void Start()
    {
        // Ensure each button calls OnButtonClick with the correct index
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Capture index to avoid closure issues
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    // Called when a button is clicked
    public void OnButtonClick(int buttonIndex)
    {
        Debug.Log($"Button pressed");

        if (radialMenu == null)
        {
            Debug.LogError("RadialMenuController reference is missing!");
            return;
        }

        // Validate the button index
        if (buttonIndex >= 0 && buttonIndex < buttonSprites.Length)
        {
            // Ensure a sprite is currently selected in the radial menu
            if (radialMenu.selectedSpriteIndex != -1)
            {
                // Update the selected sprite with the new one from the button
                radialMenu.sprites[radialMenu.selectedSpriteIndex] = buttonSprites[buttonIndex];

                Debug.Log($"Updated sprite at index {radialMenu.selectedSpriteIndex} with sprite from button index {buttonIndex}");

                // Refresh the radial menu display to show the updated sprite
                radialMenu.GenerateSprites();
            }
            else
            {
                Debug.LogWarning("No sprite is currently selected in the radial menu.");
            }
        }
        else
        {
            Debug.LogError($"Button index {buttonIndex} is out of bounds for buttonSprites array.");
        }
    }
}
