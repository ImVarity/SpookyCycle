using UnityEngine;
using UnityEngine.InputSystem; // new input system

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 6f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float jumpHeight = 1.2f;

    [Header("Look")]
    [SerializeField] Transform cam;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float maxPitch = 85;

    [Header("Input (assign from Input Actions asset")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference lookAction;
    [SerializeField] InputActionReference jumpAction;

    [SerializeField] InputActionReference sprintAction;
    [SerializeField] float sprintMultiplier = 1.8f;

    CharacterController cc;
    float pitch;
    float velY;

    // Use Awake instead of Start because we want this to run before any Start methods
    void Awake()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cam == null) Debug.LogWarning("Assign the Camera Transform to 'cam'.");
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
    }


    // Update is called once per frame
    void Update()
    {
        // look
        if (cam != null)
        {
            Vector2 look = lookAction.action.ReadValue<Vector2>() * mouseSensitivity;
            transform.Rotate(0f, look.x, 0f);
            pitch = Mathf.Clamp(pitch - look.y, -maxPitch, maxPitch);
            cam.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        // move
        Vector2 move2 = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * move2.x + transform.forward * move2.y;
        if (move.sqrMagnitude > 1f) move.Normalize();

        bool isSprinting = sprintAction.action.IsPressed();
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        if (sprintAction.action.IsPressed() && Time.frameCount % 10 == 0)
        Debug.Log("Sprinting held");


        // jump + gravity
        if (cc.isGrounded)
        {
            velY = -2f; // small downward force to keep grounded
            if (jumpAction.action.WasPressedThisFrame())
                velY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velY += gravity * Time.deltaTime;

        Vector3 velocity = move * currentSpeed + Vector3.up * velY;
        cc.Move(velocity * Time.deltaTime);
    }
}
