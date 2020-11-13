using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour
{
    private int speed = 30;
    private float zoom = 4.4f;
    private float zoomSpeed = 4.0f;
    private float panSpeed = 1f;
    private Vector3 originalPosition;
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current[Key.Q].isPressed)
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
            //transform.Rotate(0f, 20.0f, 0f);
        }else if (Keyboard.current[Key.E].isPressed)
        {
            transform.Rotate(Vector3.up * -1 * speed * Time.deltaTime);
        }else if (Keyboard.current[Key.R].isPressed)
        {
            zoom -= Time.deltaTime * zoomSpeed;
        }else if (Keyboard.current[Key.F].isPressed)
        {
            zoom += Time.deltaTime * zoomSpeed;
        }else if (Keyboard.current[Key.W].isPressed)
        {
            transform.position += transform.forward * (Time.deltaTime * panSpeed * zoom * zoom);
        }else if (Keyboard.current[Key.S].isPressed)
        {
            transform.position -= transform.forward * (Time.deltaTime * panSpeed * zoom * zoom);
        }else if (Keyboard.current[Key.A].isPressed)
        {
            transform.position -= transform.right * (Time.deltaTime * panSpeed * zoom * zoom);
        }else if (Keyboard.current[Key.D].isPressed)
        {
            transform.position += transform.right * (Time.deltaTime * panSpeed * zoom * zoom);
        }
        
        
        camera.transform.localPosition = new Vector3(0, 1, -1) * zoom * zoom;
    }

    public void SetCameraOrigin()
    {
        //transform.position = originalPosition;
    }

    public void SetCameraOrigin(Transform character)
    {
        transform.position = character.position;
    }
} 