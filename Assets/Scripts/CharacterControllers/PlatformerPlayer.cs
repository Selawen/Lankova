using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayer : MonoBehaviour
{
    public static PlatformerPlayer Instance { get; private set; }

    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    public KeyCode jump = KeyCode.W;

    public float moveSpeed;
    public float jumpForce;
    public bool grounded;

    Rigidbody rb;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if(Input.GetKeyDown(left))
        {
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }

        if(Input.GetKeyDown(right))
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }

        if(Input.GetKey(left) || Input.GetKey(right))
        {
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
        }

        if(Input.GetKeyUp(left) && Input.GetKey(right))
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0);

        }

        if (Input.GetKeyUp(right) && Input.GetKey(left))
        {
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }

        if (Input.GetKeyDown(jump) && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground")) { grounded = true; }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground")) { grounded = false; }
    }
}
