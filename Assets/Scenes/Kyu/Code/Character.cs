using UnityEngine;

/// <summary>
/// คลาสพื้นฐาน (Abstract Base Class) สำหรับตัวละครทั้งหมด
/// รองรับคุณสมบัติด้านสุขภาพ การโจมตี และสถานะพื้นฐาน
/// </summary>
public abstract class Character : MonoBehaviour
{
    // --- FIELDS / BACKING FIELDS ---
    [Header("Base Stats")]
    [SerializeField] protected float _maxHealth = 100f;
    [SerializeField] protected float _attackDamage = 40f;
    [SerializeField] protected float _health; // พลังชีวิตปัจจุบัน
    [SerializeField] protected float _speed = 5.0f;

    [Header("Base State")]
    public bool Attack = false; 
    public bool Stunned = false;
    // + Invulnerable: bool - สถานะอยู่ยงคงกระพัน (ไม่รับดาเมจ)
    public bool Invulnerable = false; 

    // --- PROPERTIES (คุณสมบัติสำหรับเข้าถึงข้อมูล) ---
    
    public float MaxHealth => _maxHealth;
    public float AttackDamage => _attackDamage;

    // + <<get>> # <<set>> health: float (Public Getter, Protected Setter)
    public float Health 
    {
        get => _health;
        protected set 
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }


    // --- UNITY LIFECYCLE ---

    protected virtual void Awake()
    {
        Health = MaxHealth;
    }
    
    
    // --- METHODS (การทำงาน) ---

    // + TakeDamage(float dmg): void - ได้รับความเสียหาย
    public virtual void TakeDamage(float dmg)
    {
        // **การเปลี่ยนแปลงสำคัญ:** ตรวจสอบสถานะ Invulnerable
        if (dmg <= 0 || Stunned || Invulnerable) 
        {
            if (Invulnerable)
            {
                Debug.Log($"[{gameObject.name}] หลบหลีกได้!");
            }
            return; 
        }

        Health -= dmg;
        Debug.Log($"[{gameObject.name}] ได้รับความเสียหาย {dmg} เหลือพลังชีวิต {Health}");

        if (Health <= 0)
        {
            Die();
        }
    }

    // + Heal(float h): void - รักษา
    public virtual void Heal(float h)
    {
        if (h <= 0) return;
        
        Health += h; 
        Debug.Log($"[{gameObject.name}] ได้รับการรักษา {h} พลังชีวิตเพิ่มเป็น {Health}");
    }

    // # Die(): void - ตาย
    protected virtual void Die()
    {
        Debug.Log($"[{gameObject.name}] ถูกกำจัด / ตายแล้ว.");
    }

    // + Move(Vector3 direction): void - ตัวอย่างการเคลื่อนที่
    public void Move(Vector3 direction)
    {
        if (Stunned) return;
        
        // จำลองการเคลื่อนที่
        transform.position += direction.normalized * _speed * Time.deltaTime;
    }
}
