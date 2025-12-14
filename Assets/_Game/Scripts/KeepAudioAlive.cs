using UnityEngine;

public class KeepAudioAlive : MonoBehaviour
{
    // Bu deðiþkene "static" diyoruz, yani sahneden baðýmsýz, oyunun hafýzasýnda durur.
    private static KeepAudioAlive instance = null;

    void Awake()
    {
        // Eðer hafýzada daha önce oluþturulmuþ bir müzik çalar varsa:
        if (instance != null && instance != this)
        {
            // Yeni oluþturulan (bu) objeyi yok et.
            // Çünkü zaten halihazýrda çalan bir müzik var.
            Destroy(this.gameObject);
            return;
        }
        else
        {
            // Eðer ilk defa çalýþýyorsa (Oyun ilk açýldýðýnda):
            instance = this;
        }

        // Sihirli komut: Sahne deðiþse bile bu objeyi yok etme!
        DontDestroyOnLoad(this.gameObject);
    }
}