using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float mouseXInterp = 0.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Convert mouse position on screen to normalized center coordinates
        Vector2 mousePos = Input.mousePosition / new Vector2(Screen.width, Screen.height);

        mousePos -= new Vector2(0.5f, 0.5f);
        mousePos *= 2.0f;
        mousePos.x = Mathf.Clamp(mousePos.x, -1.0f, 1.0f);
        mousePos.y = Mathf.Clamp(mousePos.y, -1.0f, 1.0f);

        mouseXInterp = Mathf.Lerp(mouseXInterp, mousePos.x, 3.0f * Time.deltaTime);

        transform.rotation = Quaternion.AngleAxis(10 * mouseXInterp, Vector3.up);
    }
}
