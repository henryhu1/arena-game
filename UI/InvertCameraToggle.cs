using UnityEngine;
using UnityEngine.UI;

public class InvertCameraToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(ToggleInvertYAxis);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(ToggleInvertYAxis);
    }

    private void ToggleInvertYAxis(bool isInverted)
    {
        MainCamera.Instance.isInverted = isInverted;
    }
}
