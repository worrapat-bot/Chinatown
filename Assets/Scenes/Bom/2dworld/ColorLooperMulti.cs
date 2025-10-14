using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLooperMulti : MonoBehaviour
{
    [Header("Targets (เลือกได้หลายชิ้น)")]
    [Tooltip("ใส่ Graphic (Image / Text / RawImage ฯลฯ) ที่อยากเปลี่ยนสีหลาย ๆ ตัวได้")]
    public List<Graphic> uiTargets = new List<Graphic>();

    [Tooltip("เพิ่ม SpriteRenderer หลายตัวได้ด้วย (ถ้าเป็นสไปรต์)")]
    public List<SpriteRenderer> spriteTargets = new List<SpriteRenderer>();

    [Tooltip("Renderer ที่มีพารามิเตอร์ _Color (สำหรับ MeshRenderer/SkinnedMeshRenderer)")]
    public List<Renderer> materialTargets = new List<Renderer>();

    [Space]
    [Tooltip("ถ้าเปิด จะรวบรวม Graphic ใต้ GameObject นี้ทั้งหมดให้เอง (รวมลูกหลาน)")]
    public bool includeAllChildGraphics = false;

    [Header("Loop Colors")]
    public Color[] colors = { Color.white, Color.yellow, Color.cyan };

    [Tooltip("เวลาที่ใช้ข้ามระหว่างสี (วินาทีต่อ 1 ช่วง)")]
    [Range(0.05f, 10f)] public float secondsPerStep = 1f;

    [Tooltip("ถ้าเปิด = ไปกลับ (PingPong), ถ้าปิด = วนรอบ (Loop)")]
    public bool pingPong = true;

    [Tooltip("ใช้เวลาที่ไม่โดน Time.timeScale หยุด")]
    public bool useUnscaledTime = false;

    [Header("Per-Target Phase")]
    [Tooltip("ถ้าเปิดจะเลื่อนเฟสของแต่ละตัวเป้าหมายให้เหลื่อมกันอัตโนมัติ")]
    public bool autoStaggerPhase = false;

    [Tooltip("ปริมาณเฟสที่เหลื่อมกันต่อหนึ่งตัว (หน่วยเป็น 'ช่วงสี')")]
    public float phasePerTarget = 0.15f;

    void Awake()
    {
        // สะดวกสำหรับงาน UI: รวบรวมทุก Graphic ใต้ตัวนี้
        if (includeAllChildGraphics)
        {
            uiTargets.Clear();
            uiTargets.AddRange(GetComponentsInChildren<Graphic>(true));
        }

        if (colors == null || colors.Length < 2)
            colors = new Color[] { Color.white, Color.black };

        // เซตสีตั้งต้น
        ApplyColorToAll(EvaluateColor(0f), basePhase: 0f);
    }

    void Update()
    {
        float tUnits = (useUnscaledTime ? Time.unscaledTime : Time.time)
                       / Mathf.Max(0.0001f, secondsPerStep);

        // ยิงสีไปยังทุก target (รองรับ phase เหลื่อม)
        ApplyColorToAll(EvaluateColor(tUnits), tUnits);
    }

    // ========== Core evaluate ==========
    // timeUnits = เวลาที่ถูกแปลงเป็น "หน่วยช่วงสี" (1 หน่วย = 1 การเลื่อนจากสี A ไป B)
    Color EvaluateColor(float timeUnits)
    {
        if (pingPong)
        {
            // ไป-กลับระหว่าง index 0..N-1
            float cycle = Mathf.PingPong(timeUnits, colors.Length - 1);
            int a = Mathf.FloorToInt(cycle);
            int b = Mathf.Clamp(a + 1, 0, colors.Length - 1);
            float f = cycle - a;
            return Color.Lerp(colors[a], colors[b], f);
        }
        else
        {
            // วนรอบ 0..N-1
            float cycle = Mathf.Repeat(timeUnits, colors.Length);
            int a = Mathf.FloorToInt(cycle);
            int b = (a + 1) % colors.Length;
            float f = cycle - a;
            return Color.Lerp(colors[a], colors[b], f);
        }
    }

    void ApplyColorToAll(Color baseColor, float basePhase)
    {
        // UI Graphics
        for (int i = 0; i < uiTargets.Count; i++)
        {
            var g = uiTargets[i];
            if (!g) continue;
            Color c = baseColor;
            if (autoStaggerPhase)
            {
                float tUnits = basePhase + (i * phasePerTarget);
                c = EvaluateColor(tUnits);
            }
            g.color = c;
        }

        // SpriteRenderers
        for (int i = 0; i < spriteTargets.Count; i++)
        {
            var s = spriteTargets[i];
            if (!s) continue;
            Color c = baseColor;
            if (autoStaggerPhase)
            {
                float tUnits = basePhase + (i * phasePerTarget);
                c = EvaluateColor(tUnits);
            }
            s.color = c;
        }

        // Renderers (Material _Color)
        for (int i = 0; i < materialTargets.Count; i++)
        {
            var r = materialTargets[i];
            if (!r) continue;
            if (r.material != null && r.material.HasProperty("_Color"))
            {
                Color c = baseColor;
                if (autoStaggerPhase)
                {
                    float tUnits = basePhase + (i * phasePerTarget);
                    c = EvaluateColor(tUnits);
                }
                r.material.color = c;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Child Graphics")]
    void RefreshChildGraphics()
    {
        uiTargets.Clear();
        uiTargets.AddRange(GetComponentsInChildren<Graphic>(true));
    }
#endif
}
