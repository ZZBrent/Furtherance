using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class settings_pressed : MonoBehaviour {

    public static int musicVolume;
    public static int soundVolume;
    private GameObject options_menu;
    private GameObject settingsObject;
    private GameObject closeObject;
    private GameObject playObject;
    private GameObject exitObject;
    private GameObject musicObject;
    private GameObject soundObject;
    private Button playButton;
    private Button settingsButton;
    private Button closeButton;
    private Button exitButton;
    private Slider musicSlider;
    private Slider soundSlider;

    // Use this for initialization
    void Start()
    {
        settingsObject = GameObject.Find("Canvas/Buttons/Button_Settings");
        settingsButton = settingsObject.GetComponent<Button>();
        settingsButton.onClick.AddListener(OpenSettings);
        closeObject = GameObject.Find("Canvas/Options_Panel/Layout/Close/Button");
        closeButton = closeObject.GetComponent<Button>();
        closeButton.onClick.AddListener(CloseSettings);
        exitObject = GameObject.Find("Canvas/Buttons/Button_Exit");
        exitButton = exitObject.GetComponent<Button>();
        exitButton.onClick.AddListener(CloseApplication);
        options_menu = GameObject.Find("Canvas/Options_Panel");
        options_menu.SetActive(false);
        playObject = GameObject.Find("Canvas/Buttons/Button_Play");
        playButton = playObject.GetComponent<Button>();
        playButton.onClick.AddListener(Play);
    }

    void OpenSettings()
    {
        options_menu.SetActive(true);
        playButton.interactable = false;
        settingsButton.interactable = false;
        exitButton.interactable = false;
        musicObject = GameObject.Find("Canvas/Options_Panel/Layout/Music_Volume/Slider");

    }

    void CloseSettings()
    {
        options_menu.SetActive(false);
        playButton.interactable = true;
        settingsButton.interactable = true;
        exitButton.interactable = true;
    }
    void CloseApplication()
    {
        Application.Quit();
    }
    void Play()
    {
        SceneManager.LoadScene("GameDetails");
    }
}
