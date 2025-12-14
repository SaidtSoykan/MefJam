using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçiþleri için þart

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false; // Diðer scriptlerden eriþmek istersen diye static yaptýk

    [Header("UI Elemanlarý")]
    public GameObject pauseMenuPanel;
    public GameObject pauseButton; 
    public GameObject optionsPanel;
    void Awake()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
    }
    void Update()
    {
        // ESC tuþuna basýnca da açýlsýn
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // --- BUTON FONKSÝYONLARI ---

    public void Pause()
    {
        pauseMenuPanel.SetActive(true); // Menüyü aç
        pauseButton.SetActive(false);   // Küçük butonu gizle

        Time.timeScale = 0f; // ZAMANI DURDUR
        GameIsPaused = true;

        // Mouse imlecini serbest býrak ve göster
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(false); // Eðer ayarlar açýksa onu da kapat
        pauseButton.SetActive(true);

        Time.timeScale = 1f; // ZAMANI TEKRAR AKIT
        GameIsPaused = false;

        // Mouse imlecini tekrar kilitle ve gizle (TPS oyunu olduðu için)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        // Ana menüye dönerken zamanýn donuk kalmadýðýndan emin olmalýyýz
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Build Settings'de Main Menu genelde 0. sýradadýr
    }

    public void OpenOptions()
    {
        pauseMenuPanel.SetActive(false); // Pause menüsünü gizle
        optionsPanel.SetActive(true);    // Ayarlarý aç
    }

    public void CloseOptions() // Ayarlar içindeki "Geri" butonu için
    {
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}