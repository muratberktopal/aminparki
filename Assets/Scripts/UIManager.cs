using UnityEngine;
using TMPro; // TextMeshPro kütüphanesi

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Her yerden eriþim için
    public TextMeshProUGUI ammoText;  // Ekrandaki yazý

    private void Awake()
    {
        // Singleton yapýsý (Tek yönetici olsun)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo, bool isReloading)
    {
        if (ammoText == null) return;

        if (isReloading)
        {
            ammoText.text = "RELOADING...";
            ammoText.color = Color.red; // Kýrmýzý uyarý
            ammoText.fontSize = 40;     // Biraz küçült sýðsýn
        }
        else
        {
            ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();
            ammoText.fontSize = 50;

            // Mermi bittiyse Kýrmýzý, azsa Sarý, doluysa Beyaz
            if (currentAmmo == 0) ammoText.color = Color.red;
            else if (currentAmmo <= 2) ammoText.color = Color.yellow;
            else ammoText.color = Color.white;
        }
    }
}