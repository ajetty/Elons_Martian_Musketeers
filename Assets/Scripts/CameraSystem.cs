using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour
{
    private int speed = 30;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current[Key.A].isPressed)
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
            //transform.Rotate(0f, 20.0f, 0f);
        }
    }
} 