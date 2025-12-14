using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class RitualInputHandler : MonoBehaviour
{
    [SerializeField] private Slider playerInstabilitySlider;
    [Header("Player Instability")]
    public float playerInstabilityMax = 100f;
    public float playerInstability = 0f;

    [Header("Absorption")]
    public float playerAbsorbCapacityPerSecond = 30f;

    private List<PlantTimeLoop> plants = new List<PlantTimeLoop>();
    public bool isRitualOn { get; set; }
    void Start()
    {
        // ... Var olan kodların burada durabilir ...

        // --- MOUSE AYARLARI ---
        Cursor.lockState = CursorLockMode.None; // İmleci serbest bırak
        Cursor.visible = true; // İmleci görünür yap

        // Her ihtimale karşı zamanın aktığından emin ol
        Time.timeScale = 1f;
    }
    private void Update()
    {
        //HandlePlantInput();
    }

    void FailRitual()
    {
        Debug.Log("RITUAL FAILED: Player instability maxed.");
        playerInstability = 0f;
        foreach (var plant in plants)
        {
            plant.Start();
        }
        // Reset ritual here
    }

    public void AddPlayerInstability(float amount)
    {
        playerInstability += amount;
        playerInstabilitySlider.value = playerInstability / playerInstabilityMax;
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
                plant.particleController.GiveGameEndReward();
                plant._MeshControllerrenderer.QuicklyFadeOut(plant.timelineSlider);
            }
            isRitualOn = false;
        }
    }

    private readonly KeyCode[] plantKeys =
    {
        KeyCode.Q,
        KeyCode.W,
        KeyCode.E,
        KeyCode.R,
        // KeyCode.Q,
        // KeyCode.U,
        // KeyCode.W,
        // KeyCode.I,
        // KeyCode.E,
        // KeyCode.O,
        // KeyCode.R,
        // KeyCode.P
    };

    private void HandlePlantInput()
    {
        bool spaceHeld = Input.GetKey(KeyCode.Space);
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        for (int i = 0; i < plants.Count && i < plantKeys.Length; i++)
        {
            PlantTimeLoop plant = plants[i];
            KeyCode key = plantKeys[i];

            bool keyHeld = Input.GetKey(key);
            bool keyDown = Input.GetKeyDown(key);

            // --- SHIFT + KEY → TOGGLE ACTIVE ---
            if (shiftHeld && keyDown)
            {
                plant.isActive = !plant.isActive;

                // Force clear states when deactivated
                if (!plant.isActive)
                {
                    plant.SetReversing(false);
                    plant.SetAbsorbing(false);
                }

                continue;
            }

            // Ignore inactive plants
            if (!plant.isActive)
            {
                plant.SetReversing(false);
                plant.SetAbsorbing(false);
                continue;
            }

            // --- SPACE + KEY → ABSORB ---
            if (spaceHeld && keyHeld)
            {
                plant.SetAbsorbing(true);
                plant.SetReversing(false);
                continue;
            }

            // --- KEY ONLY → REVERSE ---
            if (keyHeld)
            {
                plant.SetReversing(true);
                plant.SetAbsorbing(false);
                continue;
            }

            // --- IDLE ---
            plant.SetReversing(false);
            plant.SetAbsorbing(false);
        }
    }


}