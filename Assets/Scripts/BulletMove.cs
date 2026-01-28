using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float pushForce = 15f; // Merminin itme gücü (Bunu editörden artýrýp azaltabilirsin)

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // DÝKKAT: Artýk OnTriggerEnter kullanýyoruz (Trigger kutusunu açtýðýmýz için)
    void OnTriggerEnter(Collider other)
    {
        // --- DÜZELTÝLEN KISIM ---
        // Artýk isme ("Clone") bakmýyoruz, çünkü düþmanlarda da Clone yazýyor.
        // Sadece etiketi "Bullet" olan (diðer mermiler) veya "Player" olanlarý görmezden gel.
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
        {
            return;
        }
        // -------------------------

        HealthSystem health = other.GetComponent<HealthSystem>();

        if (health != null)
        {
            // Log'a bak: Artýk "Enemy(Clone)" yazmasý lazým
            Debug.Log($"Vurulan: {other.name} | Hasar: {damage} | Ýtme: {pushForce}");

            health.TakeDamage(damage, transform.position, transform.forward, pushForce);
        }

        // Çarptýðýn þey mermi veya oyuncu deðilse (yani Düþman veya Duvar ise) kendini yok et
        Destroy(gameObject);
    }
}