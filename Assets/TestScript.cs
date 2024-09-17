using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public GameObject obj1, obj2;
    private void Start() {
        Destroy(obj1);
    }

    private void Update() {
        print(obj2.name);
    }
}
