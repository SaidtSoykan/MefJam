using TMPro;
using UnityEngine;

public class InteractionPromptUI: MonoBehaviour
{
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] Canvas canvas;

    void Awake()
    {
        canvas.enabled = false;
    }

    public void Show(string text)
    {
        promptText.text = text;
        canvas.enabled = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}