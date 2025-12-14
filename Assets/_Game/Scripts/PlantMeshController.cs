using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlantMeshController : MonoBehaviour
{
    [Header("Meshes")]
    [SerializeField] GrowthMesh[] meshes;

    [Header("Scale")]
    [SerializeField] float minScale = 1;
    [SerializeField] float maxScale = 10;

    [Header("Harvest")]
    [SerializeField] HarvestStep harvestWindow;
    HarvestStep pendingHarvestStep;
    bool transitioningHarvestStep;
    float transitionStartPercent;

    
    [SerializeField] float bufferPercent = 0.1f;
    
    [SerializeField] Color harvestColor = Color.red;
    [SerializeField] Color defaultColor = Color.white;
    
    bool inBuffer;
    bool inHarvest;
    GrowthMesh maxGrowthMesh;

    public void ResetAndAssignGrowth(HarvestStep harvestStep)
    {
        AssignGrowthPercentages();
        ApplyGrowth(0f);
        harvestWindow = harvestStep;
    }

    public void ApplyGrowth(float value01)
    {
        value01 = Mathf.Clamp01(value01);

        UpdateMeshes(value01);
        CheckHarvestWindow(value01);
    }
    
    void AssignGrowthPercentages()
    {
        int maxIndex = Random.Range(0, meshes.Length);

        for (int i = 0; i < meshes.Length; i++)
        {
           meshes[i].renderer.material.color = defaultColor ;

            if (i == maxIndex)
            {
                meshes[i].growthMultiplier = 1f;
                maxGrowthMesh = meshes[i];
            }
            else
            {
                meshes[i].growthMultiplier = Random.Range(0.5f, 0.9f);
            }
        }
    }
    
    public void UpdateMeshes(float value01)
    {
        value01 = Mathf.Clamp01(value01);

        // Triangle wave: 0 → 1 → 0
        float loopValue = 1f - Mathf.Abs(value01 * 2f - 1f);

        foreach (var mesh in meshes)
        {
            float scaledValue = loopValue * mesh.growthMultiplier;

            Vector3 scale = mesh.renderer.transform.localScale;
            scale.z = Mathf.LerpUnclamped(minScale, maxScale, scaledValue);
            mesh.renderer.transform.localScale = scale;
        }
    }

    void CheckHarvestWindow(float value01)
    {
        float percent = value01 * 100f;

        float bufferMin = harvestWindow.minPercent - bufferPercent * 100f;
        float bufferMax = harvestWindow.maxPercent + bufferPercent * 100f;

        bool insideBuffer = percent >= bufferMin && percent <= bufferMax;
        bool insideHarvest = harvestWindow.Contains(percent);

        UpdateWinningMeshColor(percent, bufferMin, bufferMax, insideHarvest);

        // Optional logical events (no visuals here)
        if (insideHarvest && !harvestWindow.isActive)
        {
            harvestWindow.isActive = true;
            OnHarvestable();
        }
        else if (!insideHarvest && harvestWindow.isActive)
        {
            harvestWindow.isActive = false;
            PostHarvestable();
        }
    }

    void UpdateWinningMeshColor(
        float percent,
        float bufferMin,
        float bufferMax,
        bool insideHarvest)
    {
        if (maxGrowthMesh == null || maxGrowthMesh.renderer == null)
            return;

        Material mat = maxGrowthMesh.renderer.material;

        if (transitioningHarvestStep)
        {
            // Fade red → default based on distance from transition start
            float t = Mathf.InverseLerp(
                transitionStartPercent,
                transitionStartPercent + bufferPercent * 100f,
                percent
            );

            mat.color = Color.Lerp(harvestColor, defaultColor, t);

            // Once fully faded out → commit new harvest step
            if (t >= 1f)
            {
                harvestWindow = pendingHarvestStep;
                transitioningHarvestStep = false;
                harvestWindow.isActive = false;
            }

            return;
        }
        
        // 1️⃣ Inside harvest → solid red
        if (insideHarvest)
        {
            mat.color = harvestColor;
            return;
        }

        // 2️⃣ Inside buffer → blend based on proximity
        if (percent >= bufferMin && percent <= harvestWindow.minPercent)
        {
            // Approaching harvest (buffer in)
            float t = Mathf.InverseLerp(bufferMin, harvestWindow.minPercent, percent);
            mat.color = Color.Lerp(defaultColor, harvestColor, t);
            return;
        }

        if (percent >= harvestWindow.maxPercent && percent <= bufferMax)
        {
            // Leaving harvest (buffer out)
            float t = Mathf.InverseLerp(bufferMax, harvestWindow.maxPercent, percent);
            mat.color = Color.Lerp(defaultColor, harvestColor, t);
            return;
        }

        // 3️⃣ Outside buffer → normal color
        mat.color = defaultColor;
    }
    
    public void ChangeHarvestStep(HarvestStep newStep, float currentValue01)
    {
        pendingHarvestStep = newStep;
        transitioningHarvestStep = true;

        // Capture where we are so we can fade OUT smoothly
        transitionStartPercent = currentValue01 * 100f;
    }

    
    void OnHarvestable()
    {
       
    }

    void PostHarvestable()
    {
        
    }
    
    [System.Serializable]
    public class GrowthMesh
    {
        public Renderer renderer;
        [HideInInspector] public float growthMultiplier;
    }
    
    public void QuicklyFadeOut(Slider timelineSlider)
    {
        float target = timelineSlider.value < 0.5f ? 0f : 1f;
        float xx = timelineSlider.value ;
        DOTween.To(
            () => xx,
            x =>
            {
                xx = x;
                UpdateMeshes(x);
            },
            target,
            1f
        ).SetEase(Ease.InOutSine);
    }
}
