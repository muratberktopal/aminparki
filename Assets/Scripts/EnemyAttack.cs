using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 1;      // Kaç vursun?
    public float attackSpeed = 1.5f; // Kaç saniyede bir vursun?
    public float pushForce = 5.0f; // Isýrýnca oyuncuyu ne kadar iteklesin? (Mermiden daha az olmalý)

    float lastAttackTime;

    void OnCollisionStay(Collision collision)
    {
        // 1. Dokunduðum þey "Player" mý?
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Zaman kontrolü
            if (Time.time > lastAttackTime + attackSpeed)
            {
                HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();

                if (playerHealth != null)
                {
                    // --- DÜZELTME BURADA ---

                    // Yön Hesaplama: Düþmandan -> Oyuncuya doðru
                    Vector3 pushDirection = (collision.transform.position - transform.position).normalized;

                    // Vuruþ Noktasý: Çarpýþmanýn olduðu ilk nokta
                    Vector3 contactPoint = collision.contacts[0].point;

                    // Yeni sisteme uygun olarak 4 parametre gönderiyoruz:
                    // (Hasar, Çarpýþma Noktasý, Ýtme Yönü, Ýtme Gücü)
                    playerHealth.TakeDamage(damage, contactPoint, pushDirection, pushForce);

                    // -----------------------

                    lastAttackTime = Time.time;
                    Debug.Log("Isýrýldý!");
                }
            }
        }
    }
}