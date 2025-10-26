using UnityEngine;
using UnityEngine.InputSystem; // 1. Import ระบบ Input ใหม่

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))] // 2. บังคับให้มี PlayerInput
public class FPSController : MonoBehaviour
{
    [Header("Player Movement")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float jumpHeight = 2.0f;

    [Header("Camera Look")]
    [SerializeField] private Transform playerCamera;
    public float mouseSensitivity = 2.0f;
    public float verticalLookLimit = 80.0f;

    [Header("Physics")]
    public float gravity = 20.0f;

    // ตัวแปรภายใน
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float xRotation = 0.0f;

    // 3. ตัวแปรสำหรับเก็บ Actions
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    // ใช้ Awake แทน Start สำหรับการตั้งค่า Input
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>(); // ดึง PlayerInput component

        // 4. ค้นหา Actions จากชื่อที่เราตั้งไว้ (ต้องตรงกับใน Asset)
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];

        // ซ่อนและล็อคเมาส์
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
        }
    }

    // 5. เปิด/ปิดการใช้งาน Actions
    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    void Update()
    {
        // โค้ดที่เหลือเหมือนเดิม แต่จะไปดึงค่าจาก Actions แทน
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        // 6. อ่านค่าจาก Actions (แทน Input.GetAxis)
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isSprinting = sprintAction.IsPressed(); // .IsPressed() เหมือน .GetKey()
        
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        inputDirection = transform.TransformDirection(inputDirection);
        inputDirection.Normalize();

        if (isGrounded)
        {
            moveDirection = inputDirection * currentSpeed;

            // 7. ใช้ .triggered (เหมือน .GetButtonDown())
            if (jumpAction.triggered)
            {
                moveDirection.y = Mathf.Sqrt(2 * jumpHeight * gravity);
            }
        }
        else
        {
            // Air control
            moveDirection.x = inputDirection.x * currentSpeed;
            moveDirection.z = inputDirection.z * currentSpeed;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void HandleLook()
    {
        // 8. อ่านค่าเมาส์จาก Action
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // (สำคัญ: ไม่ต้องคูณ Time.deltaTime ที่นี่ เพราะ Delta Mouse คือค่า "ต่อเฟรม" อยู่แล้ว)
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}