using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "WeaponEventChannelSO", menuName = "Events/Weapon Event Channel")]
public class WeaponEventChannelSO : ScriptableObject
{
    public UnityAction<WeaponData> OnWeaponEvent;

    public void RaiseEvent(WeaponData weaponData)
    {
        OnWeaponEvent?.Invoke(weaponData);
    }
}
