using UnityEngine;

public class CameraFocus : MonoBehaviour, ICameraFocusable
{
    private Transform focusPoint;
    private bool wantsFocus;

    private void Awake() { focusPoint = transform; }

    public void SetWantsFocus() { wantsFocus = true; }

    public void StopWantingFocus() { wantsFocus = false; }

    public bool DoesWantFocus() { return wantsFocus; }

    public Vector3 GetFocusPointPosition() { return focusPoint.position; }
}
