using UnityEngine;
using System.Collections.Generic;

// สถานะการลาดตระเวนที่เป็นไปได้ของศัตรู
public enum EnemyPatrolState
{
    Moving,          // กำลังเดินไปยังจุด Patrol ถัดไป
    Waiting,         // หยุดรอตามเวลาที่กำหนด (waitTime)
    LookingAround,   // หมุนซ้ายขวาเพื่อตรวจสอบสภาพแวดล้อม
    PlayerDetected   // หยุดเมื่อตรวจจับผู้เล่นได้
}

// สคริปต์นี้ต้องมี FieldOfView ติดอยู่ด้วย
[RequireComponent(typeof(FieldOfView))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Behavior Toggles")]
    [Tooltip("เปิด/ปิดการเคลื่อนที่ลาดตระเวน. ถ้าปิด ศัตรูจะยืนนิ่ง (แต่ยังคงตรวจจับผู้เล่นได้)")]
    public bool enablePatrolMovement = true; // ตัวสลับที่คุณต้องการ

    [Header("Movement Settings")]
    public float moveSpeed = 3.0f; // ความเร็วในการเดินลาดตระเวน
    public float waitTime = 1.5f;   // เวลาที่หยุดนิ่ง *ก่อน* เริ่มหมุนสอดส่อง
    public float rotationSpeed = 50.0f; // ความเร็วในการหมุนตัวละคร (ทั้งตอนเดินและสอดส่อง)

    [Header("Look Around Settings (3 วินาทีที่คุณกำหนด)")]
    public float lookAroundDuration = 3.0f; // เวลาที่ใช้ในการหมุนซ้ายขวา
    public float lookAngle = 45.0f; // มุมที่จะหมุนไปทางซ้ายและขวาจากทิศทางหลัก

    [Header("Patrol Points")]
    public List<Transform> patrolPoints = new List<Transform>();
    
    // สถานะปัจจุบันของศัตรู
    private EnemyPatrolState currentState = EnemyPatrolState.Waiting;
    private int currentPointIndex;
    private float stateTimer; // ใช้จับเวลาสำหรับ Waiting และ LookingAround
    private FieldOfView fieldOfView;
    
    // สำหรับการหมุนสอดส่อง
    private Quaternion initialRotation; // การหมุนเริ่มต้น ณ จุดที่หยุด
    private Quaternion targetLookRotation;
    private bool isRotatingLeft = true; 

    void Start()
    {
        fieldOfView = GetComponent<FieldOfView>();

        if (patrolPoints.Count == 0)
        {
            Debug.LogError("กรุณาเพิ่มจุด Patrol Points อย่างน้อยหนึ่งจุดใน EnemyPatrol component!");
            enabled = false; 
            return;
        }

        currentPointIndex = 0;
        // กำหนดตำแหน่งเริ่มต้นให้ตรงกับจุดแรก (SnapToCurrentPoint ใช้แค่ตอนเริ่มต้น)
        SnapToCurrentPoint(); 
        currentState = EnemyPatrolState.Moving; 
    }

    void Update()
    {
        // --- 1. ตรวจสอบการตรวจจับผู้เล่นเป็นอันดับแรก ---
        if (fieldOfView.playerInSight)
        {
            currentState = EnemyPatrolState.PlayerDetected;
            HandlePlayerDetected();
            return;
        }
        
        // --- 2. ตัวสลับการเคลื่อนที่ (Toggle) ---
        if (!enablePatrolMovement)
        {
            // ถ้าปิดการลาดตระเวน ให้ศัตรูอยู่ในสถานะหยุดนิ่ง (Waiting)
            currentState = EnemyPatrolState.Waiting;
            return;
        }

        // --- 3. จัดการตามสถานะ Patrol ปกติ ---
        switch (currentState)
        {
            case EnemyPatrolState.Moving:
                MoveToPatrolPoint();
                break;
            case EnemyPatrolState.Waiting:
                HandleWaiting();
                break;
            case EnemyPatrolState.LookingAround:
                HandleLookingAround();
                break;
            // PlayerDetected ถูกจัดการด้านบนแล้ว
        }
    }

    // --- State Handlers ---

    private void MoveToPatrolPoint()
    {
        Vector3 targetPosition = patrolPoints[currentPointIndex].position;
        targetPosition.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > 0.1f)
        {
            // 1. หมุนตัวให้หันไปทางเป้าหมาย
            RotateTowards(targetPosition, rotationSpeed);

            // 2. เคลื่อนที่ไปข้างหน้าอย่างนุ่มนวล (ไม่ใช่การวาป)
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );
        }
        else // ถึงจุดหมายแล้ว
        {
            // เปลี่ยนไปสถานะหยุดรอ
            currentState = EnemyPatrolState.Waiting;
            stateTimer = waitTime;
            // เก็บการหมุนปัจจุบันไว้เป็นค่าตั้งต้นสำหรับการสอดส่อง
            initialRotation = transform.rotation; 
        }
    }
    
    // สถานะหยุดนิ่งก่อนเริ่มหมุนสอดส่อง
    private void HandleWaiting()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            // หมดเวลาหยุดรอ เปลี่ยนไปสถานะสอดส่อง
            currentState = EnemyPatrolState.LookingAround;
            stateTimer = lookAroundDuration;
            
            // กำหนดทิศทางการหมุนแรกสุด (เช่น ไปทางซ้าย)
            targetLookRotation = initialRotation * Quaternion.Euler(0, -lookAngle, 0);
            isRotatingLeft = true;
        }
    }
    
    // สถานะหมุนซ้ายขวา 3 วินาที
    private void HandleLookingAround()
    {
        stateTimer -= Time.deltaTime;
        
        // หมุนอย่างนุ่มนวลไปยัง targetLookRotation
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetLookRotation, 
            rotationSpeed * Time.deltaTime
        );
        
        // ตรวจสอบว่าใกล้ถึงการหมุนเป้าหมายหรือไม่ เพื่อสลับทิศทาง
        if (Quaternion.Angle(transform.rotation, targetLookRotation) < 1.0f)
        {
            // สลับทิศทางการหมุนไปทางตรงข้าม
            if (isRotatingLeft)
            {
                // หมุนกลับไปทางขวาจากทิศทางหลัก
                targetLookRotation = initialRotation * Quaternion.Euler(0, lookAngle, 0);
            }
            else
            {
                // หมุนกลับไปทางซ้ายจากทิศทางหลัก
                targetLookRotation = initialRotation * Quaternion.Euler(0, -lookAngle, 0);
            }
            isRotatingLeft = !isRotatingLeft;
        }

        if (stateTimer <= 0)
        {
            // หมดเวลาสอดส่อง เปลี่ยนไปจุด Patrol ถัดไป
            GoToNextPoint();
        }
    }
    
    private void HandlePlayerDetected()
    {
        // ในสถานะ PlayerDetected ให้ศัตรูหันหน้าเข้าหาผู้เล่น
        if (fieldOfView.playerTarget != null)
        {
            RotateTowards(fieldOfView.playerTarget.position, rotationSpeed * 2); // หมุนเร็วขึ้นเมื่อเจอผู้เล่น
        }
    }

    // --- Utility Functions ---

    // หมุนศัตรูให้หันหน้าไปยังเป้าหมาย (ใช้สำหรับตอนเดินและตอนพบผู้เล่น)
    private void RotateTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position);
        direction.y = 0; // ไม่รวมแกน Y ในการหมุน

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                lookRotation, 
                speed * Time.deltaTime
            );
        }
    }
    
    // เปลี่ยนไปยังจุด Patrol ถัดไป (ไม่ต้อง Snap ตำแหน่ง)
    private void GoToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        currentState = EnemyPatrolState.Moving; 
    }
    
    // กำหนดตำแหน่งเริ่มต้นที่แกน Y ให้คงที่ (ใช้เฉพาะใน Start() เท่านั้น)
    private void SnapToCurrentPoint()
    {
        if (patrolPoints[currentPointIndex] != null)
        {
            Vector3 newPos = patrolPoints[currentPointIndex].position;
            newPos.y = transform.position.y;
            transform.position = newPos;
        }
    }
}
