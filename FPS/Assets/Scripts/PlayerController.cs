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

    public GameObject bulletImpact;
    //public float timeBetweenShots = .1f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 10, /*heatPerShot = 1,*/ collRate = 4, overheatCoolRate = 5;
    private float heatCounter;
    private bool overHeated;

    public Gun[] allGuns;
    private int selectedGun;

    void Start()
    {
        //Lock and hide mouse cursor position to center of the game screen.
        Cursor.lockState = CursorLockMode.Locked;

        charCtrl = GetComponent<CharacterController>();

        cam = Camera.main;

        UIController.instance.overheatedMessage.gameObject.SetActive(false);
        UIController.instance.weaponTempSlider.maxValue = maxHeat;
        UIController.instance.crosshair.SetActive(true);
        
        SwitchGun();
    }

    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        //Horizontal Rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        //Vertical Rotation
        verticalRotStore += mouseInput.y;

        // Limit look up and down angle.
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60, 60);

        // Invert Control
        if (invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y,
                viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,
                viewPoint.rotation.eulerAngles.z);
        }

        // Move Input
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Running
        if (Input.GetKey(KeyCode.LeftShift)) activeMoveSpeed = runSpeed;
        else activeMoveSpeed = moveSpeed;

        //Store yVel before normalized.
        float yVel = movement.y;

        // Transform move vector length change to 1.
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;

        movement.y = yVel;

        if (charCtrl.isGrounded) movement.y = 0;

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

        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;
            
            if (muzzleCounter <= 0) allGuns[selectedGun].muzzleFlash.SetActive(false);
        }

        //Shoot Full Auto with Heat Control.
        if (!overHeated)
        {
            //Shoot at first click.
            if (Input.GetMouseButtonDown(0)) Shoot();

            //Automatic Fire Rate.
            if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
            {
                shotCounter -= Time.deltaTime;
                
                if(shotCounter <= 0) Shoot();
            }

            heatCounter -= collRate * Time.deltaTime;
        }
        else
        {
            heatCounter -= overheatCoolRate * Time.deltaTime;

            if (heatCounter <= 0)
            {
                overHeated = false;
                UIController.instance.overheatedMessage.gameObject.SetActive(false);
                UIController.instance.crosshair.SetActive(true);
            }
        }

        if (heatCounter < 0) heatCounter = 0;

        UIController.instance.weaponTempSlider.value = heatCounter;
        
        //Switch Gun
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            selectedGun++;
            if (selectedGun >= allGuns.Length) selectedGun = 0;
            
            SwitchGun();;
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            selectedGun--;
            if (selectedGun < 0) selectedGun = allGuns.Length - 1;
            
            SwitchGun();
        }

        // Show cursor.
        if (Input.GetKey(KeyCode.LeftAlt)) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;

        //Spawn bullet impact which will last 10 seconds.
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f),
                Quaternion.LookRotation(hit.normal, Vector3.up));
            
            Destroy(bulletImpactObject, 10f);
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;

        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;
            
            UIController.instance.overheatedMessage.gameObject.SetActive(true);
            UIController.instance.crosshair.SetActive(false);
        }
        
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        allGuns[selectedGun].muzzleFlash.transform.rotation = Quaternion.Euler(Random.Range(0, 361) * Vector3.right);
        allGuns[selectedGun].muzzleFlash.transform.localScale = Random.Range(0, 101) * Vector3.one;
        muzzleCounter = muzzleDisplayTime;
    }

    private void SwitchGun()
    {
        foreach (var gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].gameObject.SetActive(true);
        
        allGuns[selectedGun].muzzleFlash.SetActive(false);
    }

    // Camera binds to view point.
    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }
}