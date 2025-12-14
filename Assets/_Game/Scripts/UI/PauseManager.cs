using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI Elemanlarý")]
    public GameObject pauseMenuPanel;
    public GameObject pauseButton;
    public GameObject optionsPanel;

    [Header("Sahne Ayarlarý")]
    // Bu kutucuðu Inspector'da göreceksin. 
    // TPS sahnesinde ÝÞARETLE, Mini Game sahnesinde ÝÞARETÝ KALDIR.
    public bool lockCursorOnResume = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Pause modunda mouse her zaman serbest olmalý (Menüyü kullanmak için)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        pauseButton.SetActive(true);

        Time.timeScale = 1f;
        GameIsPaused = false;

        // --- DEÐÝÞÝKLÝK BURADA ---
        // Eðer bu bir TPS sahnesiyse kilitle, Mini Game ise serbest býrak
        if (lockCursorOnResume)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Mini game için serbest kalmaya devam etsin
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void OpenOptions()
    {
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}