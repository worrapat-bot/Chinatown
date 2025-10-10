using UnityEngine;
#if TMP_PRESENT
using TMPro;
#endif
using UnityEngine.UI;

public class FloatText : MonoBehaviour
{
    [Header("Motion")]
    [Tooltip("ระยะการลอย (หน่วยพิกเซลสำหรับ UI, หน่วยโลกสำหรับ 3D)")]
    public float amplitude = 20f;

    [Tooltip("ความถี่การลอย (ครั้งต่อวินาที)")]
    public float frequency = 1f;

    [Tooltip("ปรับเฟสเพื่อลดการลอยพร้อมกันของหลายชิ้น")]
    public float phase = 0f;

    [Tooltip("ใช้เวลาแบบไม่หยุดเมื่อเกมหยุด (เช่น ใน Pause)")]
    public bool useUnscaledTime = false;

    [Header("Optional Fade")]
    [Tooltip("ให้จาง-เข้าพร้อมการลอย")]
    public bool fadeWithSine = false;
    [Range(0f, 1f)]
    public float minAlpha = 0.5f;

    RectTransform rect;            // สำหรับ UI
    Vector2 startAnchoredPos;      // ตำแหน่งเริ่มต้นของ UI
    Vector3 startLocalPos;         // ตำแหน่งเริ่มต้นของวัตถุทั่วไป

    // อ้างอิงคอมโพเนนต์ข้อความ (ถ้ามี) เพื่อปรับความทึบ
    Graphic uiGraphic;
#if TMP_PRESENT
    TMP_Text tmpText;
#endif
    Text legacyText;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect != null)
            startAnchoredPos = rect.anchoredPosition;
        else
            startLocalPos = transform.localPosition;

        uiGraphic = GetComponent<Graphic>();
#if TMP_PRESENT
        tmpText = GetComponent<TMP_Text>();
#endif
        legacyText = GetComponent<Text>();
    }

    void Update()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;
        float s = Mathf.Sin((t * Mathf.PI * 2f * frequency) + phase);
        float offset = s * amplitude;

        // เคลื่อนที่ขึ้นลง
        if (rect != null)
            rect.anchoredPosition = startAnchoredPos + new Vector2(0f, offset);
        else
            transform.localPosition = startLocalPos + new Vector3(0f, offset, 0f);

        // ปรับความทึบ (ถ้าเปิด)
        if (fadeWithSine)
        {
            float a = Mathf.Lerp(minAlpha, 1f, (s + 1f) * 0.5f); // map -1..1 -> minAlpha..1
#if TMP_PRESENT
            if (tmpText != null)
            {
                var c = tmpText.color; c.a = a; tmpText.color = c;
            }
            else
#endif
            if (uiGraphic != null)
            {
                var c = uiGraphic.color; c.a = a; uiGraphic.color = c;
            }
            else if (legacyText != null)
            {
                var c = legacyText.color; c.a = a; legacyText.color = c;
            }
        }
    }
}
