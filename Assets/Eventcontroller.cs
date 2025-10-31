// Assets/Scripts/EventController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventStep
{
    [Tooltip("เช่น ActiveEvent1, ActiveEvent2")]
    public string stepId = "ActiveEvent1";

    [Tooltip("ต้องมีไอเท็มอะไรถึงจะผ่านขั้นนี้ (เว้นว่าง = ไม่ต้องใช้ไอเท็ม)")]
    public string requiredItemKey = "";

    [Tooltip("ฟังก์ชันที่จะเรียกเมื่อผ่านขั้นนี้ (ลากใน Inspector)")]
    public UnityEvent onActivated;   // <— ช่องนี้จะโผล่ใน Inspector แน่นอน

    [HideInInspector] public bool completed;
}

[AddComponentMenu("Game/Event Controller (Sequential)")]
public class EventController : MonoBehaviour
{
    [Header("เรียงขั้นตามลำดับ (แก้ใน Inspector)")]
    public List<EventStep> steps = new List<EventStep>
    {
        new EventStep { stepId = "ActiveEvent1", requiredItemKey = "glasses"  },
        new EventStep { stepId = "ActiveEvent2", requiredItemKey = "earmuffs" },
    };

    [SerializeField, Tooltip("เริ่มตรวจจากขั้นที่เท่าไหร่ (ปกติ 0)")]
    private int currentIndex = 0;

    public int CurrentIndex => currentIndex;
    public bool IsAllDone => currentIndex >= steps.Count;

    /// <summary>
    /// พยายามผ่าน "ขั้นปัจจุบัน"
    /// </summary>
    public bool TryActivateCurrentStep()
    {
        if (IsAllDone) return false;

        var step = steps[currentIndex];

        // ตรวจไอเท็มถ้ามีระบุ
        if (!string.IsNullOrWhiteSpace(step.requiredItemKey))
        {
            if (PlayerStats.Instance == null) return false;
            if (!PlayerStats.Instance.HasItem(step.requiredItemKey)) return false;
        }

        // ผ่านขั้นนี้
        step.completed = true;

        // (ออปชัน) แจ้งสัญญาณออกไป ถ้ามี GameSignals
#if GAME_SIGNALS
        GameSignals.SetEventFlag(step.stepId, true);
#endif

        // เรียก UnityEvent ที่ลากไว้
        step.onActivated?.Invoke();

        // ไปขั้นถัดไป
        currentIndex++;
        return true;
    }
}
