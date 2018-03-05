using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Game_Details_UI : MonoBehaviour {

    public static Player[] players;
    private GameObject mainMenuObject;
    private GameObject startGame;
    private GameObject playerCountObject;
    private GameObject player3;
    private GameObject player4;
    private GameObject inputObject;
    private Button mainMenuButton;
    private Button startButton;
    private Slider playerCountSlider;
    public InputField inputField1;
    public InputField inputField2;
    public InputField inputField3;
    public InputField inputField4;
    public string[] nameList = new string[4];

    // Checks if there is anything entered into the input field.
    void LockInput(InputField input, int playerNum)
    {
        nameList[playerNum] = input.text;
    }

    // Use this for initialization
    void Start () {

        //Wire up main menu button.
        mainMenuObject = GameObject.Find("Canvas/Button_Menu");
        mainMenuButton = mainMenuObject.GetComponent<Button>();
        mainMenuButton.onClick.AddListener(Menu);

        //Setup player slider and options
        playerCountObject = GameObject.Find("Canvas/Battlefield/PlayerCount/Slider");
        playerCountSlider = playerCountObject.GetComponent<Slider>();
        playerCountSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        player3 = GameObject.Find("Canvas/Battlefield/Player3");
        player4 = GameObject.Find("Canvas/Battlefield/Player4");

        //Wire up start button
        startGame = GameObject.Find("Canvas/Button_Play");
        startButton = startGame.GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);

        //Wire up player 1-4 names
        inputObject = GameObject.Find("Canvas/Battlefield/Player1/InputField");
        inputField1 = inputObject.GetComponent<InputField>();
        inputField1.onEndEdit.AddListener(delegate { LockInput(inputField1, 0); });

        inputObject = GameObject.Find("Canvas/Battlefield/Player2/InputField");
        inputField2 = inputObject.GetComponent<InputField>();
        inputField2.onEndEdit.AddListener(delegate { LockInput(inputField2, 1); });

        inputObject = GameObject.Find("Canvas/Battlefield/Player3/InputField");
        inputField3 = inputObject.GetComponent<InputField>();
        inputField3.onEndEdit.AddListener(delegate { LockInput(inputField3, 2); });

        inputObject = GameObject.Find("Canvas/Battlefield/Player4/InputField");
        inputField4 = inputObject.GetComponent<InputField>();
        inputField4.onEndEdit.AddListener(delegate { LockInput(inputField4, 3); });
    }

    void Menu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void StartGame()
    {
        players = new Player[(int)playerCountSlider.value];
        for (int i = 0; i < players.Length; i++)
        {
            string Name;
            if(nameList[i] != null && nameList[i] != "")
            {
                Name = nameList[i];
            }
            else
            {
                int playerNum = i + 1;
                Name = "Player " + playerNum;
            }

            players[i] = new Player();
            players[i].name = Name;
            players[i].order = i;
        }
        SceneManager.LoadScene("GameScene");
    }

    // Invoked when the value of the slider changes.  Sets active value of players 3 and 4.
    public void ValueChangeCheck()
    {
        switch((int)playerCountSlider.value)
        {
            case 2: player3.SetActive(false);
                    player4.SetActive(false);
                    break;
            case 3: player3.SetActive(true);
                    player4.SetActive(false);
                    break;
            case 4: player3.SetActive(true);
                    player4.SetActive(true);
                    break;
        }
    }
}
