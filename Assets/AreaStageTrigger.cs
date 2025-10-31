// Assets/Scripts/AreaStageTrigger.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("Game/Area Stage Trigger")]
public class AreaStageTrigger : MonoBehaviour
{
    [Tooltip("ลาก EventController ในซีนมาใส่")]
    public EventController controller;   // <— ช่องนี้จะโผล่ใน Inspector แน่นอน

    [Tooltip("Tag ผู้เล่น")]
    public string playerTag = "Player";

    [Header("ทำครั้งเดียว")]
    [Tooltip("ผ่านขั้นสำเร็จแล้ว ปิดโซนนี้ไปเลย")]
    public bool deactivateAfterSuccess = true;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; // โซนล่องหน
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controller == null) return;
        if (!other.CompareTag(playerTag)) return;

        bool passed = controller.TryActivateCurrentStep();

        if (passed && deactivateAfterSuccess)
        {
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
            enabled = false;
        }
    }
}
