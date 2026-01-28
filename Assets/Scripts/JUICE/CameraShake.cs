using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Tek satýrda her yerden ulaþabilmek için (Singleton)
    public static CameraShake Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Bu fonksiyonu çaðýrdýðýnda ekran sallanacak
    // duration: Ne kadar sürsün? (0.1sn, 0.5sn vb.)
    // magnitude: Ne kadar þiddetli sallansýn?
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Kamerayý rastgele saða sola titret
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null; // Bir sonraki kareyi bekle
        }

        // Titreme bitince kamerayý eski yerine koy
        transform.localPosition = originalPos;
    }
}