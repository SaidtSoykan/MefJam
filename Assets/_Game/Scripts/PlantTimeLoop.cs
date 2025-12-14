using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

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
    [SerializeField] public PlantMeshController _MeshControllerrenderer;
    [SerializeField] public PlantParticleController particleController;
    
    private bool isHavestable = false;
    public bool isActive { get; set; }
    public bool IsHarvestable
    {
        get => isHavestable;
        set
        {
            isHavestable = value;
            //plantImage.color= isHavestable ? Color.green : Color.red;
            transform.parent.GetComponent<RitualInputHandler>().CheckRitualEnd();
        }
    } 

    [Header("=== UI (Optional) ===")]
    public Slider timelineSlider;
    public Image instabilityFill;
    public Image harvestImage;
    public Image plantImage;
    private RitualInputHandler player;
    // Runtime state
   public bool _isReversing;
   public bool _isGrowing;
   public bool _isBeingAbsorbed;

    public bool IsReversing
    {
        get => _isReversing;
        private set
        {
            if (value == _isReversing)
                return;

            _isReversing = value;
            if (IsGrowing == value)
            {
                IsGrowing = !value;
            }
            particleController.Play(value, PlantParticleController.PlantState.Reversing);
        }
    }

    public bool IsGrowing
    {
        get => _isGrowing;
        private set
        {
            if (value == _isGrowing)
                return;

            _isGrowing = value;
            if (IsReversing == value)
            {
                IsReversing = !value;
            }
            particleController.Play(value, PlantParticleController.PlantState.Growing);
        }
    }

    public bool IsBeingAbsorbed
    {
        get => _isBeingAbsorbed;
        set
        {
            if (value == _isBeingAbsorbed)
                return;

            _isBeingAbsorbed = value;
            particleController.Play(value, PlantParticleController.PlantState.Absorbing);
        }
    }

    
    private float explodeCooldownTimer = 0f;

    // ------------------ UPDATE ------------------

    public void Start()
    {
        player = FindObjectOfType<RitualInputHandler>();
        Canvas.ForceUpdateCanvases();
        isActive = true;
        IsGrowing = true;
        _MeshControllerrenderer.ResetAndAssignGrowth(harvestSteps[currentStepIndex]);
        timelinePercent = 0;
        instability = 0;
        currentStepIndex = 0;
        PositionHarvestImage();
    }

    private void Update()
    {
        if (!isActive)
            return;
        float dt = Time.deltaTime;

        UpdateCooldown(dt);
        UpdateTimeline(dt);
        UpdateInstability(dt);
        UpdateAbsorption(dt);
        CheckHarvestProgress();
        UpdateUI();
        _MeshControllerrenderer.ApplyGrowth(timelineSlider.value);
    }

    private void UpdateAbsorption(float dt)
    {
        if(!IsBeingAbsorbed)
            return;
        float absorbed = AbsorbInstability(
            dt,
            player.playerAbsorbCapacityPerSecond
        );
        player.AddPlayerInstability(absorbed);
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
            if (currentStepIndex < harvestSteps.Count - 1)
            {
                currentStepIndex++;
                _MeshControllerrenderer.ChangeHarvestStep(harvestSteps[currentStepIndex],timelineSlider.value );
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
        _MeshControllerrenderer.ResetAndAssignGrowth(harvestSteps[currentStepIndex]);
    }

    // ------------------ INPUT API ------------------

    public void SetReversing(bool value)
    {
        print(value ? "reversed" : "not reversed");
        IsReversing = value;
    }
    public void SetAbsorbing(bool value)
    {
        IsBeingAbsorbed = value;
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

        float sliderWidth = sliderRect.rect.width-40f;

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
    
    public float TimelinePercent => timelinePercent;
    public float Instability => instability;
    
}