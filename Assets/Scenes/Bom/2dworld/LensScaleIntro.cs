using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LensScaleIntro : MonoBehaviour
{
    [Header("References")]
    public Volume volume;                  // ใส่ Global Volume (ถ้าเว้นไว้จะหาในตัวเอง)

    [Header("Animation")]
    [Range(0f, 1f)] public float startScale = 0f;
    [Range(0f, 1f)] public float targetScale = 0.8f;
    [Min(0.01f)] public float duration = 1.5f;  // วินาที
    public float delay = 0f;                     // หน่วงก่อนเริ่ม (วินาที)
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private LensDistortion lens;

    void Awake()
    {
        if (!volume) volume = GetComponent<Volume>();
        if (volume && volume.profile && volume.profile.TryGet(out lens))
        {
            lens.scale.overrideState = true; // ให้ค่าสคริปต์คุม
        }
        else
        {
            Debug.LogError("LensScaleIntro: ไม่พบ Lens Distortion ใน Volume Profile");
        }
    }

    void OnEnable()
    {
        if (lens == null) return;
        StopAllCoroutines();
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        lens.scale.value = startScale;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float k = ease.Evaluate(Mathf.Clamp01(t));
            lens.scale.value = Mathf.Lerp(startScale, targetScale, k);
            yield return null;
        }
    }
}
