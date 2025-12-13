using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class RitualInputHandler : MonoBehaviour
{
    [Header("Player Instability")]
    public float playerInstabilityMax = 100f;
    public float playerInstability = 0f;

    [Header("Absorption")]
    public float playerAbsorbCapacityPerSecond = 30f;

    private PlantTimeLoop selectedPlant;
    private List<PlantTimeLoop> plants = new List<PlantTimeLoop>();
    public bool isRitualOn { get; set; }
    void Update()
    {
        if(!isRitualOn)
            return;
        //fore collider systems
        //HandleSelection();
        //for keyboard plant control
        
        HandlePlantInput();
        //works in default ui system                                    
        HandleReverse();
        HandleAbsorb();
    }
    
    
    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PlantTimeLoop plant = hit.collider.GetComponentInParent<PlantTimeLoop>();
                if (plant != null)
                {
                    SelectPlant(plant);
                }
            }
        }
    }

    void HandleReverse()
    {
        if (selectedPlant == null) return;

        bool reversing = Input.GetMouseButton(0);
        selectedPlant.SetReversing(reversing);
    }

    void HandleAbsorb()
    {
        if (selectedPlant == null) return;
        if (!Input.GetMouseButton(1)) return;

        float dt = Time.deltaTime;

        float transferred = selectedPlant.AbsorbInstability(dt, playerAbsorbCapacityPerSecond);
        playerInstability += transferred;

        if (playerInstability >= playerInstabilityMax)
        {
            FailRitual();
        }
    }

    void SelectPlant(PlantTimeLoop plant)
    {
        if (selectedPlant != null)
            selectedPlant.SetReversing(false);

        selectedPlant = plant;
    }

    void FailRitual()
    {
        Debug.Log("RITUAL FAILED: Player instability maxed.");
        playerInstability = 0f;
        // Reset ritual here
    }
    public void SelectPlantFromUI(PlantTimeLoop plant)
    {
        if (selectedPlant != null)
            selectedPlant.SetReversing(false);

        selectedPlant = plant;
    }

    public void AddPlayerInstability(float amount)
    {
        playerInstability += amount;
        if (playerInstability >= playerInstabilityMax)
            FailRitual();
    }

    private void Awake()
    {
        plants.Clear();

        foreach (Transform child in transform)
        {
            PlantTimeLoop plant = child.GetComponent<PlantTimeLoop>();

            if (plant == null)
                continue;

            plants.Add(plant);
        }
        isRitualOn = true;
    }

    public void CheckRitualEnd()
    {
        if (!plants.Exists(plant => !plant.IsHarvestable))
        {
            foreach (var plant in plants)
            {
                plant.isActive = false;
            }
            isRitualOn = false;
        }
    }

    private readonly KeyCode[] plantKeys =
    {
        KeyCode.Q,
        KeyCode.U,
        KeyCode.W,
        KeyCode.I,
        KeyCode.E,
        KeyCode.O,
        KeyCode.R,
        KeyCode.P
    };

    private void HandlePlantInput()
    {
        bool spaceHeld = Input.GetKey(KeyCode.Space);

        for (int i = 0; i < plants.Count && i < plantKeys.Length; i++)
        {
            PlantTimeLoop plant = plants[i];
            KeyCode key = plantKeys[i];

            bool keyHeld = Input.GetKey(key);

            // --- Time reversal logic ---
            if (keyHeld && plant.isActive)
            {
                plant.SetReversing(true);
            }
            else
            {
                plant.SetReversing(false);
            }

            // --- Space override logic ---
            if (spaceHeld && keyHeld)
            {
                plant.isActive = false;
            }
            else
            {
                plant.isActive = true;
            }
        }
    }

}