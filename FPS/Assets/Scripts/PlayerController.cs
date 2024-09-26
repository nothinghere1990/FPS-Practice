using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5, runSpeed = 8;
    public float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCtrl; //include rigidbody and collision

    private Camera cam;

    public float jumpForce = 12, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    public bool isGrounded;
    public LayerMask groundLayers;
    
    void Start()
    {
        //Lock and hide mouse cursor position to center of the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        
        charCtrl = GetComponent<CharacterController>();
        
        cam = Camera.main;
    }
    
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        
        //Horizontal Rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        //Vertical Rotation
        verticalRotStore += mouseInput.y;
        
        // Limit look up and down angle.
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60, 60);
        
        // Invert Control
        if (invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        // Move Input
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        // Running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }

        float yVel = movement.y;
        
        // Transform move vector length change to 1.
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;

        movement.y = yVel;

        if (charCtrl.isGrounded)
        {
            movement.y = 0;
        }
        
        // Ground Raycast
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .5f, groundLayers);
        Debug.DrawRay(groundCheckPoint.position, Vector3.down * .5f, Color.green);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("jump");
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;
        
        charCtrl.Move(movement * Time.deltaTime);
        
        // Show cursor.
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Camera binds to view point.
    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }
}
