using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider shieldSlider;
    public TextMeshProUGUI shieldText;

    public GameObject floatingNumberPrefab;

    public void SetHUD(Unit unit)
    {
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        SetHP(unit.currentHP); //Initialize HP display

        shieldSlider.maxValue = unit.maxHP;
        shieldSlider.value = unit.currentShield;
        SetShield(unit.currentShield); // Initialize shield display
    }

    // Overloaded method for enemies
    public void SetHUD(EnemyBase enemy)
    {
        hpSlider.maxValue = enemy.maxHP;
        hpSlider.value = enemy.currentHP;
        SetHP(enemy.currentHP);

        shieldSlider.maxValue = enemy.maxHP;
        shieldSlider.value = enemy.currentShield;
        SetShield(enemy.currentShield);
    }

    public void SetHP(int hp)
    {
        hpText.text = "Health: " + hp; //update HP text

        hpSlider.value = hp; //update healthbar
    }

    public void SetShield(int shield)
    {
        shieldText.text = "Shield: " + shield; // Update shield text
        shieldSlider.value = Mathf.Clamp(shield, 0, shieldSlider.maxValue); // Scale the shield bar based on max HP

        // Spawn floating shield number
        SpawnFloatingNumber(shield, FloatingNumberType.Shield, shieldText.transform.position);
    }

    public void SpawnFloatingNumber(int value, FloatingNumberType type, Vector3 spawnPosition)
    {
        GameObject floatingNumber = Instantiate(floatingNumberPrefab, spawnPosition, Quaternion.identity, transform);
        FloatingNumberController controller = floatingNumber.GetComponent<FloatingNumberController>();
        controller.Initialize(value, type);
    }
}