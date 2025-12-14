using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] KeyCode interactKey = KeyCode.E;
    [SerializeField] InteractionPromptUI promptUI;

    IInteractable currentInteractable;

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
            promptUI.Show(interactable.GetPromptText());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (currentInteractable == interactable)
            {
                currentInteractable = null;
                promptUI.Hide();
            }
        }
    }
}