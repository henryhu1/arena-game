using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvertCameraToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        toggle.onValueChanged.AddListener((bool isOn) =>
        {
            MainCamera.Instance.isInverted = isOn;
        });
    }
}
