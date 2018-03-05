using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
 * Welcome to the Main Presenter for Furtherance!
 * This file contains all of the main functionality of the game.
 * There is aso much familiar behavior in the game this main presenter contains many functions that are used
 * many times in a wide variety of ways (using enums to determine which way the particular function should be used).
 * As I do with all of my projects, I recommend you take a moment to look at the variables defined at the top of
 * the file before going through the code so you have an understanding of the many variables you will see used
 * throughout the code.
 * 
 * If you have any questions, comments, or suggestions, please email me at brentkeath@gmail.com.
 */

public class MainPresenter : MonoBehaviour {

    #region Get Screen Size
    //When using width and height we want to get current cidth and height (in case the screen size changes)
    public static float width { get
        {
            return Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.width / 10);
        }
    }
    public static float height
    {
        get
        {
            return Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.height / 10);
        }
    }
    #endregion

    #region Commonly Used Variable Equations
    public static Player player
    {
        get
        {
            return Game_Details_UI.players[currentPlayer];
        }
    }
    GameDetails details
    {
        get
        {
            return gameDetails[gameDetails.Count - 1];
        }
    }
    #endregion

    #region Enums
    //Enum for cardtypes.  Used in methods to allow them to used dynamically based on what type of card is being interacted with
    enum cardType {
        unit,
        building,
        item, 
        leader,
        worker,
        research
    }

    //Types of buttons used in methods to allow them to dynamically add listeners to each button
    enum buttonType {
        leader,
        buy,
        research,
        selectWorker,
        upgradeWorker
    }
    #endregion

    #region Other Variables

    //Zoomed and moving are used to keep track of cards that are zoomed and in the process of being zoomed
    public static bool Zoomed;
    public static bool moving;

    //Keep track of current player details and allow it to be accessed by other classes
    [SerializeField]
    GameObject pDetails;
    public static GameObject playerDetails;

    //The window used to show unit details (while units are in play), accessible by other classes (mostly because of Movement.cs)
    [SerializeField]
    GameObject uDetails;
    public static GameObject unitDetails;

    //Keep track of the notification window to set active and inactive
    [SerializeField]
    GameObject nWindow;
    static GameObject notificationWindow;

    //Keep track of the notification buttons to set them to active and inactive
    [SerializeField]
    GameObject nButtons;
    static GameObject notificationButtons;

    //Keep track of the card choice menu to set active and inactive
    [SerializeField]
    GameObject cChoice;
    public static GameObject cardChoice;
    [SerializeField]
    GameObject cChoiceTitle;
    public static GameObject cardChoiceTitle;
    [SerializeField]
    GameObject cChoiceWindow;
    public static GameObject cardChoiceWindow;

    //Keep track of the end button so that we can set it to active and inactive
    [SerializeField]
    GameObject endButton;

    //The current players cards to show on their tableau
    List<GameObject> playerCards = new List<GameObject>();

    //The list of game details tracked to allow users to undo their actions
    List<GameDetails> gameDetails = new List<GameDetails>();

    //The player who is currently in control
    static public int currentPlayer;

    //The list of leaders available to choose from
    List<int> leaderList;

    //List of buttons to be hidden during combat and shown during the main phase
    List<GameObject> buttons = new List<GameObject>();

    //List of booleans used for tracking buttons clicked when user is researching (so the third action is atuo-played)
    bool topDecked = false;
    bool bottomDecked = false;
    bool researched = false;

    //Cache all important GameObjects to avoid using the Find method (which is extremely inefficient)
    [SerializeField]
    GameObject canvasObject;
    public static GameObject canvas;
    [SerializeField]
    GameObject leaderObject;
    [SerializeField]
    GameObject playerDetailsLeaderObject;
    [SerializeField]
    GameObject playerDetailsWorkerObject;
    [SerializeField]
    GameObject playerDetailsBuildingsObject;
    [SerializeField]
    GameObject playerDetailsItemsObject;
    [SerializeField]
    GameObject playerDetailsResearchObject;
    [SerializeField]
    GameObject unitObject;
    [SerializeField]
    GameObject itemObject;
    [SerializeField]
    GameObject bObject;
    public static GameObject buildingObject;
    [SerializeField]
    GameObject workerObject;
    [SerializeField]
    GameObject mineButton;
    [SerializeField]
    GameObject turnText;
    [SerializeField]
    GameObject gText;
    public static GameObject goldText;
    [SerializeField]
    GameObject hpText;
    [SerializeField]
    GameObject vpText;
    [SerializeField]
    GameObject boardObject;
    [SerializeField]
    GameObject hSpace;
    public static GameObject highlightedSpace;
    [SerializeField]
    GameObject activateButton;
    [SerializeField]
    GameObject buyButton;
    [SerializeField]
    GameObject leaderButton;
    [SerializeField]
    GameObject researchButton;
    [SerializeField]
    GameObject topdeckButton;
    [SerializeField]
    GameObject bottomButton;
    [SerializeField]
    GameObject workerButton;
    [SerializeField]
    GameObject upgradeButton;
    [SerializeField]
    GameObject dButton;
    public static GameObject defendButton;
    [SerializeField]
    GameObject sButton;
    public static GameObject selectButton;
    [SerializeField]
    GameObject aButton;
    public static GameObject attackButton;
    [SerializeField]
    GameObject nTitle;
    static GameObject notificationTitle;
    [SerializeField]
    GameObject nBody;
    static GameObject notificationBody;
    [SerializeField]
    GameObject nb1;
    static GameObject notificationButton1;
    [SerializeField]
    GameObject nb2;
    static GameObject notificationButton2;
    [SerializeField]
    GameObject nb1t;
    static GameObject notificationButton1Text;
    [SerializeField]
    GameObject nb2t;
    static GameObject notificationButton2Text;
    [SerializeField]
    GameObject unitMovementObject;
    [SerializeField]
    GameObject duplicates;

    #endregion

    #region Card Specific Variables
    //Hourglass
    bool hourglassUsed = false;
    bool secondTurn = false;

    //Sharpener
    List<Unit> sharpened = null;
    List<Unit> invisible = null;
    #endregion

    // Use this for initialization
    void Start () {

        #region Grab and Hide Important Objects
        //Grab and hide player details tableau for later use
        canvas = canvasObject;

        goldText = gText;
        buildingObject = bObject;
        selectButton = sButton;
        defendButton = dButton;
        defendButton.SetActive(false);
        attackButton = aButton;
        attackButton.SetActive(false);
        playerDetails = pDetails;
        playerDetails.SetActive(false);
        unitDetails = uDetails;
        unitDetails.SetActive(false);
        notificationButtons = nButtons;
        notificationButtons.SetActive(false);
        notificationWindow = nWindow;
        notificationWindow.SetActive(false);
        cardChoice = cChoice;
        cardChoiceTitle = cChoiceTitle;
        cardChoiceWindow = cChoiceWindow;
        cardChoice.SetActive(false);
        endButton.SetActive(false);
        notificationTitle = nTitle;
        notificationBody = nBody;
        notificationButton1 = nb1;
        notificationButton2 = nb2;
        notificationButton1Text = nb1t;
        notificationButton2Text = nb2t;
        #endregion

        #region Assign Players (for testing purposes only)
        //Check if players have been assigned names.  If not, auto-assign them player 1, 2, 3, and 4.  If so, skip to leader selection.
        //This is strictly for testing purposes, all users will go through the main page and have players made before entering the game.
        if (Game_Details_UI.players == null)
        {
            string name;
            Game_Details_UI.players = new Player[4];
            for (int i = 0; i < Game_Details_UI.players.Length; i++)
            {
                int playerNum = i + 1;
                name = "Player " + playerNum;
                Player player = new Player();
                player.name = name;
                player.order = playerNum;
                Game_Details_UI.players[i] = player;
            }
        }
        #endregion

        currentPlayer = Game_Details_UI.players.Length - 1;

        leaderSelection();
    }

    #region Card Choices

        #region Leader Methods
        //Use this method when a player selects their leader
        private void LeaderChosen(GameObject leader)
        {
            moving = false;
            Zoomed = false;

            //Destroy remaining leaders
            foreach (GameObject go in FlipCard.cards)
            {
                Destroy(go);
            }

            var ds = new DataService("Furtherance.db");
            List<Leader> playerLeader = ds.GetLeader(leader.name).ToList();

            player.leader = playerLeader[0];

            //Check for Gaita.  If so, add 10 coins
            if (player.leader.Name == "Gaita")
            {
                changeGold(12, player);
            }

            ////Check for Trosh.  If so, let them upgrade their worker.
            //if (player.leader.Name == "Trosh")
            //{
            //    //Check if worker has already been upgraded
            //    if(player.workers[0] == null)
            //    {
            //        Vector3[] positions = new Vector3[5];
            //        FlipCard.cards = new GameObject[5];
            //        for (int i = 0; i <= 5; i++)
            //        {
            //            GameObject worker = new GameObject();
            //            if ((i + 2) % 2 == 0)
            //            {
            //                positions[i] = new Vector3(width / 1.6f + (i * width / 16), height / 3.9f, 0);
            //                leader = GameObject.Instantiate(GameObject.Find("Canvas/Leader"), positions[i], Quaternion.identity, canvas.transform);
            //            }
            //            else
            //            {
            //                positions[i] = new Vector3(width / 1.6f + (i * width / 16), height / 1.58f, 0);
            //                leader = GameObject.Instantiate(GameObject.Find("Canvas/Leader"), positions[i], Quaternion.identity, canvas.transform);
            //            }
            //            FlipCard.cards[i] = worker;
            //        }
            //        int cardIndex = 0;
            //        foreach (GameObject card in FlipCard.cards)
            //        {
            //            Card_Model cardModel = card.GetComponent<Card_Model>();
            //            CardFlipper flipper = card.GetComponent<CardFlipper>();
            //            flipper.FlipCard(cardModel.cardBack, cardModel.faces[leaderList[cardIndex]], leaderList[cardIndex]);
            //            card.name = cardModel.faces[leaderList[cardIndex]].name;
            //            cardIndex++;
            //        }
            //        StartCoroutine(LoadButtons(positions, buttonType.leader, 0, null, null));
            //        return;
            //    }
            //}

            //Assign worker to player
            List<Worker> playerWorker = ds.GetWorker("Worker").ToList();

            player.workers[0] = playerWorker[0];

            leaderList.Remove(playerLeader[0].Id);
            currentPlayer--;
            if (currentPlayer >= 0)
            {
                leaderSelection();
            }
            else
            {
                //STart the game and keep track of all of the details at each point in the game
                GameDetails details = new GameDetails();

                //Destroy remaining leaders
                foreach (GameObject go in FlipCard.cards)
                {
                    Destroy(go);
                }

                //Refresh leader list
                leaderList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                //Show player details
                playerDetails.SetActive(true);

                //Setup the worker, units, items, and buildings decks
                currentPlayer = 0;

                #region Initialize Workers
                //Set values for unit array to pass back a random list of the numbers for each unit
                IEnumerable<int> intList = new int[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5};
                details.workersDeck = intList.Shuffle().ToList();
                #endregion

                #region Initialize Units
                FlipCard.cards = new GameObject[10];
                //Set values for unit array to pass back a random list of the numbers for each unit
                intList = new int[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 11, 11, 12, 12, 13, 13, 13, 13 };
                //details.unitsDeck = intList.Shuffle().ToList();
                details.unitsDeck = intList.ToList();

                //Initialize deck positions for each deck (units, buildings, and items)
                Vector3[] unitDeckPosition = { new Vector3(width / 18.5f, height / 12.4f, 0) };

                //Create sprites for units
                Vector3[] unitPositions = { new Vector3(width / 6.49f, height / 12.4f, 0), new Vector3(width / 3.94f, height / 12.4f, 0), new Vector3(width / 2.82f, height / 12.4f, 0) };
                int typeIndex = 0;
                int totalIndex = 0;
                GameObject unitDeck = GameObject.Instantiate(unitObject, unitDeckPosition[0], Quaternion.identity, canvas.transform);
                Vector3 location = unitPositions[0];
                initializeCard(typeIndex, location, cardType.unit, details.unitsDeck, totalIndex);
                typeIndex++;
                totalIndex++;

                //Check whether the next unit matches a unit in the list
                typeIndex = cardDuplicateCheck(unitPositions[1], typeIndex, totalIndex, details.unitsDeck, cardType.unit, true);
                totalIndex++;

                //Check whether the next unit matches a unit in the list
                typeIndex = cardDuplicateCheck(unitPositions[2], typeIndex, totalIndex, details.unitsDeck, cardType.unit, true);
                totalIndex++;

                //Create buy buttons
                StartCoroutine(LoadButtons(unitPositions, buttonType.buy, 0, cardType.unit, null, true));

                //Add units to details
                for (int i = 0; i < typeIndex; i++)
                {
                    details.unitsRevealed.Add(details.unitsDeck[0]);
                    details.unitsDeck.Remove(details.unitsDeck[0]);
                }

                //Create research button
                GameObject[] unitDeckArray = { unitDeck };
                StartCoroutine(LoadButtons(unitDeckPosition, buttonType.research, 0, cardType.unit, unitDeckArray, true));
                #endregion

                #region Initialize Buildings
                Vector3[] buildingDeckPosition = { new Vector3(width / 1.83f, height / 1.326f, 0) };
                //Set values for unit array to pass back a random list of the numbers for each unit
                intList = new int[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14 };
                //details.buildingsDeck = intList.Shuffle().ToList();
                details.buildingsDeck = intList.ToList();

                //Create sprites for buildings
                Vector3[] buildingPositions = { new Vector3(width / 1.5f, height / 1.326f, 0), new Vector3(width / 1.276f, height / 1.326f, 0), new Vector3(width / 1.11f, height / 1.326f, 0) };
                GameObject buildingDeck = GameObject.Instantiate(buildingObject, buildingDeckPosition[0], Quaternion.identity, canvas.transform);
                typeIndex = 0;
                location = buildingPositions[0];
                initializeCard(typeIndex, location, cardType.building, details.buildingsDeck, totalIndex);
                typeIndex++;
                totalIndex++;

                //Check whether the next building matches a building in the list
                typeIndex = cardDuplicateCheck(buildingPositions[1], typeIndex, totalIndex, details.buildingsDeck, cardType.building, true);
                totalIndex++;

                //Check whether the next building matches a building in the list
                typeIndex = cardDuplicateCheck(buildingPositions[2], typeIndex, totalIndex, details.buildingsDeck, cardType.building, true);
                totalIndex++;

                //Refresh list of buttons to show and hide buttons between buys and combat
                buttons = new List<GameObject>();

                //Create buy buttons
                StartCoroutine(LoadButtons(buildingPositions, buttonType.buy, 3, cardType.building, null, true));

                //Add buildings to details
                for (int i = 0; i < typeIndex; i++)
                {
                    details.buildingsRevealed.Add(details.buildingsDeck[0]);
                    details.buildingsDeck.Remove(details.buildingsDeck[0]);
                }

                //Create research button
                GameObject[] buildingDeckArray = { buildingDeck };
                StartCoroutine(LoadButtons(buildingDeckPosition, buttonType.research, 0, cardType.building, buildingDeckArray, true));
                #endregion

                #region Initialize Items
                //Set values for unit array to pass back a random list of the numbers for each unit
                Vector3[] itemDeckPosition = { new Vector3(width / 1.83f, height / 2f, 0) };
                intList = new int[] { 0, 0, 1, 1, 2, 3, 3, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12 };
                //details.itemsDeck = intList.Shuffle().ToList();
                details.itemsDeck = intList.ToList();

                //Gets sprites for items
                Vector3[] itemPositions = getItemPositions();
                GameObject itemDeck = GameObject.Instantiate(itemObject, itemDeckPosition[0], Quaternion.identity, canvas.transform);
                typeIndex = 0;
                location = itemPositions[0];
                initializeCard(typeIndex, location, cardType.item, details.itemsDeck, totalIndex);
                typeIndex++;
                totalIndex++;

                //Check whether the next item matches an item in the list
                typeIndex = cardDuplicateCheck(itemPositions[1], typeIndex, totalIndex, details.itemsDeck, cardType.item, true);
                totalIndex++;

                //Check whether the next item matches an item in the list
                typeIndex = cardDuplicateCheck(itemPositions[2], typeIndex, totalIndex, details.itemsDeck, cardType.item, true);
                totalIndex++;

                //Create buy buttons
                StartCoroutine(LoadButtons(itemPositions, buttonType.buy, 6, cardType.item, null, true));

                //Add items to details
                for (int i = 0; i < typeIndex; i++)
                {
                    details.itemsRevealed.Add(details.itemsDeck[0]);
                    details.itemsDeck.Remove(details.itemsDeck[0]);
                }

                //Create research buttons
                GameObject[] itemDeckArray = { itemDeck };
                StartCoroutine(LoadButtons(itemDeckPosition, buttonType.research, 0, cardType.item, itemDeckArray, true));

                //Add listener to mine button
                Button button = mineButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { Mine(); });
                buttons.Add(mineButton);
                #endregion

                //Save details
                details.players = Game_Details_UI.players;
                gameDetails.Add(details);

                nextTurn();
            }
        }
    
        void leaderSelection()
    {
        Zoomed = false;
        moving = true;
        int playersRemaining = currentPlayer + 1;
        #region Player Name
        turnText.GetComponent<Text>().text = player.name + "'s Turn";
        #endregion

        #region Show Leaders
        if (playersRemaining == Game_Details_UI.players.Length)
        {
            //Set values for leader array to pass back a random list of the numbers 0-9
            IEnumerable<int> intList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            leaderList = intList.Shuffle().ToList();
        }

        //Get player number plus one for number of leaders
        Vector3[] positions = new Vector3[playersRemaining + 1];

        switch (playersRemaining)
        {
            case 1:
                FlipCard.cards = new GameObject[2];
                for (int i = 0; i <= playersRemaining; i++)
                {
                    positions[i] = new Vector3(width / 1.6f + ((i + 1) * width / 8), height / 1.8f, 0);
                    GameObject leader = GameObject.Instantiate(leaderObject, positions[i], Quaternion.identity, canvas.transform);
                    FlipCard.cards[i] = leader;
                }
                break;
            case 2:
                FlipCard.cards = new GameObject[3];
                for (int i = 0; i <= playersRemaining; i++)
                {
                    positions[i] = new Vector3(width / 1.6f + (i * width / 8), height / 1.8f, 0);
                    GameObject leader = GameObject.Instantiate(leaderObject, positions[i], Quaternion.identity, canvas.transform);
                    FlipCard.cards[i] = leader;
                }
                break;
            case 3:
                FlipCard.cards = new GameObject[4];
                for (int i = 0; i <= playersRemaining; i++)
                {
                    GameObject leader = new GameObject();
                    if ((i + 2) % 2 == 0)
                    {
                        positions[i] = new Vector3(width / 1.6f + ((i + 1) * width / 16), height / 4.4f, 0);
                        leader = GameObject.Instantiate(leaderObject, positions[i], Quaternion.identity, canvas.transform);
                    }
                    else
                    {
                        positions[i] = new Vector3(width / 1.6f + (i * width / 16), height / 1.58f, 0);
                        leader = GameObject.Instantiate(leaderObject, positions[i], Quaternion.identity, canvas.transform);
                    }
                    FlipCard.cards[i] = leader;
                }
                break;
            case 4:
                FlipCard.cards = new GameObject[5];
                for (int i = 0; i <= playersRemaining; i++)
                {
                    GameObject leader = new GameObject();
                    if ((i + 2) % 2 == 0)
                    {
                        positions[i] = new Vector3(width / 1.6f + (i * width / 16), height / 3.9f, 0);
                        leader = GameObject.Instantiate(leaderObject, positions[i], Quaternion.identity, canvas.transform);
                    }
                    else
                    {
                        positions[i] = new Vector3(width / 1.6f + (i * width / 16), height / 1.58f, 0);
                        leader = GameObject.Instantiate(leaderObject, positions[i], Quaternion.identity, canvas.transform);
                    }
                    FlipCard.cards[i] = leader;
                }
                break;
        }
        int cardIndex = 0;
        foreach (GameObject card in FlipCard.cards)
        {
            Card_Model cardModel = card.GetComponent<Card_Model>();
            CardFlipper flipper = card.GetComponent<CardFlipper>();
            flipper.FlipCard(cardModel.cardBack, cardModel.faces[leaderList[cardIndex]], leaderList[cardIndex]);
            card.name = cardModel.faces[leaderList[cardIndex]].name;
            cardIndex++;
        }
        StartCoroutine(LoadButtons(positions, buttonType.leader, 0, null, null, true));
        #endregion
    }
        #endregion

    private void WorkerChosen(GameObject worker)
    {
        var ds = new DataService("Furtherance.db");
        List<Worker> playerWorker = ds.GetWorker(worker.name).ToList();

        //Go through each player worker and find the first one that has no counters
        for(int i = 0; i < player.workers.Length; i++)
        {
            if(player.workers[i].Id == 0  && player.workers[i].IsActive)
            {
                player.workers[i] = playerWorker[0];
                break;
            }
        }
    }
    #endregion

    #region Turn Methods

    private void nextTurn()
    {
        //Check that current player has not been defeated
        if (player.defeated)
            startNextTurn();

        //Check for abilities that go away when a new turn starts
        if(sharpened != null)
        {
            foreach(Unit u in sharpened)
            {
                u.Atk -= 1;
            }
            sharpened = null;
        }

        //Check for buildings that activate at the start of the turn
        foreach (Building b in player.buildings.Where(s => s != null && s.Name == "Treasury"))
        {
            changeGold(1, player);
        }

        //Refresh building abilities
        foreach (Building b in player.buildings.Where(s => s != null && s.TurnAbilityCount != null))
        {
            if (b.Name == "Guard Tower")
                b.TurnAbilityCount = 2;
        }

        foreach (Unit u in player.units.Where(s => s != null && s.justPlayed == true))
        {
            u.justPlayed = false;
        }

        bool workerAvailable = false;
        foreach (Worker w in player.workers.Where(s => s != null))
        {
            if (w.TurnsLeft > 0)
            {
                if (w.TurnsLeft == 1 && w.Building != null)
                {
                    switch(w.Building.Name)
                    {
                        case "Library":
                        case "Church":
                            changeVP(1, player);
                            break;
                        case "Grand Park":
                        case "City Hall":
                            changeVP(2, player);
                            break;
                        case "Walls":
                            w.Building.Time = 5;
                            break;
                        case "Fortification":
                            w.Building.Time = player.buildings.Where(s => s != null).Count();
                            break;
                    }
                    player.buildings[player.buildings.Count(s => s != null)] = w.Building;
                    foreach(Building b in player.buildings.Where(s => s != null && s.Name == "Fortification"))
                    {
                        b.Time = player.buildings.Where(s => s != null).Count();
                    }
                    foreach (Building b in player.buildings.Where(s => s != null && s.Name == "Guard Tower"))
                    {
                        b.TurnAbilityCount = 2;
                    }
                    w.Building = null;
                }
                w.TurnsLeft--;
                if(w.TurnsLeft == 0 && w.Name == "Miner")
                {
                    player.gold += 6;
                }
            }
            if (w.TurnsLeft == 0 && !workerAvailable)
            {
                workerAvailable = true;
                w.IsActive = true;
            }
        }

        moving = false;

        if (gameDetails.Count > 0)
        {
            //Delete all previous player cards
            destroyAll(playerCards);
            saveDetails(false);
        }

        //Change banner text
        turnText.GetComponent<Text>().text = player.name + "'s Turn";

        //Keep track of rounds for Gaita and buildings
        //if (currentPlayer == 0)
        //{
        //    roundCount++;
        //}

        //Show action buttons
        setButtons(true);

        //Show gold, hp, and vp
        goldText.GetComponent<Text>().text = String.Format("{0}", player.gold);
        hpText.GetComponent<Text>().text = String.Format("{0}", player.hp);
        vpText.GetComponent<Text>().text = String.Format("{0}", player.vp);

        //Clear playerCards
        playerCards = new List<GameObject>();

        //Show leader
        GameObject leader = GameObject.Instantiate(leaderObject, new Vector3(width / 1.75f, height / 5.586f, 0), Quaternion.identity, playerDetailsLeaderObject.transform);
        leader.GetComponent<Image>().sprite = leader.GetComponent<Card_Model>().faces[leaderList[player.leader.Id]];
        leader.name = leader.GetComponent<Card_Model>().faces[leaderList[player.leader.Id]].name;
        playerCards.Add(leader);

        //Show workers
        ShowPlayerCards(cardType.worker, player.workers, player);

        //Show items
        ShowPlayerCards(cardType.item, player.items, player);

        //Show buildings
        ShowPlayerCards(cardType.building, player.buildings, player);

        //Show research
        ShowPlayerCards(cardType.research, player.research, player);

        addButtonsToItems();

        addButtonsToBuildings();

        boardObject.transform.rotation = Quaternion.AngleAxis(90 * currentPlayer, Vector3.forward);

        //If the player worker is not available
        if (!workerAvailable)
            startCombatPhase(true);
    }

    private void addButtonsToBuildings()
    {
        bool firstButtonAdded = false;
        foreach (Building b in player.buildings.Where(s => s != null))
        {
            GameObject obj;
            if (playerCards.Count(s => s != null && s.name == b.Name) == 1)
            {
                obj = playerCards.Where(s => s != null && s.name == b.Name).First();
            }
            else if (!firstButtonAdded)
            {
                firstButtonAdded = true;
                obj = playerCards.Where(s => s != null && s.name == b.Name).First();
            }
            else
            {
                obj = playerCards.Where(s => s != null && s.name == b.Name).Last();
            }
            switch (b.Name)
            {
                case "Catapult":
                    List<Movement.coordinates> unitsToHit = new List<Movement.coordinates>();
                    for(int i = 1; i < Game_Details_UI.players.Count(); i++)
                    {
                        foreach(Unit u in Game_Details_UI.players[(currentPlayer + i) % Game_Details_UI.players.Count()].units.Where(s => s != null))
                        {
                            Movement.coordinates unitCo;
                            unitCo.x = (int)u.XLocation;
                            unitCo.y = (int)u.YLocation;
                            Movement.coordinates convertedCo = Movement.convertCoordinates(unitCo, currentPlayer);
                            if(convertedCo.y == -2)
                            {
                                unitsToHit.Add(unitCo);
                            }
                        }
                    }
                    if(unitsToHit.Count > 0)
                    {
                        GameObject buttonObject = Instantiate(activateButton, obj.transform.position - new Vector3(0, height/21f, 0), Quaternion.identity, obj.transform);
                        buttonObject.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
                        buttonObject.GetComponent<Button>().onClick.AddListener(delegate { activateCatapult(buttonObject, 0, unitsToHit); });
                    }
                    break;
                case "Medical Center":
                    List<Movement.coordinates> unitsToHeal = new List<Movement.coordinates>();
                    foreach (Unit u in player.units.Where(s => s != null))
                    {
                        Movement.coordinates unitCo;
                        unitCo.x = (int)u.XLocation;
                        unitCo.y = (int)u.YLocation;
                        Movement.coordinates convertedCo = Movement.convertCoordinates(unitCo, currentPlayer);
                        if (convertedCo.y == -3 && u.Damage > 0)
                        {
                            unitsToHeal.Add(unitCo);
                        }
                    }
                    if (unitsToHeal.Count > 0)
                    {
                        GameObject buttonObject = Instantiate(activateButton, obj.transform.position - new Vector3(0, height / 21f, 0), Quaternion.identity, obj.transform);
                        buttonObject.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
                        buttonObject.GetComponent<Button>().onClick.AddListener(delegate { activateMedicalCenter(buttonObject, unitsToHeal); });
                    }
                    break;
            }
        }
    }

    private void activateCatapult(GameObject button, int timesUsed, List<Movement.coordinates> coList)
    {
        setButtons(false);
        button.SetActive(false);
        timesUsed++;
        foreach(Movement.coordinates uc in coList)
        {
            Movement.coordinates c = Movement.convertCoordinates(uc, currentPlayer);
            GameObject selection = GameObject.Instantiate(highlightedSpace, new Vector3(width / 4.1925f + c.x * (width / 15.68f), height / 1.7348f + c.y * (height / 8.8138f), 0), Quaternion.identity, canvas.transform);
            Button attackButton = selection.GetComponent<Button>();
            Movement.selections.Add(selection);
            selection.GetComponent<Image>().color = new Color(1, 0, 0, 0.49f);
            if (timesUsed == 2)
                attackButton.onClick.AddListener(delegate { hitUnitWithCatapult(uc, null); });
            else
                attackButton.onClick.AddListener(delegate { hitUnitWithCatapult(uc, button); });
        }
        if(timesUsed == 2)
        {
            GameObject.Destroy(button);
        }
        else
        {
            coList = new List<Movement.coordinates>();
            for (int i = 1; i < Game_Details_UI.players.Count(); i++)
            {
                foreach (Unit u in Game_Details_UI.players[(currentPlayer + i) % Game_Details_UI.players.Count()].units.Where(s => s != null))
                {
                    Movement.coordinates unitCo;
                    unitCo.x = (int)u.XLocation;
                    unitCo.y = (int)u.YLocation;
                    Movement.coordinates convertedCo = Movement.convertCoordinates(unitCo, currentPlayer);
                    if (unitCo.y == -2)
                    {
                        coList.Add(unitCo);
                    }
                }
            }
            button.GetComponent<Button>().onClick.AddListener(delegate { activateCatapult(button, timesUsed, coList); });
        }
    }

    private void hitUnitWithCatapult(Movement.coordinates c, GameObject button)
    {
        foreach (Player p in Game_Details_UI.players)
        {
            foreach (Unit u in p.units.Where(s => s != null))
            {
                if (c.x == u.XLocation && c.y == u.YLocation)
                {
                    u.Damage += 1;
                    Movement.attackFinished = false;
                    Movement.finishAttack(u, p);
                    Movement.destroySpots();
                    if(!details.combatPhase)
                        setButtons(true);
                    if (button != null)
                        button.SetActive(true);
                    return;
                }
            }
        }
    }

    private void activateMedicalCenter(GameObject button, List<Movement.coordinates> coList)
    {
        setButtons(false);
        foreach (Movement.coordinates uc in coList)
        {
            Movement.coordinates c = Movement.convertCoordinates(uc, currentPlayer);
            GameObject selection = GameObject.Instantiate(highlightedSpace, new Vector3(width / 4.1925f + c.x * (width / 15.68f), height / 1.7348f + c.y * (height / 8.8138f), 0), Quaternion.identity, canvas.transform);
            Button attackButton = selection.GetComponent<Button>();
            Movement.selections.Add(selection);
            selection.GetComponent<Image>().color = new Color(0, 0.8f, 0.2f, 0.49f);
            attackButton.onClick.AddListener(delegate { healUnitWithMedicalCenter(uc); });
        }
        Destroy(button);
    }

    private void healUnitWithMedicalCenter(Movement.coordinates c)
    {
         foreach (Unit u in player.units.Where(s => s != null))
        {
            if (c.x == u.XLocation && c.y == u.YLocation)
            {
                u.Damage -= 1;
                Movement.destroySpots();
                return;
            }
        }
    }

    private void addButtonsToItems()
    {
        bool firstButtonAdded = false;
        List<UnityAction> listeners = new List<UnityAction>();
        List<Button> buttons = new List<Button>();
        foreach(Item i in player.items.Where(s => s != null))
        {
            GameObject obj;
            if (playerCards.Count(s => s != null && s.name == i.Name) == 1)
            {
                obj = playerCards.Where(s => s != null && s.name == i.Name).First();
            }
            else if(!firstButtonAdded)
            {
                firstButtonAdded = true;
                obj = playerCards.Where(s => s != null && s.name == i.Name).First();
            }
            else
            {
                obj = playerCards.Where(s => s != null && s.name == i.Name).Last();
            }
            Vector3 buttonPosition = obj.transform.position;
            buttonPosition.y -= height / 21f;
            GameObject buttonObj = GameObject.Instantiate(activateButton, buttonPosition, Quaternion.identity, obj.transform);
            Button button = buttonObj.GetComponent<Button>();
            buttonObj.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            buttons.Add(button);
            switch (i.Name)
            {
                case "Backup-Plan":
                    listeners.Add(delegate { BackupPlan(obj, i.Id); } );
                    break;
                case "Excalibur":
                    listeners.Add(delegate { Excalibur(obj, i.Id); });
                    break;
                case "Hammer":
                    listeners.Add(delegate { Hammer(obj, i.Id); });
                    break;
                case "Holy-Grail":
                    listeners.Add(delegate { HolyGrail(obj, i.Id); });
                    break;
                case "Hourglass":
                    listeners.Add(delegate { Hourglass(obj, i.Id); });
                    break;
                case "Invincibility-Cloak":
                    listeners.Add(delegate { InvincibilityCloak(obj, i.Id); });
                    break;
                case "Off-Hand-Knife":
                    listeners.Add(delegate { OffHandKnife(obj, i.Id); });
                    break;
                case "Potion":
                    listeners.Add(delegate { Potion(obj, i.Id); });
                    break;
                case "Report":
                    listeners.Add(delegate { Report(obj, i.Id); });
                    break;
                case "Ring-Of-Invisibility":
                    listeners.Add(delegate { RingOfInvisibility(obj, i.Id); });
                    break;
                case "Sharpener":
                    listeners.Add(delegate { Sharpener(obj, i.Id); });
                    break;
                case "Stolen-Research":
                    listeners.Add(delegate { StolenResearch(obj, i.Id); });
                    break;
            }
        }
        addListeners(buttons, listeners);
    }

    #region Item Methods
    //Below are all of the methods for the items.
    //More methods are added as items are added to the game.
    //Most items have requirements and if those requirements are not met an error is thrown to the player
    void useItem(GameObject cardObject, int itemId, bool pause)
    {
        if (Card_Zoom.zoomedCard != null)
        {
            unzoomCard(Card_Zoom.zoomedCard);
        }
        Destroy(cardObject);
        bool itemDeleted = false;
        for (int m = 0; m < player.items.Where(s => s != null).Count(); m++)
        {
            if (itemDeleted)
            {
                if (m < player.items.Where(s => s != null).Count() - 1)
                    player.items[m] = player.items[m + 1];
                else
                    player.items[m] = null;
            }
            else if (player.items[m].Id == itemId)
            {
                if (m < player.items.Where(s => s != null).Count() - 1)
                    player.items[m] = player.items[m + 1];
                else
                    player.items[m] = null;
                itemDeleted = true;
            }
        }
        if(pause)
        {
            if (details.combatPhase)
            {
                Movement.paused = true;
            }
            else
            {
                setButtons(false);
            }
        }
    }
    void BackupPlan(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
        List<int> intList = new List<int>();
        List<GameObject> cards = new List<GameObject>();
        GameObject obj = new GameObject();
        intList = details.buildingsRevealed.Select(s => s).Distinct().ToList();
        intList.AddRange(details.itemsRevealed.Select(s => s).Distinct().ToList());
        intList.AddRange(details.unitsRevealed.Select(s => s).Distinct().ToList());
        for (int i = 0; i < 9; i++)
        {
            switch (i)
            {
                case 0:
                case 1:
                case 2:
                    obj = buildingObject;
                    break;
                case 3:
                case 4:
                case 5:
                    obj = itemObject;
                    break;
                case 6:
                case 7:
                case 8:
                    obj = unitObject;
                    break;
            }
            GameObject card = GameObject.Instantiate(obj, new Vector3(), Quaternion.identity, cardChoice.transform);
            cards.Add(card);
            card.GetComponent<Image>().sprite = obj.GetComponent<Card_Model>().faces[intList[i]];
        }
        List<Vector3> positions = showCards("Backup Plan", cards);
        int n = 0;
        List<int> buildingsSelected = new List<int>();
        List<int> itemsSelected = new List<int>();
        List<int> unitsSelected = new List<int>();
        foreach (GameObject card in cards)
        {
            card.transform.position = positions[n];
            GameObject button = GameObject.Instantiate(selectButton, card.transform.position - new Vector3(0, (110f / 760f) * height), Quaternion.identity, card.transform);
            button.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            int m = intList[n];
            //Have to assign type to use it in the selectBackUpCard method
            cardType type = cardType.building;
            switch(n)
            {
                case 0:
                case 1:
                case 2:
                    type = cardType.building;
                    break;
                case 3:
                case 4:
                case 5:
                    type = cardType.item;
                    break;
                case 6:
                case 7:
                case 8:
                    type = cardType.unit;
                    break;
            }
            button.GetComponent<Button>().onClick.AddListener(delegate { selectBackupCard(m, type, buildingsSelected, itemsSelected, unitsSelected, card, cards); });
            n++;
        }
    }
    void selectBackupCard(int cardId, cardType type, List<int> buildingsSelected, List<int> itemsSelected, List<int> unitsSelected, GameObject card, List<GameObject> cards)
    {
        switch(type)
        {
            case cardType.building:
                buildingsSelected.Add(cardId);
                break;
            case cardType.item:
                itemsSelected.Add(cardId);
                break;
            case cardType.unit:
                unitsSelected.Add(cardId);
                break;
        }
        cards.Remove(card);
        Destroy(card);
        if(buildingsSelected.Count + itemsSelected.Count + unitsSelected.Count >= 3)
        {
            for(int i = 0; i < buildingsSelected.Count; i++)
            {
                List<int> revealed = details.buildingsRevealed;
                List<int> deck = details.buildingsDeck;
                GameObject obj = GameObject.Find("Canvas/" + buildingObject.GetComponent<Card_Model>().faces[cardId].name);
                for (int n = 0; n < revealed.Where(s => s == cardId).Count(); n++)
                    revealed.Remove(cardId);
                
                showNextCard(obj, revealed, deck, cardType.building, false);
            }
            for (int i = 0; i < itemsSelected.Count; i++)
            {
                List<int> revealed = details.itemsRevealed;
                List<int> deck = details.itemsDeck;
                GameObject obj = GameObject.Find("Canvas/" + itemObject.GetComponent<Card_Model>().faces[cardId].name);
                for (int n = 0; n < revealed.Where(s => s == cardId).Count(); n++)
                    revealed.Remove(cardId);

                showNextCard(obj, revealed, deck, cardType.item, false);
            }
            for (int i = 0; i < buildingsSelected.Count; i++)
            {
                List<int> revealed = details.buildingsRevealed;
                List<int> deck = details.buildingsDeck;
                GameObject obj = GameObject.Find("Canvas/" + unitObject.GetComponent<Card_Model>().faces[cardId].name);
                for (int n = 0; n < revealed.Where(s => s == cardId).Count(); n++)
                    revealed.Remove(cardId);

                showNextCard(obj, revealed, deck, cardType.unit, false);
            }
            hideCards(cards);
            if (details.combatPhase)
            {
                Movement.paused = false;
            }
            else
            {
                setButtons(true);
            }
        }
    }
    void Excalibur(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void Hammer(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void HolyGrail(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void Hourglass(GameObject cardObject, int itemId)
    {
        if(hourglassUsed || secondTurn)
        {
            showNotificationForTime("Only One", "You may only recieve second turn once per round.", 4);
        }
        else
        {
            useItem(cardObject, itemId, false);
            hourglassUsed = true;
        }
    }
    void InvincibilityCloak(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void OffHandKnife(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void Potion(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void Report(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    void RingOfInvisibility(GameObject cardObject, int itemId)
    {
        if (player.units.Where(s => s != null).Count() <= 0)
        {
            showNotificationForTime("No Units", String.Format("You have no units to use {0} on.", cardObject.name), 4);
            return;
        }
        else if(Movement.unitUsed)
        {
            showNotificationForTime("Unit Used", String.Format("You must use {0} before moving or attacking with a unit.", cardObject.name), 4);
            return;
        }
        useItem(cardObject, itemId, true);
        foreach (Unit u in player.units)
        {
            Movement.coordinates o;
            o.x = (int)u.XLocation;
            o.y = (int)u.YLocation;
            Movement.coordinates c = Movement.convertCoordinates(o, currentPlayer);
            Movement.makeSpace(c, Movement.selectionType.item, 0, o, delegate { selectUnitToMakeInvisible(u); });
        }
    }
    void selectUnitToMakeInvisible(Unit u)
    {
        if (details.combatPhase)
            Movement.paused = false;
        else
            setButtons(true);
    }
    void Sharpener(GameObject cardObject, int itemId)
    {
        if(player.units.Where(s => s != null).Count() <= 0)
        {
            showNotificationForTime("No Units", String.Format("You have no units to use {0} on.", cardObject.name), 4);
            return;
        }
        useItem(cardObject, itemId, true);
        foreach(Unit u in player.units)
        {
            Movement.coordinates o;
            o.x = (int)u.XLocation;
            o.y = (int)u.YLocation;
            Movement.coordinates c = Movement.convertCoordinates(o, currentPlayer);
            Movement.makeSpace(c, Movement.selectionType.item, 0, o, delegate { selectUnitToSharpen(u); });
        }
    }
    void selectUnitToSharpen(Unit u)
    {
        u.Atk += 1;
        sharpened.Add(u);
        if (details.combatPhase)
            Movement.paused = false;
        else
            setButtons(true);
    }
    void StolenResearch(GameObject cardObject, int itemId)
    {
        useItem(cardObject, itemId, true);
    }
    #endregion

    void startNextTurn()
    {
        endButton.SetActive(false);
        foreach (Unit u in player.units.Where(s => s != null))
        {
            u.Card.GetComponent<Movement>().restrictMovement();
        }
        if(!hourglassUsed)
        {
            currentPlayer = (currentPlayer + 1) % Game_Details_UI.players.Count(s => s != null);
            secondTurn = false;
        }
        else
        {
            hourglassUsed = false;
            secondTurn = true;
        }
        nextTurn();
        return;
    }

    private void startCombatPhase(bool required)
    {
        //Check for units, if none start next turn
        if (player.units.Count(s => s != null) > 0 || required)
        {
            saveDetails(true);

            //Check that gold is correct
            goldText.GetComponent<Text>().text = String.Format("{0}", player.gold);

            //Hide action buttons
            setButtons(false);

            foreach (Unit u in player.units.Where(s => s != null))
            {
                u.Card.GetComponent<Movement>().allowMovement(u);
            }

            endButton.SetActive(true);
            endButton.GetComponent<Button>().onClick.RemoveAllListeners();
            endButton.GetComponent<Button>().onClick.AddListener(delegate { endTurn(); });
        }
        else
        {
            startNextTurn();
        }
        return;
    }

    void endTurn()
    {
        //The end turn button is firing twice because the listener is being added by a listener (an issue Unity is aware of, but has not ficed yet)
        //End turn will check if endButton is active to fix this
        if (endButton.activeSelf)
        {
            //Check for bonuses that are only meant to last this turn


            endButton.SetActive(false);
            startNextTurn();
            return;
        }
        else
            return;
    }

    #endregion

    #region Player Actions
    private void Mine()
    {
        int goldToGain = 4;
        foreach(Building b in player.buildings.Where(s => s != null && s.Name == "Gold Mine"))
        {
            goldToGain++;
        }
        foreach(Worker w in player.workers.Where(s => s != null && s.IsActive))
        {
            if(w.Name == "Miner" && w.IsActive)
            {
                goldToGain++;
            }
            w.TurnsLeft = 1;
        }
        changeGold(goldToGain, player);
        startCombatPhase(false);
    }

    private void Buy(GameObject card, cardType type)
    {
        bool researched = false;
        //Check if card is researched.  If so, check for research discounts.
        if(card.transform.parent == playerDetailsResearchObject.transform)
        {
            researched = true;
        }
        int cost;
        int time;
        var ds = new DataService("Furtherance.db");
        switch(type)
        {
            case cardType.building:
                List<Building> building = ds.GetBuilding(card.name).ToList();
                int[] costAndTime = checkForBuildingDiscounts(building[0].Cost, building[0].Time, researched);
                cost = costAndTime[0];
                time = costAndTime[1];
                //Give warning to player that they don't have enough gold
                if (cost > player.gold)
                {
                    StartCoroutine(showNotificationForTime("Not Enough Gold", string.Format(
                        "The {0} costs {1} gold, but you only have {2} gold to spend.", 
                        building[0].Name, 
                        cost,
                        player.gold), 4));
                    return;
                }
                //TODO: Allow player to select which building they want to replace
                else if (player.buildings.Count(s => s != null) >= 7) {

                }
                else
                {
                    foreach(Worker w in player.workers.Where(w => w != null))
                    {
                        if(w.IsActive)
                        {
                            w.Building = building[0];
                            w.TurnsLeft = time;
                        }
                    }
                    payForCard(card, building[0], details.buildingsRevealed, details.buildingsDeck, cardType.building, cost);
                }
                break;
            case cardType.item:
                List<Item> item = ds.GetItem(card.name).ToList();
                //Give warning to player that they don't have enough gold
                cost = checkForItemDiscounts(item[0].Cost, researched);
                if (cost > player.gold)
                {
                    StartCoroutine(showNotificationForTime("Not Enough Gold", string.Format(
                        "The {0} costs {1} gold, but you only have {2} gold to spend.", 
                        item[0].Name, 
                        cost, 
                        player.gold), 4));
                    return;
                }
                //Allow player to select which item they want to replace
                else if (player.items.Count(s => s != null) >= 5)
                {

                }
                else
                {
                    foreach (Worker w in player.workers.Where(w => w != null))
                    {
                        if (w.IsActive)
                        {
                            w.TurnsLeft = 1;
                            break;
                        }
                    }
                    player.items[player.items.Count(s => s != null)] = item[0];
                    payForCard(card, item[0], details.itemsRevealed, details.itemsDeck, cardType.item, cost);
                }
                break;
            case cardType.unit:
                List<Unit> unit = ds.GetUnit(card.name).ToList();
                cost = checkForUnitDiscounts(unit[0].Cost, researched);
                if (cost > player.gold)
                {
                    StartCoroutine(showNotificationForTime("Not Enough Gold", string.Format(
                        "The {0} costs {1} gold, but you only have {2} gold to spend.", 
                        unit[0].Name, 
                        cost, 
                        player.gold), 4));
                    return;
                }
                //Not allowed to kill units to recruit more
                else if (player.units.Count(s => s != null) >= 6)
                {
                    StartCoroutine(showNotificationForTime("Too Many Units", string.Format(
                        "You may not buy the {0} because you already have six units in play.",
                        unit[0].Name), 4));
                    return;
                }
                else
                {
                    foreach (Worker w in player.workers.Where(w => w != null))
                    {
                        if (w.IsActive)
                        {
                            w.TurnsLeft = 1;
                            break;
                        }
                    }
                    payForCard(card, unit[0], details.unitsRevealed, details.unitsDeck, cardType.unit, cost);
                    setButtons(false);

                    //Check the number of units in castle.  If it is 3 stop the purchase,
                    //if it is two autoselect the placement for the unit, if it is 1 or 0,
                    //let the player select.
                    int unitsInCastle = 0;
                    bool leftSpot = false;
                    bool middleSpot = false;
                    bool rightSpot = false;
                    //Check if the player has already filled the spots
                    foreach(Unit u in player.units.Where(s => s != null))
                    {
                        if(Math.Abs((decimal)u.XLocation) == 3) {
                            unitsInCastle++;
                            switch(u.YLocation) {
                                case -1:
                                    leftSpot = true;
                                    break;
                                case 0:
                                    middleSpot = true;
                                    break;
                                case 1:
                                    rightSpot = true;
                                    break;
                            }
                        }
                        else if (Math.Abs((decimal)u.YLocation) == 3)
                        {
                            unitsInCastle++;
                            switch (u.XLocation)
                            {
                                case -1:
                                    leftSpot = true;
                                    break;
                                case 0:
                                    middleSpot = true;
                                    break;
                                case 1:
                                    rightSpot = true;
                                    break;
                            }
                        }
                    }
                    Vector3 position = new Vector3();
                    Vector3 firstPosition = new Vector3(width / 4.1925f - width / 15.68f, height / 4.2365f);
                    Vector3 secondPosition = new Vector3(width / 4.1925f, height / 4.2365f);
                    Vector3 thirdPosition = new Vector3(width / 4.1925f + width / 15.68f, height / 4.2365f);
                    if (unitsInCastle == 3)
                    {
                        StartCoroutine(showNotificationForTime("Castle Is Full", string.Format(
                        "You may not buy the {0} because you have nowhere to place it in your castle",
                        unit[0].Name), 4));
                        return;
                    }
                    else if(unitsInCastle == 2)
                    {
                        if(leftSpot == false)
                        {
                            switch(player.order)
                            {
                                case 0:
                                    unit[0].XLocation = -1;
                                    unit[0].YLocation = -3;
                                    break;
                                case 1:
                                    unit[0].XLocation = -3;
                                    unit[0].YLocation = 1;
                                    break;
                                case 2:
                                    unit[0].XLocation = 1;
                                    unit[0].YLocation = 3;
                                    break;
                                case 3:
                                    unit[0].XLocation = 3;
                                    unit[0].YLocation = -1;
                                    break;
                            }
                            position = firstPosition;
                        }
                        else if(middleSpot == false)
                        {
                            switch (player.order)
                            {
                                case 0:
                                    unit[0].XLocation = 0;
                                    unit[0].YLocation = -3;
                                    break;
                                case 1:
                                    unit[0].XLocation = -3;
                                    unit[0].YLocation = 0;
                                    break;
                                case 2:
                                    unit[0].XLocation = 0;
                                    unit[0].YLocation = 3;
                                    break;
                                case 3:
                                    unit[0].XLocation = 3;
                                    unit[0].YLocation = 0;
                                    break;
                            }
                            position = secondPosition;
                        }
                        else if (rightSpot == false)
                        {
                            switch (player.order)
                            {
                                case 0:
                                    unit[0].XLocation = 1;
                                    unit[0].YLocation = -3;
                                    break;
                                case 1:
                                    unit[0].XLocation = -3;
                                    unit[0].YLocation = -1;
                                    break;
                                case 2:
                                    unit[0].XLocation = -1;
                                    unit[0].YLocation = 3;
                                    break;
                                case 3:
                                    unit[0].XLocation = 3;
                                    unit[0].YLocation = 1;
                                    break;
                            }
                            position = thirdPosition;
                        }
                    }
                    else
                    {
                        //Allow player to select which spot his unit goes to (unless two out of three are filled)
                        showNotification(String.Format("Place Your {0}", unit[0].Name), "");

                        GameObject prefab = highlightedSpace;
                        Transform parent = canvas.transform;
                        List<GameObject> selections = new List<GameObject>();
                        if (leftSpot == false)
                        {
                            GameObject selection = GameObject.Instantiate(prefab, firstPosition, Quaternion.identity, parent);
                            Button button = selection.GetComponent<Button>();
                            selection.GetComponent<Image>().color = new Color(0, 0, 1, 0.49f);
                            selections.Add(selection);
                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(delegate { unitPlacement(unit[0], firstPosition, selections, 1); });
                        }
                        if (middleSpot == false)
                        {
                            GameObject selection = GameObject.Instantiate(prefab, secondPosition, Quaternion.identity, parent);
                            Button button = selection.GetComponent<Button>();
                            selection.GetComponent<Image>().color = new Color(0, 0, 1, 0.49f);
                            selections.Add(selection);
                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(delegate { unitPlacement(unit[0], secondPosition, selections, 2); });
                        }
                        if (rightSpot == false)
                        {
                            GameObject selection = GameObject.Instantiate(prefab, thirdPosition, Quaternion.identity, parent);
                            Button button = selection.GetComponent<Button>();
                            selection.GetComponent<Image>().color = new Color(0, 0, 1, 0.49f);
                            selections.Add(selection);
                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(delegate { unitPlacement(unit[0], thirdPosition, selections, 3); });
                        }
                        startCombatPhase(true);
                        Movement.paused = true;
                        endButton.SetActive(false);
                        return;
                    }
                    unit[0].Card = placeUnitOnBoard(position, unit[0]);
                    player.units[player.units.Count(s => s != null)] = unit[0];
                }
                break;
        }

        if (type == cardType.item)
        {
            for (int i = 0; i < 3; i++)
            {
                string s = itemObject.GetComponent<Card_Model>().faces[details.itemsRevealed[i]].name;

                List<Item> item = ds.GetItem(s).ToList();

                if (item[0].Cost <= player.gold)
                {
                    setButtons(false);
                    Button[] buttons = showButtons("Another Item?", "Would you like to purchase a second item", "Yes", "No");
                    buttons[0].onClick.RemoveAllListeners();
                    buttons[0].onClick.AddListener(delegate { loadItemBuyButtons(); });
                    buttons[1].onClick.RemoveAllListeners();
                    buttons[1].onClick.AddListener(delegate { finishItemBuy(player); });
                    return;
                }
            }
        }
        if (!checkWorkers(player))
        {
            startCombatPhase(false);
        }
        return;
    }

        #region Buy Helper Methods
    
        void payForCard(GameObject card, object obj, List<int> revealed, List<int> deck, cardType type, int cost)
        {
            bool duplicates = false;
            changeGold(-cost, player);

            foreach (Building b in player.buildings.Where(s => s != null && s != null && s.Name == "Marketplace"))
            {
                changeGold(1, player);
            }

            switch (type)
            {
                case cardType.building:
                    Building building = (Building)obj;
                    revealed.Remove(building.Id);
                    duplicates = revealed.Where(s => s == building.Id).Count() > 0;
                    break;
                case cardType.unit:
                    Unit unit = (Unit)obj;
                    revealed.Remove(unit.Id);
                    duplicates = revealed.Where(s => s == unit.Id).Count() > 0;
                    break;
                case cardType.item:
                    Item item = (Item)obj;
                    revealed.Remove(item.Id);
                    duplicates = revealed.Where(s => s == item.Id).Count() > 0;
                    break;
            }
            showNextCard(card, revealed, deck, type, duplicates);
        }

        void showNextCard(GameObject card, List<int> revealed, List<int> deck, cardType type, bool extraInPlay)
        {
            int n = 0;
            if (!extraInPlay)
            {
                Vector3 location = new Vector3();
                if (Zoomed)
                {
                    if (card.transform.position.x == width / 2)
                    {
                        location = Card_Zoom.startLocation;
                        Zoomed = false;
                    }
                    else
                        location = card.transform.position;
                }
                else
                {
                    location = card.transform.position;
                }
                n = cardDuplicateCheck(location, 0, 0, deck, type, false);
                Vector3[] locations = new Vector3[1];
                locations[0] = location;
                StartCoroutine(LoadButtons(locations, buttonType.buy, 0, type, null, true));
                buttons.Remove(GameObject.Find("Canvas/" + card.name + "/BuyButton"));
                Destroy(card);
            }
            else
            {
                if (type == cardType.unit)
                {
                    Destroy(GameObject.Find("Canvas/Duplicates/" + card.name + "/" + card.name + GameObject.Find("Canvas/Duplicates/" + card.name).transform.childCount));
                }
                else
                    Destroy(GameObject.Find("Canvas/Duplicates/" + card.name));

                if (card.transform.position.x == width / 2)
                {
                    unzoomCard(card);
                }
            }
            for (int m = 0; m < n; m++)
            {
                int cardIndex = deck[0];
                revealed.Add(cardIndex);
                deck.Remove(cardIndex);
            }
        }

            #region Item Buy Methods
            void loadItemBuyButtons()
                    {
                        moving = false;

                        GameObject obj = buyButton;
                        Vector3[] positions = getItemPositions();
                        List<int> list = new List<int>();
                        foreach (int i in details.itemsRevealed)
                            list.Add(i);
                        while (list.Count > 3)
                        {
                            if (list[0] == list[1])
                            {
                                list.Remove(list[1]);
                            }
                            else if (list[2] == list[1])
                            {
                                list.Remove(list[2]);
                            }
                        }
                        List<Button> buyButtons = new List<Button>();
                        List<GameObject> buttonObjects = new List<GameObject>();
                        List<Item> items = new List<Item>();
                        int cost = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            string s = itemObject.GetComponent<Card_Model>().faces[list[i]].name;

                            var ds = new DataService("Furtherance.db");

                            List<Item> item = ds.GetItem(s).ToList();

                            cost = checkForItemDiscounts(item[0].Cost, false);

                            if (player.gold >= cost)
                            {
                                Transform t = GameObject.Find("Canvas/" + item[0].Name).transform;
                                Vector3 position = t.position;
                                position.y += height / 9.25f;
                                GameObject buttonObject = GameObject.Instantiate(obj, position, Quaternion.identity, t);
                                buttonObject.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
                                buttonObject.transform.position = position;
                                Button button = buttonObject.GetComponent<Button>();
                                button.onClick.RemoveAllListeners();
                                items.Add(item[0]);

                                buyButtons.Add(button);
                                buttonObjects.Add(buttonObject);
                            }
                        }

                        //If only one item can be bought, buy it automatically.
                        if (buyButtons.Count == 1)
                        {
                            Item i = items[0];
                            cost = checkForItemDiscounts(i.Cost, false);
                            BuySecondItem(items[0], buttonObjects, cost);
                        }
                        else
                        {
                            int n = 0;
                            foreach (Button b in buyButtons)
                            {
                                Item i = items[n];
                                cost = checkForItemDiscounts(i.Cost, false);
                                b.onClick.AddListener(delegate { BuySecondItem(i, buttonObjects, cost); });
                                n++;
                            }
                        }

                        hideNotification();
                    }

            void BuySecondItem(Item item, List<GameObject> buyButtons, int cost)
            {
                if (player.items.Count(s => s != null) >= 5)
                {

                }
                else
                {
                    player.items[player.items.Count(s => s != null)] = item;
                    GameObject obj = GameObject.Find("Canvas/" + item.Name);
                    payForCard(obj, item, details.itemsRevealed, details.itemsDeck, cardType.item, cost);
                }

                destroyAll(buyButtons);

                finishItemBuy(player);
            }

            void finishItemBuy(Player player)
                {
                    if (!checkWorkers(player))
                    {
                        startCombatPhase(false);
                    }
                    else
                    {
                        setButtons(true);
                    }
                    hideNotification();
                }
            #endregion

            #region Unit Buy Methods

            GameObject placeUnitOnBoard(Vector3 position, Unit unit)
            {
            GameObject obj = GameObject.Instantiate(unitMovementObject, position, Quaternion.identity, boardObject.transform);
            obj.GetComponent<RectTransform>().localScale -= new Vector3(0.275f, 0.275f, 0);
            obj.GetComponent<Image>().sprite = obj.GetComponent<Card_Model>().faces[unit.Id];
            obj.name = unit.Name;

            if (player.leader.Name == "Fader")
                unit.justPlayed = false;
            else
                unit.justPlayed = true;

            if (unit.Name == "Civilians")
                changeVP(1, player);
            
            return obj;
            }

    private void unitPlacement(Unit unit, Vector3 position, List<GameObject> selections, int spot)
    {
        //Set intial spots
        switch (spot)
        {
            case 1:
                unit.XLocation = -1;
                break;
            case 2:
                unit.XLocation = 0;
                break;
            case 3:
                unit.XLocation = 1;
                break;
        }
        unit.YLocation = -3;

        //Adjust spots based on which player is current
        int x;
        int y;
        switch (currentPlayer)
        {
            case 0:
                break;
            case 1:
                x = (int)unit.XLocation;
                y = (int)unit.YLocation;
                unit.XLocation = y;
                unit.YLocation = x;
                unit.YLocation *= -1;
                break;
            case 2:
                unit.YLocation *= -1;
                unit.XLocation *= -1;
                break;
            case 3:
                x = (int)unit.XLocation;
                y = (int)unit.YLocation;
                unit.XLocation = y;
                unit.YLocation = x;
                unit.XLocation *= -1;
                break;
        }

        unit.Card = placeUnitOnBoard(position, unit);
        unit.Damage = 0;
        player.units[player.units.Count(s => s != null)] = unit;
        destroyAll(selections);
        hideNotification();
        Movement.paused = false;
        endButton.SetActive(true);
        if (!checkWorkers(player))
        {
            startCombatPhase(true);
        }
        else
        {
            setButtons(true);
        }
    }

    #endregion

    #endregion

    private void Research(cardType type)
    {
        if (Card_Zoom.zoomedCard != null)
        {
            unzoomCard(Card_Zoom.zoomedCard);
        }
        topDecked = false;
        bottomDecked = false;
        researched = false;
        List<int> intList = new List<int>();
        List<GameObject> topButtons = new List<GameObject>(); ;
        List<GameObject> researchButtons = new List<GameObject>();
        List<GameObject> bottomButtons = new List<GameObject>();
        List<GameObject> cards = new List<GameObject>();
        GameObject obj = new GameObject();
        for(int i = 0; i < 3; i++)
        {
            switch (type)
            {
                case cardType.building:
                    intList.Add(details.buildingsDeck[0]);
                    details.buildingsDeck.Remove(intList[i]);
                    obj = buildingObject;
                    break;
                case cardType.item:
                    intList.Add(details.itemsDeck[0]);
                    details.itemsDeck.Remove(intList[i]);
                    obj = itemObject;
                    break;
                case cardType.unit:
                    intList.Add(details.unitsDeck[0]);
                    details.unitsDeck.Remove(intList[i]);
                    obj = unitObject;
                    break;
            }
            GameObject card = GameObject.Instantiate(obj, new Vector3(), Quaternion.identity, cardChoice.transform);
            cards.Add(card);
            card.GetComponent<Image>().sprite = obj.GetComponent<Card_Model>().faces[intList[i]];
        }
        List<Vector3> positions = showCards("Researching...", cards);
        cardChoiceWindow.transform.localScale += new Vector3(0, 0.3f, 0);
        cardChoiceWindow.transform.position -= new Vector3(0, (45f / 760f) * height, 0);
        int n = 0;
        foreach(GameObject card in cards)
        {
            card.transform.position = positions[n];
            GameObject topButton = GameObject.Instantiate(topdeckButton, card.transform.position - new Vector3(0, (110f / 760f) * height), Quaternion.identity, card.transform);
            topButton.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            topButtons.Add(topButton);
            int m = intList[n];
            GameObject researchButtonObj = GameObject.Instantiate(researchButton, card.transform.position - new Vector3(0, (145f / 760f) * height), Quaternion.identity, card.transform);
            researchButtonObj.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            researchButtons.Add(researchButtonObj);
            GameObject bottomButtonObj = GameObject.Instantiate(bottomButton, card.transform.position - new Vector3(0, (180f / 760f) * height), Quaternion.identity, card.transform);
            bottomButtonObj.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            bottomButtons.Add(bottomButtonObj);
            topButton.GetComponent<Button>().onClick.AddListener(delegate { topDeck(card, m, type, topButtons, researchButtons, bottomButtons, cards); });
            researchButtonObj.GetComponent<Button>().onClick.AddListener(delegate { addToResearch(card, m, type, topButtons, researchButtons, bottomButtons, cards); });
            bottomButtonObj.GetComponent<Button>().onClick.AddListener(delegate { bottomDeck(card, m, type, topButtons, researchButtons, bottomButtons, cards); });
            n++;
        }
    }

        #region Research Helper Methods
        public static void removeOneFromResearch()
        {
            
        }

        void topDeck(GameObject card, int cardIndex, cardType type, List<GameObject> topButtons, List<GameObject> researchButtons, List<GameObject> bottomButtons, List<GameObject> cards)
        {
            switch (type)
            {
                case cardType.building:
                    details.buildingsDeck.Insert(0, cardIndex);
                    break;
                case cardType.item:
                    details.itemsDeck.Insert(0, cardIndex);
                    break;
                case cardType.unit:
                    details.unitsDeck.Insert(0, cardIndex);
                    break;
            }
            Destroy(card);
            destroyAll(topButtons);
            topDecked = true;
            GameObject c = checkCards(cards);
            if (!bottomDecked && c != null)
            {
                bottomDeck(c, cardIndex, type, topButtons, researchButtons, bottomButtons, cards);
            }
            else if (!researched && c != null)
            {
                addToResearch(c, cardIndex, type, topButtons, researchButtons, bottomButtons, cards);
            }
            else if (c != null)
            {
                hideCards(cards);
                startCombatPhase(false);
            }
            if (Card_Zoom.zoomedCard != null)
            {
                unzoomCard(Card_Zoom.zoomedCard);
            }
        }

        void addToResearch(GameObject card, int cardIndex, cardType type, List<GameObject> topButtons, List<GameObject> researchButtons, List<GameObject> bottomButtons, List<GameObject> cards)
        {
            Player p = player;
            var ds = new DataService("Furtherance.db");
            switch (type)
            {
                case cardType.building:
                    string b = buildingObject.GetComponent<Card_Model>().faces[cardIndex].name;
                    List<Building> building = ds.GetBuilding(b).ToList();
                    if (p.research.Count(s => s != null) == 2)
                        removeOneFromResearch();
                    p.research[p.research.Count(s => s != null)] = building[0];
                    break;
                case cardType.item:
                    string i = itemObject.GetComponent<Card_Model>().faces[cardIndex].name;
                    List<Item> item = ds.GetItem(i).ToList();
                    if (p.research.Count(s => s != null) == 2)
                        removeOneFromResearch();
                    p.research[p.research.Count(s => s != null)] = item[0];
                    break;
                case cardType.unit:
                    string u = unitObject.GetComponent<Card_Model>().faces[cardIndex].name;
                    List<Unit> unit = ds.GetUnit(u).ToList();
                    if (p.research.Count(s => s != null) == 2)
                        removeOneFromResearch();
                    p.research[p.research.Count(s => s != null)] = unit[0];
                    break;
            }
            Destroy(card);
            destroyAll(researchButtons);
            researched = true;
            GameObject c = checkCards(cards);
            if (!bottomDecked && c != null)
            {
                bottomDeck(c, cardIndex, type, topButtons, researchButtons, bottomButtons, cards);
            }
            else if (!topDecked && c != null)
            {
                topDeck(c, cardIndex, type, topButtons, researchButtons, bottomButtons, cards);
            }
            else if(c != null)
            {
                hideCards(cards);
                startCombatPhase(false);
            }
            if (Card_Zoom.zoomedCard != null)
            {
                unzoomCard(Card_Zoom.zoomedCard);
            }
        }

        void bottomDeck(GameObject card, int cardIndex, cardType type, List<GameObject> topButtons, List<GameObject> researchButtons, List<GameObject> bottomButtons, List<GameObject> cards)
        {
            switch (type)
            {
                case cardType.building:
                    details.buildingsDeck.Add(cardIndex);
                    break;
                case cardType.item:
                    details.itemsDeck.Add(cardIndex);
                    break;
                case cardType.unit:
                    details.unitsDeck.Add(cardIndex);
                    break;
            }
            Destroy(card);
            destroyAll(bottomButtons);
            bottomDecked = true;
            GameObject c = checkCards(cards);
            if (!bottomDecked && c != null)
            {
                bottomDeck(c, cardIndex, type, topButtons, researchButtons, bottomButtons, cards);
            }
            else if (!topDecked && c != null)
            {
                topDeck(c, cardIndex, type, topButtons, researchButtons, bottomButtons, cards);
            }
            else if (c != null)
            {
                hideCards(cards);
                startCombatPhase(false);
            }
            if (Card_Zoom.zoomedCard != null)
            {
                unzoomCard(Card_Zoom.zoomedCard);
            }
        }

        GameObject checkCards(List<GameObject> cards)
        {
            GameObject card = new GameObject();
            int cardsDestroyed = 0;
            foreach (GameObject c in cards)
            {
                if (c == null)
                {
                    cardsDestroyed++;
                }
                else
                {
                    card = c;
                }
            }
            if (cardsDestroyed == 1)
            {
                return card;
            }
            else
            {
                return null;
            }
        }
        #endregion

    private void Upgrade()
    {
        int cost = 0;
        int turns = 0;
        if(player.leader.Name == "Trosh")
        {
            cost = 2;
            turns = 2;
        }
        else
        {
            cost = 5;
            turns = 4;
        }
        if(player.gold < cost)
        {
            StartCoroutine(showNotificationForTime("Not Enough Gold", string.Format(
                        "Upgrading costs {0} gold, but you only have {1} gold to spend.",
                        cost.ToString(),
                        player.gold), 4));
            return;
        }
        List<GameObject> cards = new List<GameObject>();
        int startInt = 0;
        if (details.workersDeck[0] == details.workersDeck[1])
            startInt = 1;
        List<int> intList = new List<int>();
        if(player.leader.Name == "Trosh")
        {
            for (int i = 1; i < 6; i++)
            {
                GameObject card = GameObject.Instantiate(workerObject, new Vector3(), Quaternion.identity, cardChoice.transform);
                cards.Add(card);
                card.GetComponent<Image>().sprite = card.GetComponent<Card_Model>().faces[i];
                intList.Add(i);
            }
        }
        else
        {
            for (int i = startInt; i < startInt + 2; i++)
            {
                GameObject card = GameObject.Instantiate(workerObject, new Vector3(), Quaternion.identity, cardChoice.transform);
                cards.Add(card);
                card.GetComponent<Image>().sprite = card.GetComponent<Card_Model>().faces[details.workersDeck[i]];
                intList.Add(details.workersDeck[i]);
                details.workersDeck.Remove(details.workersDeck[i]);
            }
        }
        List<Vector3> positions = showCards("Upgrading...", cards);
        int n = 0;
        foreach (GameObject card in cards)
        {
            card.transform.position = positions[n];
            GameObject upgradeButtonObj = GameObject.Instantiate(upgradeButton, card.transform.position - new Vector3(0, height/21f), Quaternion.identity, card.transform);
            upgradeButtonObj.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            int m = intList[n];
            upgradeButtonObj.GetComponent<Button>().onClick.AddListener(delegate { chooseUpgrade(card, m, intList, cards); });
            n++;
        }
        setButtons(false);
    }

    #region Upgrade Helper Methods
    private void chooseUpgrade(GameObject card, int index, List<int> intList, List<GameObject> cards)
    {
        var ds = new DataService("Furtherance.db");
        string w = card.GetComponent<Card_Model>().faces[index].name;
        List<Worker> worker = ds.GetWorker(w).ToList();
        for(int i = 0; i < player.workers.Count(s => s != null); i++)
        {
            if(player.workers[i].IsActive)
            {
                player.workers[i] = worker[0];
                player.workers[i].IsActive = false;
                player.workers[i].TurnsLeft = 3;
                break;
            }
        }
        intList.Remove(index);
        foreach(int i in intList) 
        {
            details.workersDeck.Add(index);

        }
        destroyAll(cards);
        hideCards(cards);
        startCombatPhase(false);
        if (Card_Zoom.zoomedCard != null)
        {
            unzoomCard(Card_Zoom.zoomedCard);
        }
    }
    #endregion

    #endregion

    #region Show and Hide Methods

    IEnumerator showNotificationForTime(string title, string message, int time)
    {
        notificationWindow.SetActive(true);
        nTitle.GetComponent<Text>().text = title;
        nBody.GetComponent<Text>().text = message;
        nWindow.transform.SetAsLastSibling();
        yield return new WaitForSeconds((int)time);
        notificationWindow.SetActive(false);
    }

    static void showNotification(string title, string message)
    {
        notificationWindow.SetActive(true);
        notificationTitle.GetComponent<Text>().text = title;
        notificationBody.GetComponent<Text>().text = message;
        notificationWindow.transform.SetAsLastSibling();
    }

    public static List<Vector3> showCards(string title, List<GameObject> cards)
    {
        cardChoice.SetActive(true);
        cardChoiceTitle.GetComponent<Text>().text = title;
        cardChoice.transform.SetAsLastSibling();
        cardChoiceWindow.transform.localScale = new Vector3(cards.Count, 1f, 0);
        List<Vector3> positions = new List<Vector3>();
        Vector3 position = new Vector3();
        float y = (height / 2) - ((5f/760f)*height);
        if (cards.Count % 2 == 0)
        {
            for(int i = 0; i < cards.Count; i++)
            {
                if(i % 2 == 0)
                {
                    float m = (62.5f / 1350f) * width * ((i / 2) + 1);
                    position = new Vector3((width / 2) - m , y, 0);
                }
                else
                {
                    float m = (62.5f / 1350f) * width * ((i + 1) / 2);
                    position = new Vector3((width / 2) + m, y, 0);
                }
                positions.Add(position);
            }
        }
        else
        {
            cards[0].transform.position = new Vector3(width / 2, y, 0);
            positions.Add(cards[0].transform.position);
            for (int i = 1; i < cards.Count; i++)
            {
                if (i % 2 == 0)
                {
                    float m = (125f / 1350f) * width * (i / 2);
                    position = new Vector3(width / 2 - m, y, 0);
                }
                else
                {
                    float m = (125f / 1350f) * width * ((i + 1) / 2);
                    position = new Vector3(width / 2 + m, y, 0);
                }
                positions.Add(position);
            }
        }
        return positions;
    }

    public static void hideCards(List<GameObject> cards)
    {
        destroyAll(cards);
        cardChoiceWindow.transform.localScale = new Vector3(1f, 1f, 0);
        cardChoiceWindow.transform.position = new Vector3(width / 2f, height / 2f, 0);
        cardChoice.SetActive(false);
    }

    public static Button[] showButtons(string title, string message, string button1Text, string button2Text)
    {
        showNotification(title, message);
        notificationButtons.SetActive(true);
        notificationButton1Text.GetComponent<Text>().text = button1Text;
        notificationButton2Text.GetComponent<Text>().text = button2Text;
        Button[] buttons = new Button[2];
        buttons[0] = notificationButton1.GetComponent<Button>();
        buttons[1] = notificationButton2.GetComponent<Button>();
        return buttons;
    }

    public static void hideNotification()
    {
        notificationWindow.SetActive(false);
        notificationButtons.SetActive(false);
    }

    private void ShowPlayerCards(cardType type, object[] sprites, Player player)
    {
        int count = 0;
        int spriteCounter = 0;
        string[] cardNames = new string[7];
        int[] spriteIndices = new int[7];
        GameObject obj = new GameObject();
        Transform parent;
        Vector3 startPosition = new Vector3();
        switch (type)
        {
            case cardType.worker:
                foreach (object s in sprites.Where(s => s != null))
                {
                    spriteIndices[spriteCounter] = ((Worker)s).Id;
                    cardNames[spriteCounter] = ((Worker)s).Name;
                    spriteCounter++;
                }
                count = player.workers.Count(s => s != null);
                obj = workerObject;
                parent = playerDetailsWorkerObject.transform;
                startPosition = new Vector3(width / 1.55f, height / 3.8f, 0);
                break;
            case cardType.building:
                foreach (object s in sprites.Where(s => s != null))
                {
                    spriteIndices[spriteCounter] = ((Building)s).Id;
                    cardNames[spriteCounter] = ((Building)s).Name;
                    spriteCounter++;
                }
                count = player.buildings.Count(s => s != null);
                obj = buildingObject;
                parent = playerDetailsBuildingsObject.transform;
                startPosition = new Vector3(width / 1.55f, height / 16.4f, 0);
                break;
            case cardType.item:
                foreach (object s in sprites.Where(s => s != null))
                {
                    spriteIndices[spriteCounter] = ((Item)s).Id;
                    cardNames[spriteCounter] = ((Item)s).Name;
                    spriteCounter++;
                }
                count = player.items.Count(s => s != null);
                obj = itemObject;
                parent = playerDetailsItemsObject.transform;
                startPosition = new Vector3(width / 1.55f + 5 * width / 27f, height / 3.8f, 0);
                break;
            case cardType.research:
                //This needs to be treated differently because researched cards could be of any type
                count = player.research.Count(s => s != null);
                parent = playerDetailsResearchObject.transform;
                startPosition = new Vector3(width / 1.55f + 8 * width / 27f, height / 16.4f, 0);
                break;
            default:
                parent = playerDetails.transform;
                break;
        }
        for (int i = 0; i < count; i++)
        {
            if (type == cardType.research)
            {
                if (player.research[i].GetType() == typeof(Unit))
                {
                    obj = unitObject;
                    spriteIndices[i] = ((Unit)player.research[i]).Id;
                    cardNames[i] = ((Unit)player.research[i]).Name;
                }
                else if (player.research[i].GetType() == typeof(Building))
                {
                    obj = buildingObject;
                    spriteIndices[i] = ((Building)player.research[i]).Id;
                    cardNames[i] = ((Building)player.research[i]).Name;
                }
                else if (player.research[i].GetType() == typeof(Item))
                {
                    obj = itemObject;
                    spriteIndices[i] = ((Item)player.research[i]).Id;
                    cardNames[i] = ((Item)player.research[i]).Name;
                }
            }
            GameObject card = GameObject.Instantiate(obj, startPosition + new Vector3(i * width / 27f, 0, 0), Quaternion.identity, parent);
            if (type == cardType.research)
            {
                if (player.research[i].GetType() == typeof(Unit))
                {
                    card.GetComponent<RectTransform>().localScale += new Vector3(0.15f, 0.15f, 0);
                }
            }
            card.GetComponent<RectTransform>().localScale -= new Vector3(0.6f, 0.6f, 0);
            card.GetComponent<Image>().sprite = card.GetComponent<Card_Model>().faces[spriteIndices[i]];
            card.name = cardNames[i];
            if(type == cardType.worker && spriteIndices[i] == 0 || type == cardType.research)
            {
                GameObject[] cards = new GameObject[] { card };
                Vector3[] positions = new Vector3[] { card.transform.position };
                if(type == cardType.research)
                {
                    if (player.research[i].GetType() == typeof(Unit))
                    {
                        StartCoroutine(LoadButtons(positions, buttonType.buy, 0, cardType.unit, cards, false));
                    }
                    else if (player.research[i].GetType() == typeof(Building))
                    {
                        StartCoroutine(LoadButtons(positions, buttonType.buy, 0, cardType.building, cards, false));
                    }
                    else if (player.research[i].GetType() == typeof(Item))
                    {
                        StartCoroutine(LoadButtons(positions, buttonType.buy, 0, cardType.item, cards, false));
                    }
                }
                else
                {
                    StartCoroutine(LoadButtons(positions, buttonType.upgradeWorker, 0, cardType.worker, cards, false));
                }
            }
            playerCards.Add(card);
        }
    }

    private void setButtons(bool value)
    {
        foreach (GameObject o in buttons)
        {
            if (o != null)
                o.SetActive(value);
        }
    }

    #endregion

    #region Other Methods

    Vector3[] getItemPositions()
    {
        Vector3[] items = { new Vector3(width / 1.5f, height / 2f, 0), new Vector3(width / 1.276f, height / 2f, 0), new Vector3(width / 1.11f, height / 2f, 0) };

        return items;
    }

    IEnumerator LoadButtons(Vector3[] positions, buttonType buttonType, int flipIndex, cardType? card, GameObject[] parentObjects, bool delay)
    {
        //Load buttons is used to load buttons of any type into a list of positions.  Card type can be specified, if necessary and parent objects will be assigned by card flip if not otherwise specified
        //Wait for card to flip
        if(delay)
            yield return new WaitForSeconds(1);
        moving = false;
        
        GameObject obj = new GameObject();
        switch (buttonType)
        {
            case buttonType.leader:
                obj = leaderButton;
                break;
            case buttonType.buy:
                obj = buyButton;
                break;
            case buttonType.research:
                obj = researchButton;
                break;
            case buttonType.selectWorker:
                obj = workerButton;
                break;
            case buttonType.upgradeWorker:
                obj = upgradeButton;
                break;
        }
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 buttonPosition = positions[i];
            if(buttonType == buttonType.leader)
                buttonPosition.y -= height/6f;
            else
            {
                if (card == cardType.unit)
                {
                    buttonPosition.y += height / 14f;
                }
                else if (!delay)
                {
                    buttonPosition.y -= height / 21f;
                }
                else
                {
                    buttonPosition.y += height / 9.25f;
                }
            }
            GameObject buttonObject = new GameObject();
            if (buttonType == buttonType.buy && delay || buttonType == buttonType.leader)
            {
                buttonObject = GameObject.Instantiate(obj, buttonPosition, Quaternion.identity, FlipCard.cards[i + flipIndex].transform);
            }
            else
            {
                buttonObject = GameObject.Instantiate(obj, buttonPosition, Quaternion.identity, parentObjects[i].transform);
            }

            buttons.Add(buttonObject);

            if (gameDetails.Count > 0 && ((cardType.unit == card && details.combatPhase) || !details.combatPhase))
            {
                buttonObject.SetActive(false);
            }

            if (card == cardType.unit)
                buttonObject.GetComponent<RectTransform>().localScale -= new Vector3(0.8f, 0.8f, 0);
            else
                buttonObject.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
            buttonObject.transform.position = buttonPosition;
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            switch (buttonType)
            {
                case buttonType.leader:
                    button.onClick.AddListener(delegate { LeaderChosen(buttonObject.transform.parent.gameObject); });
                    break;
                case buttonType.selectWorker:
                    button.onClick.AddListener(delegate { LeaderChosen(buttonObject.transform.parent.gameObject); });
                    break;
                case buttonType.upgradeWorker:
                    button.onClick.AddListener(delegate { Upgrade(); });
                    break;
                case buttonType.buy:
                    switch (card)
                    {
                        case cardType.building:
                            button.onClick.AddListener(delegate { Buy(buttonObject.transform.parent.gameObject, (cardType)card); });
                            break;
                        case cardType.item:
                            button.onClick.AddListener(delegate { Buy(buttonObject.transform.parent.gameObject, (cardType)card); });
                            break;
                        case cardType.unit:
                            button.onClick.AddListener(delegate { Buy(buttonObject.transform.parent.gameObject, (cardType)card); });
                            break;
                    }
                    break;
                case buttonType.research:
                    switch (card)
                    {
                        case cardType.building:
                            button.onClick.AddListener(delegate { Research((cardType)card); });
                            break;
                        case cardType.item:
                            button.onClick.AddListener(delegate { Research((cardType)card); });
                            break;
                        case cardType.unit:
                            button.onClick.AddListener(delegate { Research((cardType)card); });
                            break;
                    }
                    break;
            }
            if(gameDetails.Count > 0 && !details.combatPhase)
                setButtons(true);
        }
    }

    private void saveDetails(bool combatPhase)
    {;
        details.players = Game_Details_UI.players;
        details.currentPlayer = currentPlayer;
        details.combatPhase = combatPhase;
        gameDetails.Add(details);
    }

    private void initializeCard(int cardIndex, Vector3 location, cardType type, List<int> list, int? flipIndex)
    {
        GameObject objectFound = new GameObject();
        switch (type)
        {
            case cardType.building:
                objectFound = buildingObject;
                break;
            case cardType.item:
                objectFound = itemObject;
                break;
            case cardType.unit:
                objectFound = unitObject;
                break;
            case cardType.leader:
                objectFound = leaderObject;
                break;
        }
        if(flipIndex == null)
        {
            //Check for previous duplicates
            string cardName = objectFound.GetComponent<Card_Model>().faces[list[cardIndex]].name;
            GameObject parentObj = GameObject.Find("Canvas/Duplicates/" + cardName);
            if (parentObj == null)
            {
                parentObj = GameObject.Instantiate(new GameObject(), location, Quaternion.identity, duplicates.transform);
                parentObj.name = cardName;
            }
            GameObject obj = GameObject.Instantiate(objectFound, location, Quaternion.identity, parentObj.transform);
            obj.GetComponent<Image>().sprite = obj.GetComponent<Card_Model>().faces[list[cardIndex]];
            //Don't let users zoom in on the duplicates
            Destroy(obj.GetComponent<Card_Zoom>());
            Card_Model cardModel = obj.GetComponent<Card_Model>();
            if (type == cardType.unit)
            {
                obj.name = cardName + parentObj.transform.childCount;
                for(int i = parentObj.transform.childCount-1; i>0; i--)
                {
                    GameObject.Find("Canvas/Duplicates/" + cardName + "/" + cardName + i).transform.SetAsLastSibling();
                }
            }
            else
                obj.name = cardName;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(objectFound, location, Quaternion.identity, canvas.transform);
            FlipCard.cards[(int)flipIndex] = obj;
            Card_Model cardModel = obj.GetComponent<Card_Model>();
            CardFlipper flipper = obj.GetComponent<CardFlipper>();
            flipper.FlipCard(cardModel.cardBack, cardModel.faces[list[cardIndex]], list[cardIndex]);
            obj.name = cardModel.faces[list[cardIndex]].name;
        }
    }

    bool checkWorkers(Player player)
    {
        bool workerAvailable = false;
        foreach (Worker w in player.workers.Where(w => w != null))
        {
            if (w.TurnsLeft == 0)
            {
                w.IsActive = true;
                workerAvailable = true;
                break;
            }
        }
        return workerAvailable;
    }

    int cardDuplicateCheck(Vector3 placement, int n, int m, List<int> listType, cardType type, bool initialize)
    {
        //n is the type number while m is the total number (for flip count)
        //Use this only for the initial placement of all of the cards, after that just check the revealed card list
        if (m == 0)
            FlipCard.cards = new GameObject[12];

        float l = width / 67.5f;

        Vector3 location = new Vector3();
        bool repeat = true;
        while (repeat)
        {
            int num;
            if (m - n >= 0)
            {
                num = (m - n);
            }
            else
            {
                num = 0;
            }
            for (int i = num; i < FlipCard.cards.Length; i++)
            {
                GameObject o = FlipCard.cards[i];
                if (initialize && o != null)
                {
                    Card_Model cardModel = new Card_Model();
                    switch (type)
                    {
                        case cardType.unit:
                            cardModel = unitObject.GetComponent<Card_Model>();
                            break;
                        case cardType.building:
                            cardModel = buildingObject.GetComponent<Card_Model>();
                            break;
                        case cardType.item:
                            cardModel = itemObject.GetComponent<Card_Model>();
                            break;
                    }
                    if (o.name == cardModel.faces[listType[n]].name)
                    {
                        if (type == cardType.unit)
                        {
                            //There can be up to four one kind of unit so we need to check how many there are
                            GameObject parentObj = GameObject.Find("Canvas/Duplicates/" + cardModel.faces[listType[n]].name);
                            if (parentObj != null && parentObj.transform.childCount > 0)
                            {
                                switch (parentObj.transform.childCount)
                                {
                                    case 2:
                                        location = o.transform.position + new Vector3(l * 3, 0, 0);
                                        break;
                                    case 1:
                                        location = o.transform.position + new Vector3(l * 2, 0, 0);
                                        break;
                                }
                            }
                            else
                            {
                                location = o.transform.position + new Vector3(l, 0, 0);
                            }
                        }
                        else
                        {
                            location = o.transform.position + new Vector3(l, 0, 0);
                        }
                        initializeCard(n, location, type, listType, null);
                        o.transform.SetAsLastSibling();
                        n++;
                        repeat = true;
                        break;
                    }
                    else
                    {
                        location = placement;
                        repeat = false;
                    }
                }
                else if (!initialize && o == null)
                {
                    List<int> intList = new List<int>();
                    switch (type)
                    {
                        case cardType.unit:
                            intList = details.unitsRevealed;
                            break;
                        case cardType.item:
                            intList = details.itemsRevealed;
                            break;
                        case cardType.building:
                            intList = details.buildingsRevealed;
                            break;
                    }
                    foreach (int x in intList)
                    {
                        if (x == listType[n])
                        {
                            if (type == cardType.unit)
                            {
                                //There can be up to four one kind of unit so we need to check how many there are
                                GameObject parentObj = GameObject.Find("Canvas/Duplicates/" + unitObject.GetComponent<Card_Model>().faces[listType[n]].name);
                                o = GameObject.Find("Canvas/" + unitObject.GetComponent<Card_Model>().faces[listType[n]].name);
                                if (parentObj != null && parentObj.transform.childCount > 0)
                                {
                                    switch (parentObj.transform.childCount)
                                    {
                                        case 2:
                                            location = o.transform.position + new Vector3(l * 3, 0, 0);
                                            break;
                                        case 1:
                                            location = o.transform.position + new Vector3(l * 2, 0, 0);
                                            break;
                                    }
                                }
                                else
                                {
                                    location = o.transform.position + new Vector3(l, 0, 0);
                                }
                            }
                            else
                            {
                                switch (type)
                                {
                                    case cardType.building:
                                        o = GameObject.Find("Canvas/" + buildingObject.GetComponent<Card_Model>().faces[listType[n]].name);
                                        break;
                                    case cardType.item:
                                        o = GameObject.Find("Canvas/" + itemObject.GetComponent<Card_Model>().faces[listType[n]].name);
                                        break;
                                }
                                location = o.transform.position + new Vector3(l, 0, 0);
                            }
                            initializeCard(n, location, type, listType, null);
                            o.transform.SetAsLastSibling();
                            n++;
                            repeat = true;
                            break;
                        }
                        else
                        {
                            location = placement;
                            repeat = false;
                        }
                    }
                    break;
                }
                else
                {
                    location = placement;
                    repeat = false;
                    break;
                }
            }
        }
        initializeCard(n, location, type, listType, m);
        n++;
        return n;
    }

    public static void destroyAll(List<GameObject> l)
    {
        foreach(GameObject g in l)
        {
            Destroy(g);
        }
    }

    public static void unzoomCard(GameObject card)
    {
        Zoomed = false;
        moving = false;
        card.transform.position = Card_Zoom.startLocation;
        card.transform.localScale = Card_Zoom.startSize;
    }

    void changeGold(int change, Player player)
    {
        player.gold += change;
        goldText.GetComponent<Text>().text = String.Format("{0}", player.gold);
    }

    public static void changeVP(int change, Player player)
    {
        player.vp += change;

        //Check for second worker
        if(player.vp >= 4 && !player.earnedSecondWorker)
        {
            var ds = new DataService("Furtherance.db");
            //Add worker to player
            List<Worker> playerWorker = ds.GetWorker("Worker").ToList();
            player.workers[player.workers.Count(s => s != null)] = playerWorker[0];

        }

        //Check for victory and express victoriousness
        if(player.vp >= 6)
        {

        }

        goldText.GetComponent<Text>().text = String.Format("{0}", player.vp);
    }

    int[] checkForBuildingDiscounts(int cost, int time, bool researched)
    {
        int[] array = new int[2];

        foreach (Worker w in player.workers.Where(s => s != null))
        {
            if (w.Name == "Builder" && w.IsActive)
            {
                cost -= 1;
                time -= 1;
                break;
            }
        }

        foreach (Building b in player.buildings.Where(s => s != null))
        {
            if (b.Name == "Quarry")
            {
                time -= 1;
            }
        }

        if (researched)
            cost = checkForResearchDiscounts(cost);

        if (cost < 0)
            cost = 0;
        if (time < 0)
            time = 0;
        array[0] = cost;
        array[1] = time;
        return array;
    }

    int checkForItemDiscounts(int cost, bool researched)
    {
        foreach (Worker w in player.workers.Where(s => s != null && s.Name == "Merchant" && s.IsActive))
        {
            if (cost % 2 == 1)
            {
                cost++;
            }
            cost /= 2;
            break;
        }

        if (researched)
            cost = checkForResearchDiscounts(cost);

        if (cost < 0)
            cost = 0;

        return cost;
    }

    int checkForUnitDiscounts(int cost, bool researched)
    {
        foreach (Worker w in player.workers.Where(s => s != null && s.Name == "Recruiter" && s.IsActive))
        {
            cost -= 3;
            break;
        }
        foreach (Building b in player.buildings.Where(s => s != null && s.Name == "Barracks"))
        {
            cost -= 2;
        }

        if (researched)
            cost = checkForResearchDiscounts(cost);

        if (cost < 0)
            cost = 0;

        return cost;
    }

    int checkForResearchDiscounts(int cost)
    {
        foreach (Worker w in player.workers.Where(s => s != null && s.Name == "Scientist" && s.IsActive))
        {
            cost -= 1;
            break;
        }

        foreach (Building b in player.buildings.Where(s => s != null && s.Name == "Laboratory"))
        {
            cost -= 2;
        }

        return cost;
    }
    
    //Use the following to add a list of listeners to a lst of buttons
    public static void addListeners(List<Button> buttons, List<UnityAction> listeners)
    {
        for(int i = 0; i < buttons.Count(); i++)
        {
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(listeners[i]);
        }
    }
    #endregion

}
