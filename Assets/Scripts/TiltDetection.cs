using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltDetection : MonoBehaviour {
    private ChemicalContainer container;
    // Adjustable tilt angle threshold (in degrees)
    public float tiltThreshold = 90.0f;

    // Flag to track whether the bottle is pouring
    private bool isPouring = false;

    public Transform pourLocation;

    private GameObject pourCylinder;
    public GameObject pourCylinderPrefab;

    public float minStreamWidth = 0.2f;
    public float maxStreamWidth = 0.05f;

    private void Start() {
        container = GetComponent<ChemicalContainer>();
    }

    void Update() {
        float tiltAngle = Vector3.Angle(Vector3.down, transform.up); //Up is 180, down is 0
        if (tiltAngle < tiltThreshold && !isPouring && container.GetML() > 0) {
            StartPouring();
        } else if ((tiltAngle >= tiltThreshold && isPouring) || container.GetML() <= 0) {
            StopPouring();
        }

        if (isPouring && pourCylinder != null) {
            ParticleSystem ps = pourCylinder.GetComponentInChildren<ParticleSystem>();

            if (ps && !ps.isPlaying) {
                ps.Play();
            }

            // Rotate the pourCylinder to face downward
            pourCylinder.transform.rotation = Quaternion.LookRotation(Vector3.right);

            // Cast a ray downwards to detect the ground or other surfaces
            RaycastHit hit;
            if (Physics.Raycast(pourLocation.position, Vector3.down, out hit)) {
                // Adjust the pourCylinder's scale so that it reaches the hit point (ground or surface)
                float distanceToGround = hit.distance;

                pourCylinder.transform.localScale = new Vector3(pourCylinder.transform.localScale.x, distanceToGround / 2, pourCylinder.transform.localScale.z);
                pourCylinder.transform.position = (pourLocation.position + hit.point) / 2;
            }

            AdjustStreamWidth(tiltAngle);

            float flowRate = GetFlowRate(tiltAngle);
            float flowRatePower = flowRate * flowRate;
            Tuple<string, float> removedAmount = container.Remove(flowRatePower * container.pourSpeed * Time.deltaTime);
            ChemicalContainer targetContainer = hit.collider.GetComponent<ChemicalContainer>();
            if (targetContainer) {
                float actuallyAddedAmount = targetContainer.Add(removedAmount.Item1, removedAmount.Item2);
                if (actuallyAddedAmount < removedAmount.Item2) {
                    if (ps && !ps.isPlaying) ps.Play();
                } else {
                    if (ps && ps.isPlaying) ps.Stop();
                }
            }
        }
    }

    float GetFlowRate(float tiltAngle) {
        float normalizedTilt = Mathf.InverseLerp(tiltThreshold, 0, tiltAngle);
        return Mathf.Lerp(0, 10, normalizedTilt);
    }

    void AdjustStreamWidth(float tiltAngle) {
        float normalizedTilt = Mathf.InverseLerp(tiltThreshold, 0, tiltAngle);

        float streamWidth = Mathf.Clamp(Mathf.Lerp(minStreamWidth - 0.001f, maxStreamWidth + 0.001f, normalizedTilt), minStreamWidth, maxStreamWidth);

        if (pourCylinder != null) {
            pourCylinder.transform.localScale = new Vector3(streamWidth, pourCylinder.transform.localScale.y, streamWidth);
        }
    }

    void StartPouring() {
        isPouring = true;

        pourCylinder = Instantiate(pourCylinderPrefab, pourLocation.position, Quaternion.identity, pourLocation);
    }

    void StopPouring() {
        isPouring = false;

        Destroy(pourCylinder);
    }
}