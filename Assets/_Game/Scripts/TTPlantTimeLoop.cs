using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class TTPlantTimeLoop : PlantTimeLoop
{
    public void Start()
    {
        player = FindObjectOfType<RitualInputHandler>();
        Canvas.ForceUpdateCanvases();
        isActive = true;
        IsGrowing = true;
        currentStepIndex = 0;
        print(harvestSteps.Count);
        print(currentStepIndex);
        _MeshControllerrenderer.ResetAndAssignGrowth(harvestSteps[currentStepIndex]);
        timelinePercent = 0;
        instability = 0;
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
    public TutorialManager ttManager;
    
    public TutorialManager TutorialManager
    {
        get
        {
            if (ttManager == null)
            {
                ttManager = FindObjectOfType<TutorialManager>();
            }
            return ttManager;
        }
        set { ttManager = value; }
    }
    
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
            if(TutorialManager.isGrowingOn) 
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
        if(!TutorialManager.isProgressOn)
            return;
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