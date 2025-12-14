using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletionManager : MonoBehaviour
{
    [Header("Hedef Script")]
    // DÝKKAT: 'RitualScriptAdi' yerine kendi scriptinin adýný yaz!
    public RitualInputHandler targetRitualScript;

    [Header("UI Elemanlarý")]
    public GameObject levelCompletePanel; // Hazýrladýðýmýz panel

    [Header("Ayarlar")]
    public string hubSceneName = "WelcomeScene"; // Dönülecek sahne

    private bool levelEnded = false; // Sürekli tetiklenmemesi için kontrol

    void Update()
    {
        // Level henüz bitmediyse VE Ritüel Scripti tanýmlýysa kontrol et
        if (!levelEnded && targetRitualScript != null)
        {
            // Senin deðiþkenin false olduðu aný yakalýyoruz
            if (targetRitualScript.isRitualOn == false)
            {
                ShowLevelCompleteUI();
            }
        }
    }

    void ShowLevelCompleteUI()
    {
        levelEnded = true; // Bir daha çalýþmasýný engelle

        // 1. Paneli Aç
        levelCompletePanel.SetActive(true);

        // 2. Mouse Ýmlecini Serbest Býrak (Butona basabilmek için þart!)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. (Ýsteðe Baðlý) Oyunu durdurmak istersen:
        // Time.timeScale = 0f; 
    }

    // Bu fonksiyonu "Continue" butonuna baðlayacaðýz
    public void OnContinueClicked()
    {
        // Zamaný tekrar normalleþtir (Eðer durdurduysan)
        Time.timeScale = 1f;

        // --- ÝLERLEMEYÝ KAYDETME ---

        // 1. Mevcut leveli öðren
        int currentProgress = PlayerPrefs.GetInt("PlayerLevel", 1);

        // 2. Leveli bir artýr (Max 4 level varsayýmýyla)
        if (currentProgress < 4)
        {
            PlayerPrefs.SetInt("PlayerLevel", currentProgress + 1);
            PlayerPrefs.Save();
        }

        Debug.Log("Ýlerleme Kaydedildi. Yeni Level: " + (currentProgress + 1));

        // 3. WelcomeScene'e geri dön
        SceneManager.LoadScene(hubSceneName);
    }
}