using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject display;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO onPlayerWeaponChange;
    [SerializeField] private WeaponEventChannelSO onPlayerWeaponDrop;
    [SerializeField] private VoidEventChannelSO onGameOver;

    private void Start()
    {
        display.SetActive(false);
    }

    private void OnEnable()
    {
        onPlayerWeaponChange.OnWeaponEvent += ToggleDisplay;
        onPlayerWeaponDrop.OnWeaponEvent += RemoveDisplay;
        onGameOver.OnEventRaised += RemoveDisplay;
    }

    private void OnDisable()
    {
        onPlayerWeaponChange.OnWeaponEvent -= ToggleDisplay;
        onPlayerWeaponDrop.OnWeaponEvent -= RemoveDisplay;
        onGameOver.OnEventRaised -= RemoveDisplay;
    }

    private void ToggleDisplay(Weapon weapon)
    {
        if (weapon == null || !weapon.GetWeaponData().IsWeaponOfType(AttackType.BOW))
        {
            RemoveDisplay();
        }
        else
        {
            display.SetActive(true);
        }
    }

    private void RemoveDisplay(Weapon _)
    {
        RemoveDisplay();
    }

    private void RemoveDisplay()
    {
        display.SetActive(false);
    }
}
