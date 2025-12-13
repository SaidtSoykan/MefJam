using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlantTimeLoop : MonoBehaviour
{
    [Header("=== TIME LOOP ===")]
    [Tooltip("Seconds to go from 0% to 100%")]
    public float loopDuration = 10f;

    [Tooltip("Multiplier applied to forward speed when reversing")]
    public float reverseSpeedMultiplier = 1.2f;

    [Range(0f, 100f)]
    [SerializeField] private float timelinePercent = 0f;

    [Header("=== INSTABILITY ===")]
    public float instabilityMax = 100f;
    public float instabilityGrowPerSecond = 10f;
    public float absorbRatePerSecond = 20f;

    [SerializeField] private float instability = 0f;

    [Header("Explosion Cooldown")]
    public float explodeCooldownSeconds = 1.0f;

    [Header("=== HARVEST PATTERN ===")]
    public List<HarvestStep> harvestSteps = new List<HarvestStep>();

    [SerializeField] private int currentStepIndex = 0;
    
    private bool isHavestable = false;
    public bool isActive { get; set; }
    public bool IsHarvestable
    {
        get => isHavestable;
        set
        {
            isHavestable = value;
            plantImage.color= isHavestable ? Color.green : Color.red;
            transform.parent.GetComponent<RitualInputHandler>().CheckRitualEnd();
        }

    } 

    [Header("=== UI (Optional) ===")]
    public Slider timelineSlider;
    public Image instabilityFill;
    public Image harvestImage;
    public Image plantImage;

    // Runtime state
    public bool IsReversing { get; private set; }
    private float explodeCooldownTimer = 0f;

    // ------------------ UPDATE ------------------

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        PositionHarvestImage();
        isActive = true;
    }

    private void Update()
    {
        if (!isActive)
            return;
        float dt = Time.deltaTime;

        UpdateCooldown(dt);
        UpdateTimeline(dt);
        UpdateInstability(dt);
        CheckHarvestProgress();
        UpdateUI();
    }
    

    // ------------------ TIME ------------------

    void UpdateTimeline(float dt)
    {
        float forwardSpeed = 100f / Mathf.Max(0.001f, loopDuration);
        float reverseSpeed = forwardSpeed * reverseSpeedMultiplier;

        if (IsReversing)
        {
            timelinePercent -= reverseSpeed * dt;
        }
        else
        {
            timelinePercent += forwardSpeed * dt;
        }

        // Clamp
        if (timelinePercent < 0f)
            timelinePercent = 0f;

        // Natural death
        if (timelinePercent >= 100f)
        {
            timelinePercent = 0f;
            ResetPatternOnly();
        }
    }

    // ------------------ INSTABILITY ------------------

    void UpdateInstability(float dt)
    {
        if (IsReversing && explodeCooldownTimer <= 0f)
        {
            instability += instabilityGrowPerSecond * dt;
        }

        if (instability >= instabilityMax)
        {
            Explode();
        }
    }

    public float AbsorbInstability(float dt, float playerCapacityPerSecond)
    {
        float amount = Mathf.Min(
            absorbRatePerSecond * dt,
            playerCapacityPerSecond * dt,
            instability
        );

        instability -= amount;
        return amount;
    }

    // ------------------ EXPLOSION ------------------

    void Explode()
    {
        instability = 0f;
        timelinePercent = 0f;
        ResetPatternOnly();
        explodeCooldownTimer = explodeCooldownSeconds;
    }

    void UpdateCooldown(float dt)
    {
        if (explodeCooldownTimer > 0f)
            explodeCooldownTimer -= dt;
    }

    // ------------------ HARVEST PATTERN ------------------

    void CheckHarvestProgress()
    {
        if(harvestSteps.Count == 0)
            return;
        HarvestStep step = harvestSteps[currentStepIndex];
        
        if (step.Contains(timelinePercent))
        {
            print("girdin bro");
            if (currentStepIndex < harvestSteps.Count - 1)
            {
                currentStepIndex++;
                PositionHarvestImage();
                return;
            }
            
            if (currentStepIndex >= harvestSteps.Count-1)
            {
                IsHarvestable = true;
            }
        }
        else
        {
            IsHarvestable = false;
        }
    }

    void ResetPatternOnly()
    {
        currentStepIndex = 0;
        PositionHarvestImage();
        IsHarvestable = false;
    }

    // ------------------ INPUT API ------------------

    public void SetReversing(bool value)
    {
        IsReversing = value;
    }

    // ------------------ UI ------------------

    void UpdateUI()
    {
        if (timelineSlider != null)
            timelineSlider.value = timelinePercent / 100f;

        if (instabilityFill != null)
            instabilityFill.color = new Color(instabilityFill.color.r , instabilityFill.color.g, instabilityFill.color.b, instability / instabilityMax);
    }
    
    
    public void PositionHarvestImage()
    {
        if(harvestSteps.Count-1 < currentStepIndex)
            return;
        HarvestStep step = harvestSteps[currentStepIndex];
        float minPercent = step.minPercent;
        float maxPercent = step.maxPercent;
            
        if (timelineSlider == null || harvestImage == null)
            return;

        RectTransform sliderRect = timelineSlider.GetComponent<RectTransform>();
        RectTransform imageRect = harvestImage.rectTransform;

        float sliderWidth = sliderRect.rect.width;

        // Safety clamps
        minPercent = Mathf.Clamp(minPercent, 0f, 100f);
        maxPercent = Mathf.Clamp(maxPercent, 0f, 100f);

        float minNorm = minPercent / 100f;
        float maxNorm = maxPercent / 100f;

        float rangeNorm = maxNorm - minNorm;
        float imageWidth = sliderWidth * rangeNorm;

        // Set image width
        imageRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            imageWidth
        );

        // Compute center position relative to slider
        float centerNorm = (minNorm + maxNorm) * 0.5f;
        float centerX = (centerNorm * sliderWidth) - (sliderWidth * 0.5f);

        Vector2 pos = imageRect.anchoredPosition;
        pos.x = centerX;
        imageRect.anchoredPosition = pos;
    }


    // ------------------ INSPECTOR HELPERS ------------------

    public float TimelinePercent => timelinePercent;
    public float Instability => instability;
}