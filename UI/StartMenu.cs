using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StartMenuDisplay
{
    MAIN,
    CONTROLS,
    SETTINGS,
    CREDITS,
}
public class StartMenu : MonoBehaviour
{
    [Header("Displays")]
    [SerializeField] private GameObject mainMenuDisplay;
    [SerializeField] private GameObject controlsDisplay;
    [SerializeField] private GameObject settingsDisplay;
    [SerializeField] private GameObject creditsDisplay;
    private StartMenuDisplay currentlyDisplaying;
    private Dictionary<StartMenuDisplay, GameObject> displayMapping = new();

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button[] backButtons;

    private void Awake()
    {
        displayMapping.Add(StartMenuDisplay.MAIN, mainMenuDisplay);
        displayMapping.Add(StartMenuDisplay.CONTROLS, controlsDisplay);
        displayMapping.Add(StartMenuDisplay.SETTINGS, settingsDisplay);
        displayMapping.Add(StartMenuDisplay.CREDITS, creditsDisplay);
    }

    private void Start()
    {
        OnToMainMenu();
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnClickStart);
        controlsButton.onClick.AddListener(OnClickControls);
        settingsButton.onClick.AddListener(OnClickSettings);
        creditsButton.onClick.AddListener(OnClickCredits);
        foreach (Button backButton in backButtons)
        {
            backButton.onClick.AddListener(OnToMainMenu);
        }
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnClickStart);
        controlsButton.onClick.RemoveListener(OnClickControls);
        settingsButton.onClick.RemoveListener(OnClickSettings);
        creditsButton.onClick.RemoveListener(OnClickCredits);
        foreach (Button backButton in backButtons)
        {
            backButton.onClick.RemoveListener(OnToMainMenu);
        }
    }

    private void OnClickStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnClickControls()
    {
        currentlyDisplaying = StartMenuDisplay.CONTROLS;
        UpdateDisplay();
    }

    private void OnClickSettings()
    {
        currentlyDisplaying = StartMenuDisplay.SETTINGS;
        UpdateDisplay();
    }

    private void OnClickCredits()
    {
        currentlyDisplaying = StartMenuDisplay.CREDITS;
        UpdateDisplay();
    }

    private void OnToMainMenu()
    {
        currentlyDisplaying = StartMenuDisplay.MAIN;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        foreach (KeyValuePair<StartMenuDisplay, GameObject> display in displayMapping)
        {
            display.Value.SetActive(display.Key == currentlyDisplaying);
        }
    }
}
