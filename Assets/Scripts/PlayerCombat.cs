using UnityEngine;

// Bu satýr, scripti eklediðin objeye otomatik olarak AudioSource ekler. Hata almaný önler.
[RequireComponent(typeof(AudioSource))]
public class PlayerCombat : MonoBehaviour
{
    [Header("--- SES VE EFEKTLER ---")]
    private AudioSource audioSource; // Sesi çalacak bileþen
    public AudioClip revolverSfx;    // Altýpatlar Sesi
    public AudioClip shotgunSfx;     // Pompalý Sesi

    public ParticleSystem leftMuzzleVFX;   // Sol el ýþýðý
    public ParticleSystem rightMuzzleVFX;  // Sað el ýþýðý
    public ParticleSystem shotgunMuzzleVFX;// Pompalý ýþýðý

    [Header("--- SÝLAH 1: ÇÝFT ALTIPATLAR (Sol Týk) ---")]
    public GameObject revolverBulletPrefab; // Mermi Prefabý
    public Transform leftHandPoint;         // Sol namlu ucu
    public Transform rightHandPoint;        // Sað namlu ucu

    public float revolverFireRate = 0.25f;  // Ateþ hýzý (Týk-Týk-Týk)
    public int revolverDamage = 2;          // Hasar
    public float revolverPushForce = 3f;    // Ýtme Gücü (DÜÞÜK - Sadece titretir)

    // Altýpatlar iç mantýðý
    private float nextRevolverTime;
    private bool useRightHand = true;       // Sýra sað elde mi?

    [Header("--- SÝLAH 2: DÖRT NAMLULU POMPALI (Sað Týk) ---")]
    public GameObject shotgunPelletPrefab;  // Saçma Prefabý (Ayný mermi veya ufak hali)
    public Transform shotgunPoint;          // Göðüs/Sýrt namlu ucu

    public float shotgunCooldown = 1.2f;    // Bekleme süresi
    public int pelletCount = 10;            // Kaç saçma çýksýn? (4 namlu için 8-12 ideal)
    public float spreadAngle = 25f;         // Saçýlma açýsý
    public int shotgunDamage = 2;           // Tane baþýna hasar
    public float shotgunPushForce = 35f;    // Ýtme Gücü (YÜKSEK - Uçurur)

    // Pompalý iç mantýðý
    private float nextShotgunTime;

    [Header("--- YETENEK: PARRY (F Tuþu) ---")]
    public float parryRange = 3.0f;
    public float parryCooldown = 1.0f;
    public LayerMask enemyLayer;
    private float nextParryTime;

    void Start()
    {
        // Ses bileþenini otomatik al
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 1. REVOLVER (Basýlý Tutma)
        if (Input.GetMouseButton(0) && Time.time >= nextRevolverTime)
        {
            ShootRevolver();
        }

        // 2. SHOTGUN (Tek Týk)
        if (Input.GetMouseButtonDown(1) && Time.time >= nextShotgunTime)
        {
            ShootShotgun();
        }

        // 3. PARRY (Tek Týk)
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextParryTime)
        {
            DoParry();
        }
    }

    void ShootRevolver()
    {
        nextRevolverTime = Time.time + revolverFireRate;

        // A) SES ÇAL (Ses þiddeti 0.6 - Kulak týrmalamasýn)
        if (revolverSfx != null) audioSource.PlayOneShot(revolverSfx, 0.6f);

        // B) HANGÝ EL ATEÞ EDECEK?
        Transform currentPoint = useRightHand ? rightHandPoint : leftHandPoint;
        ParticleSystem currentVFX = useRightHand ? rightMuzzleVFX : leftMuzzleVFX;

        // C) EFEKT OYNAT
        if (currentVFX != null) currentVFX.Play();

        // D) MERMÝYÝ YARAT
        CreateBullet(revolverBulletPrefab, currentPoint.position, currentPoint.rotation, revolverDamage, revolverPushForce);

        // E) EL DEÐÝÞTÝR
        useRightHand = !useRightHand;

        // F) KAMERA TÝTREMESÝ (Hafif)
        if (CameraShake.Instance != null)
            StartCoroutine(CameraShake.Instance.Shake(0.04f, 0.025f));
    }

    void ShootShotgun()
    {
        nextShotgunTime = Time.time + shotgunCooldown;

        // A) SES ÇAL (Gürültülü - 1.0)
        if (shotgunSfx != null) audioSource.PlayOneShot(shotgunSfx, 1.0f);

        // B) EFEKT OYNAT
        if (shotgunMuzzleVFX != null) shotgunMuzzleVFX.Play();

        // C) MERMÝLERÝ SAÇ (Döngü)
        for (int i = 0; i < pelletCount; i++)
        {
            // Rastgele saçýlma açýsý hesapla
            Quaternion randomRot = shotgunPoint.rotation * Quaternion.Euler(0, Random.Range(-spreadAngle, spreadAngle), 0);

            CreateBullet(shotgunPelletPrefab, shotgunPoint.position, randomRot, shotgunDamage, shotgunPushForce);
        }

        // D) KAMERA TÝTREMESÝ (SERT!)
        if (CameraShake.Instance != null)
            StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.6f));

        Debug.Log("BOOM! Shotgun Ateþlendi.");
    }

    void DoParry()
    {
        nextParryTime = Time.time + parryCooldown;

        // Ekranda hafif bir sarsýntý olsun ki parry attýðýmýz belli olsun
        if (CameraShake.Instance != null) StartCoroutine(CameraShake.Instance.Shake(0.1f, 0.3f));

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, parryRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            HealthSystem hp = enemy.GetComponent<HealthSystem>();
            if (hp != null)
            {
                // Hasar 0 ama Ýtme Gücü 40 (Çok sert it)
                Vector3 pushDir = (enemy.transform.position - transform.position).normalized;
                hp.TakeDamage(0, enemy.transform.position, pushDir, 40f);
                Debug.Log("Parry Baþarýlý: " + enemy.name);
            }
        }
    }

    // Kod tekrarýný önlemek için tek fonksiyon
    void CreateBullet(GameObject prefab, Vector3 pos, Quaternion rot, int dmg, float push)
    {
        GameObject bullet = Instantiate(prefab, pos, rot);

        // BulletMove scriptine ulaþýp deðerleri üzerine yazýyoruz
        BulletMove bm = bullet.GetComponent<BulletMove>();
        if (bm != null)
        {
            bm.damage = dmg;
            bm.pushForce = push; // Buradaki deðer inspector'dan gelen deðer olacak
        }
    }

    // Parry alanýný Editörde görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward, parryRange);
    }
}