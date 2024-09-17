using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PhTestColorer : MonoBehaviour {
    public Renderer testPartRenderer;
    private Material material;

    private void Start() {
        material = testPartRenderer.material;
    }

    public static float CalculatePH(float volumeHClML, float volumeNaOHML) {
        // Convert mL to liters
        float volumeHCl = volumeHClML / 1000.0f;
        float volumeNaOH = volumeNaOHML / 1000.0f;

        // Molarity is assumed to be 1 M for both
        float molesHCl = volumeHCl * 1.0f; // Moles of HCl
        float molesNaOH = volumeNaOH * 1.0f; // Moles of NaOH

        float excessHPlus = molesHCl - molesNaOH;
        float excessOHMinus = molesNaOH - molesHCl;

        float pH;

        if (excessHPlus > 0) {
            // Acidic solution
            float concentrationHPlus = excessHPlus / (volumeHCl + volumeNaOH); // Molarity of H+
            pH = -Mathf.Log10(concentrationHPlus);
        } else if (excessOHMinus > 0) {
            // Basic solution
            float concentrationOHMinus = excessOHMinus / (volumeHCl + volumeNaOH); // Molarity of OH-
            float pOH = -Mathf.Log10(concentrationOHMinus);
            pH = 14.0f - pOH;
        } else {
            // Neutral solution
            pH = 7.0f;
        }

        return pH;
    }

    static Color getColor(int red, int green, int blue) {
        return new Color((float)red / 255.0f, (float)green / 255.0f, (float)blue / 255.0f);
    }

    Color[] loopupTable = {
        getColor(170, 21, 23),
        getColor(228, 32, 42),
        getColor(224, 82, 42),
        getColor(243, 112, 30),
        getColor(225, 135, 12),
        getColor(224, 192, 11),
        getColor(201, 157, 60),
        getColor(159, 135, 87),
        getColor(159, 131, 83),
        getColor(80, 98, 74),
        getColor(64, 85, 70),
        getColor(86, 66, 166),
        getColor(53, 42, 110),
        getColor(52, 29, 75),
        getColor(42, 25, 35)
        };

    private void OnCollisionStay(Collision collision) {
        ChemicalContainer container = collision.gameObject.GetComponent<ChemicalContainer>();
        if (container) {
            float NaOH = container.GetContentML("NaOH");
            float HCl = container.GetContentML("HCl");
            float pH = CalculatePH(HCl, NaOH);

            material.SetColor("_BaseColor", loopupTable[Mathf.RoundToInt(pH)]);
        }
    }
}
