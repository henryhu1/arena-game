using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject display;

    [Header("Events")]
    [SerializeField] private WeaponEventChannelSO onPlayerWeaponChange;
    [SerializeField] private VoidEventChannelSO onGameOver;

    private void Start()
    {
        display.SetActive(false);
    }

    private void OnEnable()
    {
        onPlayerWeaponChange.OnWeaponEvent += ToggleDisplay;
        onGameOver.OnEventRaised += RemoveDisplay;
    }

    private void OnDisable()
    {
        onPlayerWeaponChange.OnWeaponEvent -= ToggleDisplay;
        onGameOver.OnEventRaised -= RemoveDisplay;
    }

    private void ToggleDisplay(Weapon weapon)
    {
        // not directly using condition as arg in SetActive for readability;
        if (weapon == null || !weapon.GetWeaponData().IsWeaponOfType(AttackType.BOW))
        {
            display.SetActive(false);
        }
        else
        {
            RemoveDisplay();
        }
    }

    private void RemoveDisplay()
    {
        display.SetActive(true);
    }
}
