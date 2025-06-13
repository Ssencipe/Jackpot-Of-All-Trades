using System.Collections.Generic;
using UnityEngine;

public class FloatingNumberTracker : MonoBehaviour
{
    public GameObject floatingNumberPrefab;
    private Dictionary<FloatingNumberType, FloatingNumberController> activeNumbers = new();


    //adds together values of same type floating numbers
    public void ShowNumber(int value, FloatingNumberType type)
    {
        if (activeNumbers.TryGetValue(type, out var existing) && existing != null)
        {
            existing.AddValue(value);
            return;
        }

        GameObject go = Instantiate(floatingNumberPrefab, transform);
        var controller = go.GetComponent<FloatingNumberController>();
        controller.Initialize(value, type);
        activeNumbers[type] = controller;

        controller.OnDestroyed += (t) => activeNumbers.Remove(t);
    }
}