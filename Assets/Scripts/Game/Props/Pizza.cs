using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
    {
        public float rotationSpeed = 30f;
    
    void Update()
    {
        RotatePizza();
    }

    private void RotatePizza()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
}
