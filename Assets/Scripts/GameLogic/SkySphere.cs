/*
    SkySphere
    - This moves the sphere with a sky texture around to simulate movement
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySphere : MonoBehaviour
{


    public float speed = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, (Time.deltaTime * speed * 0.1f));
    }
}
