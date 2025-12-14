using UnityEngine;
using UnityEngine.SceneManagement;

public class HubPortal : MonoBehaviour
{
    [Header("UI Ayarlarý")]
    public GameObject interactUI; // "E'ye Bas" yazýsý

    [Header("Sahne Ýsimleri (Build Settings ile Ayný Olmalý)")]
    public string level1SceneName = "Level1";
    public string level2SceneName = "Level2";
    public string level3SceneName = "Level3";
    public string endSceneName = "StartScene"; // Oyun bitince nereye gitsin?

    private bool isPlayerNearby = false;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            LoadCorrectLevel();
        }
    }

    void LoadCorrectLevel()
    {
        // 1. Hafýzadaki level bilgisini al
        int currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);

        // --- YENÝ EKLENEN KISIM: DÖNGÜ KONTROLÜ ---
        // Eðer level sayýsý 3'ten büyükse (yani oyun bitmiþse),
        // Level'i zorla 1 yap ve hafýzayý güncelle.
        if (currentLevel > 3)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("PlayerLevel", 1);
            PlayerPrefs.Save(); // Kaydetmeyi unutma!
            Debug.Log("Oyun bittiði için Level 1'e dönülüyor...");
        }
        // ------------------------------------------

        string sceneToLoad = "";

        switch (currentLevel)
        {
            case 1:
                sceneToLoad = level1SceneName;
                break;
            case 2:
                sceneToLoad = level2SceneName;
                break;
            case 3:
                sceneToLoad = level3SceneName;
                break;
            default:
                // Artýk buraya düþmesi çok zor ama güvenlik için kalsýn
                sceneToLoad = level1SceneName;
                break;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }

    // --- Trigger Giriþ Çýkýþ Kodlarý ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}