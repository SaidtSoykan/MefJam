using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] string promptText = "Press E to interact";

    public string GetPromptText() => promptText;

    public virtual void Interact(GameObject interactor)
    {
        Debug.Log($"Interacted with {name}");
        // Do your thing here
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}