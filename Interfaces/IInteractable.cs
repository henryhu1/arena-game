using UnityEngine;

public interface IInteractable
{
    void SetInteractor(PlayerInteractHandler playerInteractor);
    void Interact(GameObject interactor);
    bool IsInteractable();
    string GetInteractionTextKey();
    Vector3 GetScreenPos();
}
