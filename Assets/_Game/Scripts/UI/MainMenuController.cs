using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    public AudioMixer mainMixer;

    public void PlayGame()
    {
        // YENÝ EKLENEN SATIR: Oyunu en baþtan baþlat (Level 1'e çek)
        PlayerPrefs.SetInt("PlayerLevel", 1);
        PlayerPrefs.Save();

        // WelcomeScene sahnesini yükle (Build Index veya Ýsimle)
        SceneManager.LoadScene("WelcomeScene");
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çýkýldý!");
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}