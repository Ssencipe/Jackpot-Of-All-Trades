using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider shieldSlider;
    public TextMeshProUGUI shieldText;
    public GameObject floatingNumberPrefab;

    public void Bind(Unit unit)
    {
        if (unit == null) return;

        hpSlider.maxValue = unit.maxHP;
        shieldSlider.maxValue = unit.maxHP;

        SetHP(unit.currentHP);
        SetShield(unit.currentShield);

        unit.OnHealthChanged += SetHP;
        unit.OnShieldChanged += SetShield;
        unit.OnFloatingNumber += SpawnFloatingNumber;
    }

    public void Bind(BaseEnemy enemy)
    {
        if (enemy == null) return;

        hpSlider.maxValue = enemy.baseData.maxHealth;
        shieldSlider.maxValue = enemy.baseData.maxHealth;

        SetHP(enemy.currentHP);
        SetShield(enemy.currentShield);

        enemy.OnHealthChanged += SetHP;
        enemy.OnShieldChanged += SetShield;
        enemy.OnFloatingNumber += SpawnFloatingNumber;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
        hpText.text = $"Health: {hp}";
    }

    public void SetShield(int shield)
    {
        shieldSlider.value = Mathf.Clamp(shield, 0, shieldSlider.maxValue);
        shieldText.text = $"Shield: {shield}";
    }

    private void SpawnFloatingNumber(FloatingNumberData data)
    {
        if (floatingNumberPrefab == null)
        {
            Debug.LogWarning("[BattleHUD] FloatingNumber prefab not assigned.");
            return;
        }

        GameObject floating = Instantiate(floatingNumberPrefab, transform); // or some other clean canvas
        var controller = floating.GetComponentInChildren<FloatingNumberController>();
        if (controller == null)
        {
            Debug.LogError("[BattleHUD] FloatingNumber prefab is missing FloatingNumberController!");
            return;
        }

        Debug.Log($"[BattleHUD] Spawning floating number: {data.value} ({data.type})");
        controller.Initialize(data.value, data.type);
    }
}
