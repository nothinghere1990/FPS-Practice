using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5;
    private Vector3 moveDir, movement;

    public CharacterController charCtrl; //include rigidbody and collision
    
    void Start()
    {
        //Lock mouse cursor position to center of the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        
        charCtrl = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        
        //horizontal rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        //vertical rotation
        verticalRotStore += mouseInput.y;
        
        // Limit look up and down angle.
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60, 60);
        
        // invert control
        if (invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        // move input
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // Move vector length change to 1.
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        charCtrl.Move(movement * moveSpeed * Time.deltaTime);
        
        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
