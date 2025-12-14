using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial State")]
    [SerializeField] private int stageIndex = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject space;
    [SerializeField] private TTPlantTimeLoop plant;

    [Header("Flags Exposed To Game")]
    public bool isGrowingOn { get; set; }

    private void Start()
    {
        RunTutorial().Forget();
    }

    /// <summary>
    /// Call this when the player completes the current tutorial objective
    /// </summary>
    public void AdvanceStage()
    {
        stageIndex++;
    }

    /// <summary>
    /// Main tutorial flow — single sequential UniTask method
    /// </summary>
    private async UniTaskVoid RunTutorial()
    {
        // =========================
        // STAGE 0 — Introduction
        // =========================
        isGrowingOn = false;
        isHarvestableOn = false;
        isInputOn = false;
        isProgressOn = false;
        ShowMessage("İlk ritüeline hoşgeldin...");
        
        await WaitForButtonPress(KeyCode.Space);

       
        isGrowingOn = true;
        plant.IsGrowing = true;
        ShowMessage("Ritüel sırasında zaman çok hızlı akmaya başlar ve bitkiler normalden daha hızlı büyür");
        await UniTask.WaitUntil(() => plant.timelinePercent >= 55f);
        isGrowingOn = false;
        await WaitForButtonPress(KeyCode.Space);
        
        ShowMessage("Her bitkiyi kullanabileceğin kısa bir olgunluk aralığı var");
        
        await WaitForButtonPress(KeyCode.Space);
        
        ShowMessage("ve zaman bu kadar hızlı akarken kaçırman olası");
        
        await WaitForButtonPress(KeyCode.Space);
        isGrowingOn = true;
        plant.IsGrowing = true;
        
        await UniTask.WaitUntil(() => plant.timelinePercent >= 90f);
        isGrowingOn = false;
        ShowMessage("Ancak burada zamanı geri alman mümkün");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Zamanı geri almak istediğin bitkiye sol mouse butonuyla tıkla");
        isInputOn = true;
        await UniTask.WaitUntil(() => plant.timelinePercent < 10f);
        isInputOn = false;
        plant.instability = (float)(0.8 * plant.instabilityMax);
        ShowMessage("Güçlerini bitkiler üzetrinde kullandığında onları dengesizlestirirsin");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Eğer bir bitki çok dengesizleşirse gelişimi bozulur ve baştan başlar");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Bunu önlemek için denegsizliği absorbe edebilirsin");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Dengesizliği absorbe etmek istediğin bitkiye sağ mouse butonuyla tıkla");
        isInputOn = true;
        await UniTask.WaitUntil(() => plant.instability <= 0.1f);
        isInputOn = false;
        ShowMessage("Absorbe ettiğin bitkinin dengesizliği senin üzerinde birikir");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Eğer çok fazla dengesizliğe sahip olursan tüm ritüel sıfırlanır");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Son olarak bazı bitkilerin toplanabilmesi için zamanda birden fazla yönde hareket etmesi gerekir");
        await WaitForButtonPress(KeyCode.Space);
        ShowMessage("Yeşil aralığa geldiklerinde bir sonraki olgunluk adımına geçerler");
        isGrowingOn = true;
        isProgressOn = true;
        await UniTask.WaitUntil(() => plant.timelinePercent >= 75f);
        isGrowingOn = false;
        isProgressOn = false;
        ShowMessage("Bir sonraki aralığa ulaşmak için zamanı geri alman gerekebilir");
        isInputOn = true;
        await UniTask.WaitUntil(() => plant.timelinePercent <= 15f);
        isInputOn = false;
        isGrowingOn = false;
        plant.particleController.GiveGameEndReward();
        plant._MeshControllerrenderer.QuicklyFadeOut(plant.timelineSlider);
        ShowMessage("Tebrikler! İlk ritüelini başarıyla tamamladın.");
        EndTutorial();
    }

    public bool isInputOn { get; set; }

    public bool isHarvestableOn { get; set; }
    public bool isProgressOn { get; set; }

    private async UniTask WaitForStage(int targetStage)
    {
        await UniTask.WaitUntil(() => stageIndex >= targetStage);
    }

    private void ShowMessage(string message)
    {
        if (tutorialText == null)
            return;

        tutorialText.text = message;
    }
    
    private async UniTask WaitForButtonPress(KeyCode key)
    {
        space.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => Input.GetKeyDown(key),
            cancellationToken: this.GetCancellationTokenOnDestroy());
        space.gameObject.SetActive(false);
    }

    private void EndTutorial()
    {
        RitualInputHandler player = FindObjectOfType<RitualInputHandler>();
        player.isRitualOn = false;
    }
}
