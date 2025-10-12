using UnityEngine;

/// กล้อง 2.5D: ตามเป้าหมายแบบ 2D ในฉาก 3D
[ExecuteAlways]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target;          // ลาก Player มาวางที่นี่

    [Header("Framing")]
    public Vector2 offset = new Vector2(0f, 2f);  // ระยะเผื่อบน-ล่าง/ซ้าย-ขวา
    public float distance = 10f;                  // ระยะกล้องถอยหลังไปทาง -Z

    [Header("Smoothing")]
    public float smoothTime = 0.20f;              // ค่ายิ่งสูง = หน่วงมาก (นุ่มนวลขึ้น)
    private Vector3 velocity;                     // ตัวแปรภายในของ SmoothDamp

    [Header("Camera Mode")]
    public bool orthographic = true;              // เปิด Orthographic เพื่ออารมณ์ 2D แท้ ๆ
    public float orthoSize = 6f;                  // ซูมเข้า/ออกสำหรับ Orthographic

    [Header("Optional Clamp (ขอบเขตพื้นที่กล้อง)")]
    public bool clamp = false;
    public Vector2 minXY = new Vector2(-999, -999);
    public Vector2 maxXY = new Vector2( 999,  999);

    void Start()
    {
        // ตั้งค่ากล้องเป็น Orthographic ถ้าต้องการ
        var cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = orthographic;
            if (orthographic) cam.orthographicSize = orthoSize;
        }

        // ล็อกมุมมองให้กล้องมองตรงเข้าฉาก (แกน +Z)
        transform.rotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        if (!target) return;

        // ต้องการตามเฉพาะ X,Y และให้ Z คงที่เป็นระยะถอย "distance"
        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            -distance
        );

        if (clamp)
        {
            desired.x = Mathf.Clamp(desired.x, minXY.x, maxXY.x);
            desired.y = Mathf.Clamp(desired.y, minXY.y, maxXY.y);
        }

        // เลื่อนแบบนุ่มนวล
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
