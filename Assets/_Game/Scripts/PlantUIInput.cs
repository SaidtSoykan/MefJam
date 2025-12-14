using UnityEngine;
using UnityEngine.EventSystems;

public class PlantUIInput : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public PlantTimeLoop plant;

    private bool isHovering;
    
    private RitualInputHandler ritualInputHandler;

    public RitualInputHandler RitualInputHandler
    {
        get
        {
            if (ritualInputHandler == null)
            {
                ritualInputHandler = FindObjectOfType<RitualInputHandler>();
            }
            return ritualInputHandler;
        }
        set { ritualInputHandler = value; }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!RitualInputHandler.isRitualOn)
            return;
        if (eventData.button == PointerEventData.InputButton.Left)
            plant.SetReversing(true);
        if (eventData.button == PointerEventData.InputButton.Right)
            plant.SetAbsorbing(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!RitualInputHandler.isRitualOn)
            return;
        if (eventData.button == PointerEventData.InputButton.Left)
            plant.SetReversing(false);
        if (eventData.button == PointerEventData.InputButton.Right)
            plant.SetAbsorbing(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}