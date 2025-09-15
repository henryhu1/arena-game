using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "WeaponEventChannelSO", menuName = "Events/Weapon")]
public class WeaponEventChannelSO : ScriptableObject
{
    public UnityAction<Weapon> OnWeaponEvent;

    public void RaiseEvent(Weapon weapon)
    {
        OnWeaponEvent?.Invoke(weapon);
    }
}
