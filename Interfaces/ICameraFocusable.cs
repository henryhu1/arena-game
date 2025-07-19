using UnityEngine;

public interface ICameraFocusable
{
    bool WantsFocus { get; set; }
    Transform FocusPoint { get; }
}
