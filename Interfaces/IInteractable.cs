using UnityEngine;

public interface IInteractable
{
    void SetInteractor(PlayerInteractHandler playerInteractor);
    void Interact(GameObject interactor);
    bool IsInteractable();
    string GetInteractionText();
    Vector3 GetScreenPos();
}
