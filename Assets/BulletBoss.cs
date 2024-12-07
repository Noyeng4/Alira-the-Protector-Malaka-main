using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBoss : MonoBehaviour
{
    public Transform target; // Target untuk Player atau Ally
    public GameObject volleyPrefab; // Prefab untuk peluru jenis satu
    public GameObject pbPrefab; // Prefab untuk peluru jenis dua
    public GameObject hmPrefab; // Prefab untuk peluru homing
    public Transform bulletPos; // Posisi tetap untuk peluru biasa
    public float projectileSpeed = 10f;
    public float attackCooldown = 3f; // Cooldown setelah semua proyektil ditembakkan
    public float volleyInterval = 0.2f; // Interval antar proyektil
    public int volleyCount = 6; // Jumlah proyektil dalam satu volley
    public AudioClip shootSound; // Suara tembakan

    private AudioSource audioSource;
    public string playerTag = "Player"; // Tag untuk mencari objek player
    private float lastAttackTime = 0f;
    private Animator animator;
    private bool isShooting = false;
    private int currentRound = 0; // Melacak urutan siklus tembakan
    private List<System.Func<IEnumerator>> attackPatterns; // Daftar pola serangan

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Inisialisasi pola serangan
        attackPatterns = new List<System.Func<IEnumerator>>()
        {
            ShootVolley,
            ShootVolley,
            ShootPB,
            ShootVolley,
            ShootVolley,
            ShootPB,
            ShootHM // Tambahkan pola peluru homing
        };
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target != null)
        {
            AimAtTarget();
        }
    }

    private void AimAtTarget()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void ShootAtTarget()
    {
        if (Time.time >= lastAttackTime + attackCooldown && target != null && !isShooting)
        {
            if (currentRound >= 0 && currentRound < attackPatterns.Count)
            {
                StartCoroutine(attackPatterns[currentRound]());
            }

            currentRound = (currentRound + 1) % attackPatterns.Count; // Siklus ulang
        }
    }

    private IEnumerator ShootVolley()
    {
        isShooting = true;
        animator?.SetTrigger("isAttacking");

        for (int i = 0; i < volleyCount; i++)
        {
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            if (volleyPrefab != null)
            {
                GameObject projectile = Instantiate(volleyPrefab, bulletPos.position, bulletPos.rotation);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (target.position - bulletPos.position).normalized;
                    rb.velocity = direction * projectileSpeed;
                }
            }

            yield return new WaitForSeconds(volleyInterval);
        }

        lastAttackTime = Time.time;
        isShooting = false;
    }

    private IEnumerator ShootPB()
    {
        isShooting = true;
        animator?.SetTrigger("isAttacking");

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (pbPrefab != null)
        {
            GameObject projectile = Instantiate(pbPrefab, bulletPos.position, bulletPos.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (target.transform.position - bulletPos.position).normalized;
                rb.velocity = direction * projectileSpeed;
            }
        }

        yield return new WaitForSeconds(0.2f);

        lastAttackTime = Time.time;
        isShooting = false;
    }

    private IEnumerator ShootHM()
    {
        isShooting = true;
        animator?.SetTrigger("isHoming");

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (hmPrefab != null)
        {
            GameObject projectile = Instantiate(hmPrefab, bulletPos.position, bulletPos.rotation);
            HMBullet hmBullet = projectile.GetComponent<HMBullet>();
            if (hmBullet != null)
            {
                hmBullet.target = target; // Berikan referensi target ke peluru homing
            }
        }

        yield return new WaitForSeconds(0.2f);

        lastAttackTime = Time.time;
        isShooting = false;
    }
}
