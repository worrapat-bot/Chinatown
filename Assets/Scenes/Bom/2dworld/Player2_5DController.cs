using UnityEngine;

/// ควบคุมผู้เล่นแบบ 2D ในฉาก 3D (วิ่งแกน X + กระโดดยก Y, ล็อก Z)
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player2_5DController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7.5f;     // ความเร็ววิ่ง
    [SerializeField] float accelGround = 60f;    // อัตราเร่งบนพื้น
    [SerializeField] float accelAir = 30f;       // อัตราเร่งกลางอากาศ
    [SerializeField] bool faceFlip = true;       // ให้ตัวหันตามทิศวิ่ง (กลับสเกล X)

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2.6f;    // ความสูงกระโดดประมาณ
    [SerializeField] int extraJumps = 0;         // 0 = กระโดดเดียว, 1 = ดับเบิลจัมพ์ ฯลฯ
    [SerializeField] float coyoteTime = 0.12f;   // เผื่อเวลาเท้าลอยนิดหน่อยก็ยังกระโดดได้
    [SerializeField] float jumpBuffer = 0.12f;   // กดปุ่มก่อนแตะพื้นเล็กน้อยแล้วติดกระโดด

    [Header("Ground Check")]
    [SerializeField] Vector3 groundCheckOffset = new Vector3(0, -0.6f, 0);
    [SerializeField] float groundCheckRadius = 0.25f;
    [SerializeField] LayerMask groundMask;

    [Header("Z Lock")]
    [SerializeField] float fixedZ = 0f;          // ตำแหน่ง Z ที่ต้องการล็อก

    // === Private ===
    Rigidbody rb;
    CapsuleCollider col;
    float coyoteCounter;
    float jumpBufferCounter;
    int jumpsLeft;
    float inputX;

    // ระบบอนิเมเตอร์ (ถ้ามี)
    [Header("Optional Animator")]
    [SerializeField] Animator animator;
    [SerializeField] string runParam = "Speed";
    [SerializeField] string groundParam = "Grounded";
    [SerializeField] string jumpTrig = "Jump";

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // ตั้งค่า Rigidbody ให้เหมาะกับเกม 2D
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;      // กันหมุน
        // ถ้าอยากล็อก Z แบบฮาร์ด ให้เปิด FreezePositionZ ด้วยก็ได้
        // rb.constraints |= RigidbodyConstraints.FreezePositionZ;

        // เริ่มต้นจำนวนจัมพ์
        jumpsLeft = extraJumps;
    }

    void Update()
    {
        // --- อ่านอินพุต ---
        inputX = Input.GetAxisRaw("Horizontal");  // A/D หรือ ซ้าย/ขวา (ค่า -1..1)

        // เก็บ jump buffer เมื่อกดกระโดด
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBuffer;

        // นับถอยหลังตัวจับเวลา
        coyoteCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;

        // อัปเดตอนิเมเตอร์ (ถ้ามี)
        if (animator)
        {
            animator.SetFloat(runParam, Mathf.Abs(inputX));
            animator.SetBool(groundParam, IsGrounded());
        }

        // หันตัวตามทิศทางวิ่ง
        if (faceFlip && Mathf.Abs(inputX) > 0.01f)
        {
            var s = transform.localScale;
            s.x = Mathf.Sign(inputX) * Mathf.Abs(s.x);
            transform.localScale = s;
        }

        // พยายามกระโดดถ้ากดทันเวลา
        TryConsumeJump();
    }

    void FixedUpdate()
    {
        // เช็กพื้นเพื่อรีเซ็ตตัวจับเวลาและจำนวนจัมพ์
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            jumpsLeft = extraJumps;
        }

        // คำนวณความเร็วเป้าหมายบนแกน X
        float targetVX = inputX * moveSpeed;
        float accel = IsGrounded() ? accelGround : accelAir;
        float newVX = Mathf.MoveTowards(rb.velocity.x, targetVX, accel * Time.fixedDeltaTime);

        // ล็อก Z และคงค่า vy ตามฟิสิกส์
        rb.velocity = new Vector3(newVX, rb.velocity.y, 0f);
        var p = rb.position;
        p.z = fixedZ;
        rb.position = p;
    }

    void TryConsumeJump()
    {
        // กระโดดได้ ถ้า (ยังอยู่ในช่วง coyote) หรือ (จำนวนจัมพ์เสริม > 0) และมี buffer อยู่
        if (jumpBufferCounter > 0f &&
            (coyoteCounter > 0f || (!IsGrounded() && jumpsLeft > 0)))
        {
            // ความเร็วกระโดดจากความสูงที่ต้องการ v = sqrt(2gh)
            float g = Physics.gravity.y;
            float vJump = Mathf.Sqrt(Mathf.Abs(2f * g * jumpHeight));

            var v = rb.velocity;
            v.y = vJump;
            rb.velocity = v;

            if (coyoteCounter <= 0f && !IsGrounded())
                jumpsLeft--; // ใช้สิทธิ์จัมพ์กลางอากาศ

            // รีเซ็ตตัวจับเวลา
            coyoteCounter = 0f;
            jumpBufferCounter = 0f;

            if (animator && !string.IsNullOrEmpty(jumpTrig))
                animator.SetTrigger(jumpTrig);
        }

        // กดค้างแล้วลดยกขาเพื่อทำ "variable jump" (ปล่อยปุ่มแล้วร่วงเร็วขึ้น)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.55f, 0f);
        }
    }

    bool IsGrounded()
    {
        // ใช้ OverlapSphere ใต้เท้า เพื่อตรวจพื้น
        Vector3 center = transform.TransformPoint(groundCheckOffset);
        return Physics.CheckSphere(center, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ?
            transform.TransformPoint(groundCheckOffset) :
            transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(center, groundCheckRadius);
    }
}
