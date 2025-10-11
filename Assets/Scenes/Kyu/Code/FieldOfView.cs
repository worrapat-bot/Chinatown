using UnityEngine;

public class FieldOfView : EnemyPatrol
{
    [Header("Field of View Settings")]
    public float viewRadius = 10.0f; // รัศมีการมองเห็น (ระยะไกลสุด)
    [Range(0, 360)]
    public float viewAngle = 90.0f; // มุมมองทั้งหมด (เช่น 90 องศา)
    public LayerMask targetMask;     // Layer ของผู้เล่น
    public LayerMask obstructionMask; // Layer ของสิ่งกีดขวาง (กำแพง)

    [HideInInspector]
    public bool playerInSight = false;
    // แก้ไข: ต้องเป็น public เพื่อให้ EnemyPatrol เข้าถึงได้
    [HideInInspector]
    public Transform playerTarget; 

    protected virtual void Start()
    {
        // ค้นหาผู้เล่นด้วย Tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
            // เริ่มการตรวจสอบการมองเห็นเป็นระยะๆ (ดีกว่าการทำทุกเฟรม)
            InvokeRepeating(nameof(FindTargetsWithDelay), 0.1f, 0.2f);
        }
        else
        {
            Debug.LogError("ไม่พบ GameObject ที่มี Tag เป็น 'Player' ในฉาก! กรุณาตรวจสอบ Tag 'Player'!");
        }
    }

    // ฟังก์ชันที่เรียกใช้ซ้ำๆ เพื่อตรวจสอบผู้เล่น
    protected virtual void FindTargetsWithDelay()
    {
        playerInSight = CheckForPlayer();
    }

    private bool CheckForPlayer()
    {
        if (playerTarget == null) return false;

        // คำนวณทิศทางไปยังผู้เล่น
        Vector3 directionToTarget = (playerTarget.position - transform.position).normalized;
        
        // คำนวณมุมระหว่างทิศทางที่ศัตรูหันหน้าไป (transform.forward) กับทิศทางไปยังผู้เล่น
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        // 1. ตรวจสอบว่าผู้เล่นอยู่ในมุมมองหรือไม่
        if (angleToTarget < viewAngle / 2)
        {
            // ผู้เล่นอยู่ในมุมกรวย

            // 2. ตรวจสอบระยะห่าง
            float distanceToTarget = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToTarget < viewRadius)
            {
                // ผู้เล่นอยู่ในระยะการมองเห็น

                // 3. ตรวจสอบสิ่งกีดขวาง (Raycasting)
                // ยิง Raycast จากตาของศัตรูไปยังผู้เล่น
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    // ไม่มีสิ่งกีดขวาง: ผู้เล่นถูกตรวจจับ
                    return true;
                }
            }
        }
        
        // หากไม่เป็นไปตามเงื่อนไขทั้งหมด
        return false;
    }
    
    // ฟังก์ชันสำหรับวาดมุมมองทรงกรวยใน Editor (Gizmos)
    private void OnDrawGizmos()
    {
        // ตรวจสอบว่ากำลังเล่นใน Editor และถูกเลือกอยู่
        if (Application.isPlaying)
        {
            Gizmos.color = playerInSight ? Color.red : Color.yellow;
        }
        else
        {
             Gizmos.color = Color.yellow;
        }

        // วาด Raycast เพื่อแสดงทิศทางที่หันหน้าไป
        Gizmos.DrawRay(transform.position, transform.forward * viewRadius);

        // คำนวณทิศทาง Ray ซ้ายและขวาของมุมมอง
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        // วาดเส้นขอบของมุมมอง
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }
    
    // ฟังก์ชันช่วยในการคำนวณ Vector3 จากมุม (Angle)
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        // แปลงมุมจากองศาให้เป็น Vector3
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
