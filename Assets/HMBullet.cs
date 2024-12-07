using UnityEngine;

public class HMBullet : MonoBehaviour
{
    public Transform target; // Target yang akan diikuti
    public float speed = 5f; // Kecepatan peluru
    public float rotateSpeed = 200f; // Kecepatan rotasi peluru
    public float damage = 25f; // Damage
    public float lifetime = 5f; // Waktu hidup sebelum dihancurkan

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); // Hancurkan peluru setelah beberapa detik
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return; // Jika tidak ada target, peluru tidak bergerak
        }

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.right).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed; // Putar peluru ke arah target
        rb.velocity = transform.right * speed; // Gerakkan peluru maju
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // Hancurkan peluru setelah mengenai target
        }
        else
        {
            Destroy(gameObject); // Hancurkan jika mengenai objek lain
        }
    }
}
