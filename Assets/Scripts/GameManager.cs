using UnityEngine;
using TMPro; // DÝKKAT: Bunu eklemezsen TextMeshPro çalýþmaz!
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Ekrandaki yazýya ulaþmak için
    int score = 0; // Arka plandaki matematiksel skor
    public GameObject gameOverPanel;
    // Bu fonksiyonu düþmanlar ölünce çaðýracak
    public void AddScore(int point)
    {
        // 1. Skoru artýr
        score += point;

        // 2. Ekrana yazdýr (Sayýyý metne çeviriyoruz)
        scoreText.text = "Skor: " + score.ToString();
    }

    public void GameOver()
    {
        // Gizlediðimiz paneli aç
        gameOverPanel.SetActive(true);

        // Zamaný durdur (Arka planda oyun akmasýn)
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Zamaný tekrar akýtmaya baþla (Yoksa yeni oyun donuk baþlar!)
        Time.timeScale = 1;

        // Þu anki sahneyi (Leveli) baþtan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}