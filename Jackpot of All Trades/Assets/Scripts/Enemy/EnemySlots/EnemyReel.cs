using UnityEngine;
using System.Collections;

public class EnemyReel : MonoBehaviour
{
    [Header("Spells")]
    public RuntimeSpell[] availableSpells;

    [Header("Spin Settings")]
    public float minSpinDuration = 2f;
    public float maxSpinDuration = 3f;
    public float minSpinSpeed = 100f;
    public float maxSpinSpeed = 400f;

    [Header("Visuals")]
    public EnemyReelVisual visual;

    private bool isSpinning = false;

    private void Start()
    {
        RandomizeStart();
    }

    public void RandomizeStart()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;
        visual?.InitializeVisuals(availableSpells);
    }

    public void Spin()
    {
        if (isSpinning || availableSpells == null || availableSpells.Length == 0) return;
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        float spinDuration = Random.Range(minSpinDuration, maxSpinDuration);
        yield return StartCoroutine(visual.ScrollSpells(spinDuration, minSpinSpeed, maxSpinSpeed, availableSpells));

        isSpinning = false;
    }

    public RuntimeSpell GetCenterSpell()
    {
        return visual?.GetCenterSpell();
    }

    public bool IsSpinning()
    {
        return isSpinning;
    }
}
