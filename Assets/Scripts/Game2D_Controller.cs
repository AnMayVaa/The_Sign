using UnityEngine;
using UnityEngine.InputSystem;

public class Game2D_Controller : MonoBehaviour
{
    // เราต้องเข้าถึง PlayerInput ที่อยู่บนตัว Player 3D
    private PlayerInput playerInput;
    private InputAction move2DAction;
    private InputAction jump2DAction;

    // สคริปต์นี้จะถูกเปิดโดย ArcadeCabinet
    void OnEnable()
    {
        // ค้นหา PlayerInput ที่มีอยู่ใน Scene (ปกติมีอันเดียว)
        playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput != null && playerInput.currentActionMap.name == "ArcadeGame")
        {
            move2DAction = playerInput.actions["Move2D"];
            jump2DAction = playerInput.actions["Jump2D"];
        } 
        else
        {
            Debug.LogError("Could not find PlayerInput or wrong Action Map!");
        }
    }

    void Update()
    {
        if (move2DAction == null) return;

        // อ่านค่าจาก Action Map "ArcadeGame"
        Vector2 moveInput = move2DAction.ReadValue<Vector2>();

        // ขยับตัวละคร 2D
        transform.Translate(new Vector3(moveInput.x, moveInput.y, 0) * 5.0f * Time.deltaTime);

        if (jump2DAction.triggered)
        {
            Debug.Log("2D Jump!");
            // ใส่โค้ดกระโดด 2D
        }
    }

    // เมื่อสคริปต์นี้ถูกปิด (ตอนเลิกเล่น)
    void OnDisable()
    {
        move2DAction = null;
        jump2DAction = null;
    }
}