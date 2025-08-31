using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "WeaponEventChannelSO", menuName = "Events/Weapon")]
public class WeaponEventChannelSO : ScriptableObject
{
    public UnityAction<WeaponData, GameObject> OnWeaponEvent;

    public void RaiseEvent(WeaponData weaponData, GameObject impactPoint)
    {
        OnWeaponEvent?.Invoke(weaponData, impactPoint);
    }
}
