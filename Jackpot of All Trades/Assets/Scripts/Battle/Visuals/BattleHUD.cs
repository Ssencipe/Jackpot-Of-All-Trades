using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private TextMeshProUGUI shieldText;
    [SerializeField] private GameObject floatingNumberPrefab;
    [SerializeField] private Image wandCircleHealth;

    [Header("Status Effects")]
    [SerializeField] private Transform statusContainer;
    [SerializeField] private GameObject statusIcon;
    private StatusEffectController statusController;
    private List<GameObject> iconPool = new();
    private int maxHP;

    private Dictionary<FloatingNumberType, FloatingNumberController> floatingCache = new();


    //for player
    public void Bind(Unit unit)
    {
        if (unit == null) return;

        // Unbind previous events
        unit.OnHealthChanged -= SetHP;
        unit.OnShieldChanged -= SetShield;
        unit.OnFloatingNumber -= SpawnFloatingNumber;

        // Only apply shield setup — HP slider is obsolete for player
        shieldSlider.maxValue = unit.maxHP;
        maxHP = unit.maxHP;
        SetShield(unit.currentShield);

        BindStatus(unit.StatusEffects);

        // Rebind events
        unit.OnHealthChanged += SetHP;
        unit.OnShieldChanged += SetShield;
        unit.OnFloatingNumber += SpawnFloatingNumber;

        // Set HP without slider (wand-only)
        SetHP(unit.currentHP);
    }

    //for enemy
    public void Bind(BaseEnemy enemy)
    {
        if (enemy == null) return;

        // Unbind previous events (prevents multiple subscriptions)
        enemy.OnHealthChanged -= SetHP;
        enemy.OnShieldChanged -= SetShield;
        enemy.OnFloatingNumber -= SpawnFloatingNumber;

        hpSlider.maxValue = enemy.runtimeData.maxHealth;
        shieldSlider.maxValue = enemy.runtimeData.maxHealth;

        SetHP(enemy.currentHP);
        SetShield(enemy.currentShield);

        BindStatus(enemy.StatusEffects);

        //Bind new instances of events
        enemy.OnHealthChanged += SetHP;
        enemy.OnShieldChanged += SetShield;
        enemy.OnFloatingNumber += SpawnFloatingNumber;
    }

    //set up status effect references
    private void BindStatus(StatusEffectController controller)
    {
        if (statusController != null)
            statusController.OnEffectsChanged -= UpdateStatusUI;

        statusController = controller;

        if (statusController != null)
        {
            statusController.OnEffectsChanged += UpdateStatusUI;
            UpdateStatusUI();
        }
    }

    public void SetHP(int hp)
    {
        if (hpText != null)
            hpText.text = $"HP: {hp}";

        // Only update slider if it's used (e.g. enemies)
        if (hpSlider != null)
            hpSlider.value = hp;

        if (wandCircleHealth != null)
        {
            float fill = (maxHP > 0) ? (float)hp / maxHP : 0f;
            wandCircleHealth.fillAmount = fill;
        }
    }

    public void SetShield(int shield)
    {
        shieldSlider.value = Mathf.Clamp(shield, 0, shieldSlider.maxValue);
        shieldText.text = $"SH: {shield}";
    }

    //the numbers that appear when damaged/healing/shielding
    private void SpawnFloatingNumber(FloatingNumberData data)
    {
        if (floatingNumberPrefab == null)
        {
            Debug.LogWarning("[BattleHUD] FloatingNumber prefab not assigned.");
            return;
        }

        if (floatingCache.TryGetValue(data.type, out var existing) && existing != null)
        {
            existing.AddValue(data.value);
            return;
        }

        GameObject floating = Instantiate(floatingNumberPrefab, transform);
        var controller = floating.GetComponentInChildren<FloatingNumberController>();
        if (controller == null)
        {
            Debug.LogError("[BattleHUD] FloatingNumber prefab is missing FloatingNumberController!");
            return;
        }

        Debug.Log($"Floating number [{data.type}] +{data.value} spawned for: {gameObject.name}");

        controller.Initialize(data.value, data.type);
        floatingCache[data.type] = controller;

        controller.OnDestroyed += (t) => floatingCache.Remove(t);
    }

    //update status effect icons
    private void UpdateStatusUI()
    {
        // Clear old
        foreach (var icon in iconPool) Destroy(icon);
        iconPool.Clear();

        if (statusController == null) return;

        foreach (var effect in statusController.ActiveEffects)
        {
            GameObject iconGO = Instantiate(statusIcon, statusContainer);
            iconPool.Add(iconGO);

            var img = iconGO.GetComponent<Image>();
            var txt = iconGO.GetComponentInChildren<TextMeshPro>();

            if (img != null)
                img.sprite = effect.Icon;
            else
                Debug.LogWarning("[BattleHUD] No Image found in status icon prefab!");

            if (txt != null)
                txt.text = effect.Duration.ToString();
            else
                Debug.LogWarning("[BattleHUD] No TextMeshProUGUI found in status icon prefab!");

            //Status tooltip setup
            var trigger = iconGO.AddComponent<StatusTooltipUI>();
            trigger.Initialize(effect);
        }
    }
}
