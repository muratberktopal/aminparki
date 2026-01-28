using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 12.0f;      // Hızı biraz artırdık
    public float turnSpeed = 25f;        // Dönüş hızı daha keskin olsun
    public float moveSmoothTime = 0.1f;  // AKIŞKANLIK AYARI (Düşük = Sert, Yüksek = Kaygan)

    [Header("Dash Ayarları")]
    public float dashSpeed = 35f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;

    [Header("Efektler")]
    public TrailRenderer dashTrail;

    private Rigidbody rb;
    private Vector3 movementInput;
    private Vector3 currentVelocity;     // Yumuşatma hesaplaması için gerekli
    private Vector3 smoothMoveVelocity;  // Anlık yumuşatılmış hız vektörü

    private Camera mainCam;

    // Dash Durumları
    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;

        if (dashTrail != null)
            dashTrail.emitting = false;
    }

    void Update()
    {
        // Girdi Al
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(h, 0f, v).normalized;

        // Mouse'a Dön
        TurnPlayerToMouse();

        // Dash Kontrolü (Space)
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown)
        {
            if (movementInput.magnitude > 0)
            {
                StartDash();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            DashMove();
        }
        else
        {
            SmoothMove(); // Yeni akışkan hareket fonksiyonu
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
        if (dashTrail != null) dashTrail.emitting = true;
    }

    void DashMove()
    {
        // Dash atarken yumuşatma olmaz, ZINK diye gider
        rb.linearVelocity = movementInput * dashSpeed;

        dashTimeLeft -= Time.fixedDeltaTime;

        if (dashTimeLeft <= 0)
        {
            EndDash();
        }
    }

    void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector3.zero;
        if (dashTrail != null) dashTrail.emitting = false;
    }

    // --- İŞTE SİHİRLİ DOKUNUŞ BURADA ---
    void SmoothMove()
    {
        // Hedef hızımız (Yön * Hız)
        Vector3 targetVelocity = movementInput * moveSpeed;

        // SmoothDamp: Mevcut hızdan hedef hıza "SmoothTime" süresinde yumuşakça geç
        Vector3 smoothedVelocity = Vector3.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref currentVelocity,
            moveSmoothTime
        );

        // Y eksenini (zıplama/düşme) koru ki karakter havada asılı kalmasın
        smoothedVelocity.y = rb.linearVelocity.y;

        // Yeni hızı uygula
        rb.linearVelocity = smoothedVelocity;
    }

    void TurnPlayerToMouse()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            // Dönüşü de biraz daha yumuşak ama seri yap
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
}