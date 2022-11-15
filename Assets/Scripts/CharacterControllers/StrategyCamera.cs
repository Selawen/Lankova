using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyCamera : MonoBehaviour
{
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    public float moveSpeed, zoomSpeed, zoomMin, zoomMax;
    public bool zoomOnScroll = true;

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKey(forward))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(left))
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(right))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(back))
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }

        if (zoomOnScroll)
        {
            float newY = Mathf.Clamp(transform.position.y - Input.mouseScrollDelta.y * zoomSpeed, zoomMin, zoomMax);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
