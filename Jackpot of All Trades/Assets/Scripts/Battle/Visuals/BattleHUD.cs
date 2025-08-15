using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetHP(int newHP)
    {
        StopCoroutine(nameof(AnimateHP));
        StartCoroutine(AnimateHP(newHP));
    }

    public void SetShield(int newShield)
    {
        StopCoroutine(nameof(AnimateShield));
        StartCoroutine(AnimateShield(newShield));
    }

    private IEnumerator AnimateHP(int newHP)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        // Get current displayed HP from slider or wand fill amount
        int startHP = 0;

        if (hpSlider != null)
        {
            startHP = Mathf.RoundToInt(hpSlider.value);
        }
        else if (wandCircleHealth != null)
        {
            float fill = wandCircleHealth.fillAmount;
            startHP = Mathf.RoundToInt(fill * maxHP);
        }

        float startFill = wandCircleHealth != null ? wandCircleHealth.fillAmount : 0f;
        float targetFill = (maxHP > 0) ? (float)newHP / maxHP : 0f;

        bool wasDamaged = newHP < startHP;

        if (wasDamaged && wandCircleHealth != null)
            wandCircleHealth.color = Color.red;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Easing.EaseOutCubic(t);

            int current = Mathf.RoundToInt(Mathf.Lerp(startHP, newHP, eased));

            if (hpSlider != null)
                hpSlider.value = current;

            if (hpText != null)
                hpText.text = $"HP: {current}";

            if (wandCircleHealth != null)
                wandCircleHealth.fillAmount = Mathf.Lerp(startFill, targetFill, eased);

            yield return null;
        }

        if (hpSlider != null)
            hpSlider.value = newHP;

        if (hpText != null)
            hpText.text = $"HP: {newHP}";

        if (wandCircleHealth != null)
        {
            wandCircleHealth.fillAmount = targetFill;

            if (wasDamaged)
                wandCircleHealth.color = Color.green;
        }
    }

    private IEnumerator AnimateShield(int newShield)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        float start = shieldSlider.value;
        float end = Mathf.Clamp(newShield, 0, shieldSlider.maxValue);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Easing.EaseOutCubic(t);
            float value = Mathf.Lerp(start, end, eased);

            shieldSlider.value = value;
            shieldText.text = $"SH: {Mathf.RoundToInt(value)}";

            yield return null;
        }

        shieldSlider.value = end;
        shieldText.text = $"SH: {newShield}";
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

        //detect if this is the player
        bool isPlayer = GetComponent<FloatingNumberTracker>() != null;

        controller.Initialize(data.value, data.type, isPlayer ? 0.005f : 1f); // Reduce size for player
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
