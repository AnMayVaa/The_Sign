// PickupItem.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // ถ้าใช้ TextMeshPro ให้เปลี่ยนเป็น using TMPro;

[RequireComponent(typeof(Collider))] // 3D Collider
public class PickupItem : MonoBehaviour
{
    [Header("Item Settings")]
    [Tooltip("ชื่อไอเท็ม เช่น glasses, earmuffs (เช็กแบบไม่สนใจตัวพิมพ์เล็กใหญ่)")]
    public string itemKey = "glasses";

    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("UI บอกให้กดเก็บ (เช่น Canvas + Text)")]
    public GameObject promptUI;

    [Header("After Pickup")]
    public bool destroyOnPickup = true;
    [Tooltip("ถ้าไม่ทำลาย: จะปิด MeshRenderer/Collider แทน")]
    public bool hideInstead = false;

    [Header("Events")]
    public UnityEvent onPicked; // เผื่อไว้ เช่น เล่นเสียง/เอฟเฟกต์

    private bool _playerInRange;

    private void Reset()
    {
        // ทำให้ Collider เป็น Trigger อัตโนมัติ
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        if (promptUI) promptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            _playerInRange = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            _playerInRange = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_playerInRange) return;
        if (Input.GetKeyDown(interactKey))
        {
            // อัปเดตสแตตผู้เล่น
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.SetItem(itemKey, true);

            onPicked?.Invoke();

            if (destroyOnPickup)
            {
                Destroy(gameObject);
            }
            else if (hideInstead)
            {
                // ปิดการมองเห็นและชน
                var col = GetComponent<Collider>();
                if (col) col.enabled = false;

                foreach (var r in GetComponentsInChildren<Renderer>())
                    r.enabled = false;

                if (promptUI) promptUI.SetActive(false);
                enabled = false; // ไม่ต้องเช็กต่อ
            }
            else
            {
                // ไม่ทำลายและไม่ซ่อน: อย่างน้อยปิด prompt
                if (promptUI) promptUI.SetActive(false);
                enabled = false;
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        // วิธี 1: หา Component PlayerStats ในตัวชน
        if (other.GetComponentInParent<PlayerStats>() != null) return true;

        // วิธี 2: เช็ก Tag = "Player"
        return other.CompareTag(PlayerStats.Instance ? PlayerStats.Instance.playerTag : "Player")
            || other.gameObject.CompareTag("Player");
    }
}
