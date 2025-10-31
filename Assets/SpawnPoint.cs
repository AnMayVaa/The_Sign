// SceneEventController.cs
using UnityEngine;

public class SceneEventController : MonoBehaviour
{
    [Header("Player & Start Point")]
    [SerializeField] private Transform player;         // ลาก Player มาวาง หรือปล่อยว่างให้หา tag = Player
    [SerializeField] private Transform startPoint;     // จุดเริ่มเกม (ตั้งตำแหน่ง/หมุนได้ในซีน)
    [SerializeField] private bool applyStartRotation = true;

    void Awake()
    {
        // ถ้าไม่ได้อ้างอิง player ไว้ ให้ลองหาในซีนด้วย Tag
        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
        }

        // เทเลพอร์ตผู้เล่นไปจุดเริ่มเกม
        if (player && startPoint)
        {
            TeleportTo(player, startPoint.position, applyStartRotation ? startPoint.rotation : player.rotation);
        }

        // ตรงนี้เผื่อไว้สำหรับ init อื่น ๆ ในซีน (ระบบเซฟ/โหลด, ลงทะเบียนอีเวนต์ ฯลฯ)
        // Example: SaveSystem.LoadOrNew();  // ถ้าคุณมีระบบเซฟแล้วค่อยเปิดใช้
    }

    /// <summary>
    /// เทเลพอร์ตอย่างปลอดภัย (รองรับ CharacterController / Rigidbody)
    /// </summary>
    private void TeleportTo(Transform target, Vector3 pos, Quaternion rot)
    {
        if (!target) return;

        // ถ้ามี CharacterController ปิดก่อนกันแรงชน
        var cc = target.GetComponent<CharacterController>();
        if (cc)
        {
            bool wasEnabled = cc.enabled;
            cc.enabled = false;
            target.SetPositionAndRotation(pos, rot);
            cc.enabled = wasEnabled;
            return;
        }

        // ถ้ามี Rigidbody ใช้ MovePosition/MoveRotation เพื่อความนุ่มนวล
        var rb = target.GetComponent<Rigidbody>();
        if (rb && !rb.isKinematic)
        {
            rb.position = pos;
            rb.rotation = rot;
        }
        else if (rb && rb.isKinematic)
        {
            rb.MovePosition(pos);
            rb.MoveRotation(rot);
        }
        else
        {
            target.SetPositionAndRotation(pos, rot);
        }
    }

#if UNITY_EDITOR
    // ช่วยให้เห็นจุดเริ่มเกมใน Scene View
    void OnDrawGizmosSelected()
    {
        if (!startPoint) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPoint.position, 0.3f);
        Gizmos.DrawLine(startPoint.position, startPoint.position + startPoint.forward * 1.0f);
    }
#endif
}
