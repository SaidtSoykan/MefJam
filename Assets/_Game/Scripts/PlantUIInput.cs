using UnityEngine;
using UnityEngine.EventSystems;

public class PlantUIInput : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public PlantTimeLoop plant;
    private RitualInputHandler player;

    private bool isHovering;

    void Start()
    {
        player = FindObjectOfType<RitualInputHandler>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        player.SelectPlantFromUI(plant);

        if (eventData.button == PointerEventData.InputButton.Left)
            plant.SetReversing(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        plant.SetReversing(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        plant.SetReversing(false);
    }

    void Update()
    {
        if (!isHovering || plant == null) return;

        if (Input.GetMouseButton(1))
        {
            float dt = Time.deltaTime;
            float absorbed = plant.AbsorbInstability(
                dt,
                player.playerAbsorbCapacityPerSecond
            );
            player.AddPlayerInstability(absorbed);
        }
    }
}