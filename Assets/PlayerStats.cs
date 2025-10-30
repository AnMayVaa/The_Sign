// PlayerStats.cs
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Owned Items (flags)")]
    public bool hasGlasses;   // แว่น
    public bool hasEarmuffs;  // ที่ปิดหู

    [Header("Optional")]
    [Tooltip("Tag ของ Player object (ใช้ช่วยให้ Pickup หา Player เจอ)")]
    public string playerTag = "Player";

    private void Awake()
    {
        // ทำเป็น Singleton แบบง่าย ๆ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // ถ้าอยากให้คงอยู่ข้ามซีน (เลือกได้)
        // DontDestroyOnLoad(gameObject);

        LoadFromPrefs();
    }

    // เรียกด้วยชื่อ itemKey (ไม่สนใจตัวพิมพ์เล็กใหญ่)
    public void SetItem(string itemKey, bool owned = true)
    {
        if (string.IsNullOrWhiteSpace(itemKey)) return;

        string k = itemKey.Trim().ToLowerInvariant();

        if (k == "glasses" || k == "แว่น")
            hasGlasses = owned;
        else if (k == "earmuffs" || k == "ที่ปิดหู")
            hasEarmuffs = owned;

        // เซฟสถานะ
        PlayerPrefs.SetInt(PrefKey(k), owned ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool HasItem(string itemKey)
    {
        if (string.IsNullOrWhiteSpace(itemKey)) return false;
        string k = itemKey.Trim().ToLowerInvariant();

        return k switch
        {
            "glasses" or "แว่น" => hasGlasses,
            "earmuffs" or "ที่ปิดหู" => hasEarmuffs,
            _ => PlayerPrefs.GetInt(PrefKey(k), 0) == 1 // เผื่ออนาคตมีไอเท็มใหม่
        };
    }

    private void LoadFromPrefs()
    {
        hasGlasses = PlayerPrefs.GetInt(PrefKey("glasses"), 0) == 1
                  || PlayerPrefs.GetInt(PrefKey("แว่น"), 0) == 1;
        hasEarmuffs = PlayerPrefs.GetInt(PrefKey("earmuffs"), 0) == 1
                   || PlayerPrefs.GetInt(PrefKey("ที่ปิดหู"), 0) == 1;
    }

    private string PrefKey(string key) => $"item_{key}";
}
