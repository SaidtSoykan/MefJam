using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinisher : MonoBehaviour
{
    [Header("Dönülecek Sahne")]
    public string hubSceneName = "WelcomeScene"; // TPS Hub sahnemizin adý

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        // 1. Mevcut ilerlemeyi öðren
        int currentProgress = PlayerPrefs.GetInt("PlayerLevel", 1);

        // 2. Ýlerlemeyi bir artýr (Level 1 bittiyse 2 yap)
        // Ancak en fazla 4 olsun (3 level var, 4 oyun bitti demek)
        if (currentProgress < 4)
        {
            PlayerPrefs.SetInt("PlayerLevel", currentProgress + 1);
            PlayerPrefs.Save(); // Kaydetmeyi unutma
        }

        Debug.Log("Level Tamamlandý! Yeni Hedef Level: " + (currentProgress + 1));

        // 3. Mouse kilidini Hub sahnesi için ayarla (TPS olduðu için kilitli)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. Hub Sahnesine (WelcomeScene) geri dön
        SceneManager.LoadScene(hubSceneName);
    }
}