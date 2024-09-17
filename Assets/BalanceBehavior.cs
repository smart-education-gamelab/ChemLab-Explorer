using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BalanceBehavior : MonoBehaviour {
    private DateTime lastVisualUpdate = DateTime.Now;
    private const float updateInterval = 0.01666667f;
    public int speedFactor = 2;

    private float balanceValueVisual;
    private float balanceValue;
    public TextMeshProUGUI balanceText;

    List<ChemicalContainer> chemicalContainers = new List<ChemicalContainer>();
    private void Start() {
        balanceText.text = balanceValueVisual.ToString("0.0000");
    }

    private void OnTriggerEnter(Collider other) {
        ChemicalContainer container = other.GetComponent<ChemicalContainer>();
        if (container != null) {
            chemicalContainers.Add(container);
        }
    }

    private void OnTriggerExit(Collider other) {
        ChemicalContainer container = other.GetComponent<ChemicalContainer>();
        if (container != null) {
            chemicalContainers.Remove(container);
        }
    }

    public void UpdateVisualPoints() {
        DateTime now = DateTime.Now;
        if ((now - lastVisualUpdate).TotalSeconds > updateInterval) {
            float difference = Math.Abs(balanceValueVisual - balanceValue);
            float changeAmount = difference / speedFactor;

            if (balanceValueVisual < balanceValue) {
                balanceValueVisual = Math.Min(balanceValueVisual + changeAmount, balanceValue);
            } else if (balanceValueVisual > balanceValue) {
                balanceValueVisual = Math.Max(balanceValueVisual - changeAmount, balanceValue);
            }

            lastVisualUpdate = now;
            balanceText.text = balanceValueVisual.ToString("0.0000");
        }
    }

    private void Update() {
        balanceValue = 0;
        foreach (ChemicalContainer container in chemicalContainers) {
            balanceValue += container.GetML();
        }
        UpdateVisualPoints();
    }
}
