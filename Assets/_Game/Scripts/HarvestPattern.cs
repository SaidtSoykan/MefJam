using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HarvestPattern", menuName = "TimeGarden/HarvestPattern")]
public class HarvestPattern : ScriptableObject
{
    public List<HarvestStep> steps = new List<HarvestStep>();
}

[System.Serializable]
public class HarvestStep
{
    public float minPercent; // 0..100
    public float maxPercent; // 0..100
    public bool isActive { get; set; }

    public bool Contains(float percent) => percent >= minPercent && percent <= maxPercent;
}