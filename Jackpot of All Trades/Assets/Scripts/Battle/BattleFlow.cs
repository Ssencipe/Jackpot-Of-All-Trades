using UnityEngine;

public class BattleFlow : MonoBehaviour
{
    [Header("Screens")]
    public GameObject battleScreen;       // Main battle HUD
    public GameObject slotMachineScreen;  // Slot machine HUD

    private void Start()
    {
        ShowBattleScreen();
    }

    public void ShowBattleScreen()
    {
        if (battleScreen != null) battleScreen.SetActive(true);
        if (slotMachineScreen != null) slotMachineScreen.SetActive(false);
    }

    public void ShowSlotMachineScreen()
    {
        if (battleScreen != null) battleScreen.SetActive(false);
        if (slotMachineScreen != null) slotMachineScreen.SetActive(true);
    }
}
