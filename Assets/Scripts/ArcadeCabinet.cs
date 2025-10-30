using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeCabinet : MonoBehaviour
{
    // --- เพิ่ม 2 ช่องนี้ ---
    [Tooltip("จุดที่กล้อง Player จะซูมไป (สร้าง Empty Object แล้วลากมาใส่)")]
    public Transform cameraZoomTarget;
    public MonoBehaviour game2DController; // ลากสคริปต์ที่ใช้ควบคุมเกม 2D มาใส่
    
    private FPSController activePlayer;
    private PlayerInput playerInput;
    private InputAction exitAction;

    // --- เพิ่มตัวแปรสำหรับจำตำแหน่งเดิม ---
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private CharacterController playerCharacterController; // ตัวควบคุม 3D

    public void StartPlaying(FPSController player)
    {
        activePlayer = player;
        playerInput = player.GetComponent<PlayerInput>();

        // 1. --- โค้ดส่วนวาร์ปกล้อง (เพิ่มเข้ามา) ---
        // จำค่า CharacterController
        playerCharacterController = activePlayer.GetComponent<CharacterController>();
        
        // จำตำแหน่งและมุมกล้องเดิมของ Player
        originalPlayerPosition = activePlayer.transform.position;
        originalPlayerRotation = activePlayer.transform.rotation;

        // ปิด CharacterController ชั่วคราว (สำคัญมาก!)
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = false;
        }

        // วาร์ป Player ไปที่จุดซูม
        if (cameraZoomTarget != null)
        {
            activePlayer.transform.position = cameraZoomTarget.position;
            activePlayer.transform.rotation = cameraZoomTarget.rotation;
        }
        // ------------------------------------

        // 2. บอก FPSController ให้หยุดทำงาน (เหมือนเดิม)
        activePlayer.isPlayingArcade = true;

        // 3. สลับ Action Map (เหมือนเดิม)
        playerInput.SwitchCurrentActionMap("ArcadeGame");
        Debug.Log("Switched to ArcadeGame Map");

        // 4. เปิดสคริปต์เกม 2D (เหมือนเดิม)
        if (game2DController != null)
        {
            Debug.Log("Enabling 2D Game Controller");
            game2DController.enabled = true;
        }

        // 5. เปิดการดักฟังปุ่ม Exit (เหมือนเดิม)
        exitAction = playerInput.actions["Exit"];
        exitAction.Enable();
        exitAction.performed += OnExitGame; 
    }

    private void OnExitGame(InputAction.CallbackContext context)
    {
        Debug.Log("Exiting Arcade Game");

        // 1. --- โค้ดส่วนวาร์ปกลับ (เพิ่มเข้ามา) ---
        // วาร์ป Player กลับที่เดิม
        activePlayer.transform.position = originalPlayerPosition;
        activePlayer.transform.rotation = originalPlayerRotation;

        // เปิด CharacterController คืน (สำคัญมาก!)
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = true;
        }
        // ------------------------------------

        // 2. บอก FPSController ให้กลับมาทำงาน (เหมือนเดิม)
        activePlayer.isPlayingArcade = false;

        // 3. สลับ Action Map กลับ (เหมือนเดิม)
        playerInput.SwitchCurrentActionMap("Player");

        // 4. ปิดสคริปต์เกม 2D (เหมือนเดิม)
        if (game2DController != null)
        {
            game2DController.enabled = false;
        }

        // 5. ปิดการดักฟังปุ่ม Exit (เหมือนเดิม)
        exitAction.performed -= OnExitGame;
        exitAction.Disable();
        
        activePlayer = null;
    }
}