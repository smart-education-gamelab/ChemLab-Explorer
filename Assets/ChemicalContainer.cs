using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using AYellowpaper.SerializedCollections;

[Serializable]
struct ChemicalContainerContents {
    public string name;
    public float ml;
}

public class ChemicalContainer : MonoBehaviour {
    [SerializedDictionary("Name", "ML")]
    public SerializedDictionary<string, float> contents;
    public float pourSpeed = 10.0f;
    public float maxCapacity;
    public GameObject contentsVisual;
    public TextMeshProUGUI tooltipTex;
    private float originalContentsHeight, originalContentsY;

    private void Start() {
        originalContentsHeight = contentsVisual.transform.localScale.y;
        originalContentsY = contentsVisual.transform.localPosition.y;
        UpdateContentsVisual();
    }

    public float GetContentML(string name) {
        if (contents.TryGetValue(name, out float value))
            return value;
        return 0;
    }

    private string GetContentsName() {
        string name = "";
        foreach (KeyValuePair<string, float> kvp in contents) {
            name += kvp.Key + ": " + kvp.Value.ToString("0.000") + "mL\n";
        }
        return name;
    }

    public float GetML() {
        return GetTotalML();
    }

    public float Add(string name, float amount) {
        if(GetTotalML() + amount > maxCapacity) {
            float remaining = maxCapacity - GetTotalML();
            contents[name] += remaining;
            UpdateContentsVisual();
            return remaining;
        }
        if (contents.ContainsKey(name)) {
            contents[name] += amount;
        } else {
            contents.Add(name, amount);
        }
        UpdateContentsVisual();
        return amount;
    }

    //Returns actual amount removed
    public Tuple<string, float> Remove(float amount) {
        if (contents.Count > 0) {

            KeyValuePair<string, float> kvp = contents.First();
            if (kvp.Value > amount) {
                contents[kvp.Key] = kvp.Value - amount;
                UpdateContentsVisual();
                return new Tuple<string, float>(kvp.Key, amount);
            } else {
                contents.Remove(kvp.Key);
                UpdateContentsVisual();
                return new Tuple<string, float>(kvp.Key, kvp.Value);
            }
        }

        UpdateContentsVisual();
        return null;
    }

    private float GetTotalML() {
        float total = 0;
        foreach (KeyValuePair<string, float> kvp in contents) {
            total += kvp.Value;
        }
        return total;
    }

    private void UpdateContentsVisual() {
        float totalML = GetTotalML();
        if (totalML == 0) {
            contentsVisual.SetActive(false);
        } else {
            contentsVisual.SetActive(true);
        }
        // Calculate the new scale for the contentsVisual based on the current ml and maxCapacity
        float newHeightScale = (totalML / maxCapacity) * originalContentsHeight;
        Vector3 newScale = new Vector3(contentsVisual.transform.localScale.x, newHeightScale, contentsVisual.transform.localScale.z);
        contentsVisual.transform.localScale = newScale;

        // Calculate the new position for the contentsVisual so it moves to the bottom of the container
        float newYPosition = originalContentsY - (originalContentsHeight - newHeightScale);
        Vector3 newPosition = new Vector3(contentsVisual.transform.localPosition.x, newYPosition, contentsVisual.transform.localPosition.z);
        contentsVisual.transform.localPosition = newPosition;

        if (tooltipTex != null) tooltipTex.text = GetContentsName();
    }
}
