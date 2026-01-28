using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Görsel Efektler")]
    public GameObject hitBloodPrefab;    // Vuruþ Kaný
    public Renderer enemyRenderer;       // Beyaz Parlama için

    // Bileþenler
    NavMeshAgent agent;
    EnemyAttack attackScript;
    Animator animator;
    Rigidbody mainRb;

    // RAGDOLL ÝÇÝN GEREKLÝ LÝSTELER
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    bool isDead = false;
    Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        attackScript = GetComponent<EnemyAttack>();
        animator = GetComponent<Animator>();
        mainRb = GetComponent<Rigidbody>();

        // Renderer'ý bul ve orijinal rengi kaydet
        if (enemyRenderer == null)
            enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;

        // --- RAGDOLL KURULUMU ---
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // --- BUZ PÝSTÝ ÖNLEYÝCÝ (OTOMATÝK FREN) ---
        // Eðer düþmanýn ana Rigidbody'sinde sürtünme yoksa, biz ekleyelim ki kaymasýn.
        if (mainRb != null)
        {
            mainRb.linearDamping = 10f;  // Sürtünme (Eski adý: Drag)
            mainRb.angularDamping = 5f; // Dönme Sürtünmesi (Eski adý: Angular Drag)
        }

        // Oyun baþýnda Ragdoll kapalý olsun (Yürüyebilsin)
        ToggleRagdoll(false);
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection, float hitForce)
    {
        if (isDead) return;

        currentHealth -= amount;

        // 1. KAN EFEKTÝ
        if (hitBloodPrefab != null)
        {
            Quaternion bloodRotation = Quaternion.LookRotation(hitDirection);
            GameObject blood = Instantiate(hitBloodPrefab, hitPoint, bloodRotation);
            Destroy(blood, 2f);
        }

        // 2. HIT FLASH (BEYAZLAMA)
        if (enemyRenderer != null) StartCoroutine(FlashWhite());

        // 3. GERÝ ÝTÝLME (KNOCKBACK) - Parry burada çalýþýr
        if (hitForce > 5f)
        {
            StartCoroutine(ApplyKnockback(hitDirection, hitForce));
        }

        // 4. ÖLÜM KONTROLÜ
        if (currentHealth <= 0)
        {
            Die(hitDirection, hitForce);
        }
    }

    // --- EKSÝK OLAN FONKSÝYON BU ---
    IEnumerator ApplyKnockback(Vector3 direction, float force)
    {
        // 1. Zekayý kapat (Direnmeyi býrak)
        if (agent != null) agent.enabled = false;

        // 2. Fiziði aç ve it
        if (mainRb != null)
        {
            mainRb.isKinematic = false; // Hareket serbest
            // Hafif yukarý kaldýrarak it ki yere takýlmasýn
            mainRb.AddForce((direction + Vector3.up * 0.5f).normalized * force, ForceMode.Impulse);
        }

        // 3. Bir süre bekle (Uçuþ süresi)
        yield return new WaitForSeconds(0.25f);

        // 4. Hala ölmediysek toparlan
        if (!isDead && mainRb != null)
        {
            // Durdurmak için hýzýný sýfýrla
            mainRb.linearVelocity = Vector3.zero;
            mainRb.isKinematic = true; // Tekrar kilitle

            if (agent != null)
            {
                agent.Warp(transform.position); // NavMesh'e oturt
                agent.enabled = true; // Zekayý aç
                // Oyuncuyu tekrar hedef al
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) agent.SetDestination(player.transform.position);
            }
        }
    }

    void Die(Vector3 impactDir, float force)
    {
        isDead = true;

        // Puan ver
        GameManager manager = FindFirstObjectByType<GameManager>();
        if (manager != null) manager.AddScore(10);

        // --- RAGDOLL AKTÝFLEÞTÝR ---
        ToggleRagdoll(true);

        // Cesedi it
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddForce(impactDir * force, ForceMode.Impulse);
        }

        // 10 saniye sonra yok et
        Destroy(gameObject, 10f);
    }

    void ToggleRagdoll(bool state)
    {
        // TRUE = Ölü (Ragdoll Açýk), FALSE = Canlý
        if (agent != null) agent.enabled = !state;
        if (animator != null) animator.enabled = !state;
        if (attackScript != null) attackScript.enabled = !state;

        // Ana Kapsülü Kapat
        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = !state;

        // Kemikleri Aç/Kapat
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col != mainCollider) col.enabled = state;
        }
    }

    IEnumerator FlashWhite()
    {
        enemyRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        enemyRenderer.material.color = originalColor;
    }
}