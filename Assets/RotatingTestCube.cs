using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTestCube : MonoBehaviour
{
    public float speed = 0.1f;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(speed, speed, speed));
    }
}
