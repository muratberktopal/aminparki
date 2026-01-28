using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f; // Kameranýn gecikme süresi (0.1 - 0.3 arasý iyidir)
    public Vector3 offset; // Editörden ayarlayabileceðin mesafe (Örn: X:0, Y:10, Z:-8)

    void LateUpdate()
    {
        if (target == null) return;

        // Kameranýn gitmek istediði hedef pozisyon
        Vector3 desiredPosition = target.position + offset;

        // Mevcut pozisyondan hedefe yumuþak geçiþ (Lerp yerine SmoothDamp daha iyidir ama Lerp basittir)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        // Kameranýn sürekli karaktere bakmasýný istiyorsan açabilirsin, ama izometrikte genelde sabit açý iyidir.
        // transform.LookAt(target); 
    }
}