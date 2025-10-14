using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class WoozyLensDistortion : MonoBehaviour
{
    public Volume volume;                 // ใส่ Global Volume ของนาย (ถ้าไม่ได้ใส่ จะ auto หาในตัวเอง)
    private LensDistortion lens;

    [Header("Base multipliers (ค่าแกนกลาง)")]
    [Range(0f, 1f)] public float baseX = 0.40f;
    [Range(0f, 1f)] public float baseY = 0.40f;

    [Header("Amplitude (ระยะส่าย)")]
    [Range(0f, 1f)] public float ampX = 0.15f;
    [Range(0f, 1f)] public float ampY = 0.15f;

    [Header("Speed (ความเร็วการส่าย)")]
    public float speedX = 0.7f;
    public float speedY = 1.1f;

    [Header("Extra")]
    public float phaseOffset = 0.5f;      // เฟสต่างกันเล็กน้อยให้ดูเมามากขึ้น
    public bool alsoTiltCamera = true;    // เอียงกล้องเพิ่มความเวียนหัว
    public float tiltAmplitude = 3f;      // องศาเอียง (Z)
    public float tiltSpeed = 0.6f;

    void Awake()
    {
        if (!volume) volume = GetComponent<Volume>();
        if (volume && volume.profile && volume.profile.TryGet(out lens))
        {
            // เผื่อบางโปรไฟล์ยังไม่ tick Override
            lens.xMultiplier.overrideState = true;
            lens.yMultiplier.overrideState = true;
            lens.intensity.overrideState  = true; // มีไว้เฉยๆ เผื่ออยากปรับ intensity ภายหลัง
        }
        else
        {
            Debug.LogError("WoozyLensDistortion: ไม่เจอ Lens Distortion ใน Volume Profile!");
        }
    }

    void Update()
    {
        if (lens == null) return;

        float t = Time.time;

        // ส่ายแบบ Sin/Cos คนละเฟส
        float x = baseX + Mathf.Sin(t * speedX) * ampX;
        float y = baseY + Mathf.Cos((t + phaseOffset) * speedY) * ampY;

        lens.xMultiplier.value = Mathf.Clamp01(x);
        lens.yMultiplier.value = Mathf.Clamp01(y);

        // (ออปชัน) เอียงกล้องนิดๆ ให้ภาพหมุนๆ
        if (alsoTiltCamera && Camera.main != null)
        {
            float z = Mathf.Sin(t * tiltSpeed) * tiltAmplitude;
            var e = Camera.main.transform.eulerAngles;
            e.z = z;
            Camera.main.transform.eulerAngles = e;
        }
    }
}
