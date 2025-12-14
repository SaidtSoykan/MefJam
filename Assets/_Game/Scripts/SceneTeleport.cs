using UnityEngine;
using UnityEngine.SceneManagement; // Sahne deðiþimi için gerekli
using UnityEngine.UI; // UI iþlemleri için (Eðer TMPro kullanýyorsan TMPro kütüphanesini ekle)

public class SceneTeleport : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject interactUI; // Ekranda çýkacak "E'ye Bas" yazýsý
    public int sceneIndex = 2;    // Gidilecek sahnenin numarasý

    private bool isPlayerNearby = false; // Oyuncu alanda mý?

    void Start()
    {
        // Baþlangýçta yazýnýn kapalý olduðundan emin olalým
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        // Eðer oyuncu alandaysa VE 'E' tuþuna basarsa
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextLevel();
        }
    }

    // Oyuncu alana girdiðinde çalýþýr
    void OnTriggerEnter(Collider other)
    {
        // Giren obje "Player" etiketine sahipse
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null)
                interactUI.SetActive(true); // Yazýyý göster
        }
    }

    // Oyuncu alandan çýktýðýnda çalýþýr
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactUI != null)
                interactUI.SetActive(false); // Yazýyý gizle
        }
    }

    void LoadNextLevel()
    {
        // Sahne yüklenmeden hemen önce mouse'u serbest býrakýyoruz
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Zamaný normalleþtir
        Time.timeScale = 1f;

        // Sahneyi yükle
        SceneManager.LoadScene(sceneIndex);
    }
}