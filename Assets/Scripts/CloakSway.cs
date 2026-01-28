using UnityEngine;

public class CloakSway : MonoBehaviour
{
    [Header("Sallanma Ayarlarý")]
    public float swayAmount = 10f;  // Açýyý küçülttüm (Daha aðýr dursun)
    public float swaySpeed = 2f;    // Hýzý düþürdüm (Ýpek deðil deri bu)

    [Header("Sallanma Yönü (1 yaz, diðerleri 0 olsun)")]
    public Vector3 swayAxis = new Vector3(1, 0, 0); // X ekseninde salla (Varsayýlan)

    [Header("Referans")]
    public Rigidbody playerRb;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
        if (playerRb == null) playerRb = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        float movementFactor = 0.2f; // Dururkenki hafif kýpýrtý

        if (playerRb != null)
        {
            // Hýza göre artýr ama fok balýðý olmasýn diye maksimumu kýstým (3f)
            movementFactor = Mathf.Clamp(playerRb.linearVelocity.magnitude, 0.2f, 3f);
        }

        // Sinüs dalgasý
        float angle = Mathf.Sin(Time.time * swaySpeed * movementFactor) * swayAmount;

        // Seçtiðin eksende salla
        Quaternion targetRotation = Quaternion.AngleAxis(angle, swayAxis);

        // Yavaþça (Lerp ile) yeni açýya geç ki titreme yapmasýn
        transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation * targetRotation, Time.deltaTime * 5f);
    }
}