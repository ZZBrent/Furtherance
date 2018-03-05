using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Movement : MonoBehaviour {

    #region Variables
    //Keep track of the current unit selected
    public static Unit unit;
    //Find where other units are so you know to attack those coordinates
    public static List<coordinates> coordinatesToAttack = new List<coordinates>();
    //This is a list of all of the coordinates that have already been mapped to
    public static List<coordinates> coordinatesSpotted = new List<coordinates>();
    //Keep track of spaces that can be selected (so that they can be deleted after one is selected)
    public static List<GameObject> selections = new List<GameObject>();
    //Keep track of when users can move units and when they can't by pausing and unpausing movement
    public static bool paused;
    //Keep track of if the player used a unit this turn
    public static bool unitUsed;

    //Track the following so that buttons cannot be clicked or events fired more than once
    public static bool countered;
    public static bool attackFinished;
    public static bool unitAddedToResearch;
    public static bool unitKilled;
    public static bool castleOrBuilding;
    public static bool buildingSelection;
    public static bool castleDamaged;
    public static bool unitAttacked;
    public static bool castleAttacked;
    public static bool attackStarted;
    public static bool defenseStarted;
    public static bool castleDefended;
    GameObject currentUnit;

    public enum selectionType
    {
        attack,
        move,
        item
    }

    public struct coordinates
    {
        public int x;
        public int y;
    }
    #endregion

    #region Allow and Restrict Movement
    public void allowMovement (Unit u)
    {
        paused = false;
        u.movesRemaining = u.Movement;
        u.attacksRemaining = u.Attacks;
        unit = u;
    }

    public void restrictMovement()
    {
        paused = true;
        unit.movesRemaining = 0;
        unit.attacksRemaining = 0;
        destroySpots();
    }
    #endregion

    public void OnPointerClick()
    {
        if(Card_Zoom.zoomedCard != null)
            MainPresenter.unzoomCard(Card_Zoom.zoomedCard);

        castleOrBuilding = false;
        buildingSelection = false;
        castleDamaged = false;
        defenseStarted = false;
        castleDefended = false;
        attackStarted = false;
        countered = false;
        attackFinished = false;
        unitAddedToResearch = false;
        unitKilled = false;
        unitAttacked = false;
        castleAttacked = false;

        //Check that unit is not already moving
        if (selections.Count > 0)
        {
            MainPresenter.unitDetails.SetActive(false);
            destroySpots();
            return;
        }

        bool enemyUnit = false;
        //Find the unit clicked by the object
        foreach (Unit u in MainPresenter.player.units.Where(n => n != null))
        {
            if (u.Card == this.gameObject)
            {
                if(unit == u && MainPresenter.unitDetails.activeSelf)
                {
                    MainPresenter.unitDetails.SetActive(false);
                    return;
                }
                unit = u;
                break;
            }
            enemyUnit = true;
        }

        //Find all units on the board so we know spots to remove
        coordinatesToAttack = new List<coordinates>();
        coordinatesSpotted = new List<coordinates>();

        int i = 0;
        foreach (Player p in Game_Details_UI.players.Where(p => p != MainPresenter.player))
        {
            foreach (Unit u in p.units.Where(s => s != null && Math.Abs((int)s.XLocation) <= 2 && Math.Abs((int)s.YLocation) <= 2))
            {
                coordinates c = new coordinates();
                c.x = (int)u.XLocation;
                c.y = (int)u.YLocation;
                coordinatesToAttack.Add(c);
                if (enemyUnit && u.Card == gameObject)
                {
                    if (unit == u && MainPresenter.unitDetails.activeSelf)
                    {
                        MainPresenter.unitDetails.SetActive(false);
                        return;
                    }
                    unit = u;
                }
            }
            i++;
        }

        //Add castle coordinates
        for(int m = 0; m < 3; m++)
        {
            for(int n = 0; n < Game_Details_UI.players.Where(s => s != null).Count(); n++)
            {
                coordinates c = new coordinates();
                if (MainPresenter.currentPlayer != n)
                {
                    switch (n)
                    {
                        case 0:
                            c.x = -1 + m;
                            c.y = -3;
                            break;
                        case 1:
                            c.x = -3;
                            c.y = -1 + m;
                            break;
                        case 2:
                            c.x = -1 + m;
                            c.y = 3;
                            break;
                        case 3:
                            c.x = 3;
                            c.y = -1 + m;
                            break;
                    }
                    coordinatesToAttack.Add(c);
                }
            }
        }

        if (!paused)
        {
            Transform detailsTransform = MainPresenter.unitDetails.transform;
            MainPresenter.unitDetails.SetActive(true);
            MainPresenter.unitDetails.transform.SetAsLastSibling();
            if(currentUnit != null)
            {
                Destroy(currentUnit);
            }
            currentUnit = Instantiate(unit.Card, detailsTransform.position, new Quaternion(), detailsTransform);
            Destroy(currentUnit.GetComponent<Movement>());
            Card_Zoom zoom = currentUnit.AddComponent<Card_Zoom>();
            EventTrigger trigger = currentUnit.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(delegate { zoom.OnPointerClick(); });
            trigger.triggers.Add(entry);
            currentUnit.transform.localScale = new Vector3(2.75f, 2.75f, 1);
            if (enemyUnit)
                return;
        }

        if ((unit.movesRemaining == 0 && unit.attacksRemaining == 0) || paused || unit.justPlayed)
            return;

        coordinates co = new coordinates();
        co.x = (int)unit.XLocation;
        co.y = (int)unit.YLocation;
        changeOrientation(co, 1);
    }

    public static void destroySpots()
    {
        foreach (GameObject o in selections)
        {
            Destroy(o);
        }
        selections = new List<GameObject>();

        //Spots being destroyed means that the unit is no longer being focused on
        MainPresenter.unitDetails.SetActive(false);
    }

    void changeOrientation(coordinates c, int moves)
    {
        coordinates originalCoordinates = c;

        c = convertCoordinates(c, MainPresenter.currentPlayer);

        checkAdjacentSpaces(c, moves, originalCoordinates, true);
    }

    public static coordinates convertCoordinates(coordinates c, int playerIndex)
    {
        //Change current coordinates according to the way board is flipped (which is based on current player from main presenter)
        int x;
        int y;
        switch (playerIndex)
        {
            case 0:
                break;
            case 1:
                x = c.x;
                y = c.y;
                c.x = y;
                c.y = x;
                c.x *= -1;
                break;
            case 2:
                c.y *= -1;
                c.x *= -1;
                break;
            case 3:
                x = c.x;
                y = c.y;
                c.x = y;
                c.y = x;
                c.y *= -1;
                break;
        }
        return c;
    }

    public static void checkAdjacentSpaces(coordinates c, int moves, coordinates o, bool adjacentAttack)
    {
        //check top spot, but only if it is adjacent
        if(!((c.y == 1 && c.x != 0) || c.y == 3 || (c.y == -2 && c.x != 0)))
        {
            coordinates cSpot = new coordinates();
            cSpot.x = c.x;
            cSpot.y = c.y + 1;
            coordinates cOriginal = new coordinates();
            switch (MainPresenter.currentPlayer)
            {
                case 0:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y + 1;
                    break;
                case 1:
                    cOriginal.x = o.x + 1;
                    cOriginal.y = o.y;
                    break;
                case 2:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y - 1;
                    break;
                case 3:
                    cOriginal.x = o.x - 1;
                    cOriginal.y = o.y;
                    break;
            }
            spotCheck(cSpot, moves, cOriginal, adjacentAttack);
        }

        //check bottom spot, but only if it is adjacent
        if (!((c.y == -1 && c.x != 0) || c.y == -3 || (c.y == 2 && c.x != 0)))
        {
            coordinates cSpot = new coordinates();
            cSpot.x = c.x;
            cSpot.y = c.y - 1;
            coordinates cOriginal = new coordinates();
            switch (MainPresenter.currentPlayer)
            {
                case 0:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y - 1;
                    break;
                case 1:
                    cOriginal.x = o.x - 1;
                    cOriginal.y = o.y;
                    break;
                case 2:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y + 1;
                    break;
                case 3:
                    cOriginal.x = o.x + 1;
                    cOriginal.y = o.y;
                    break;
            }
            spotCheck(cSpot, moves, cOriginal, adjacentAttack);
        }

        //check right spot, but only if it is adjacent
        if (!((c.x == 1 && c.y != 0) || c.x == 3 || (c.x == -2 && c.y != 0)))
        {
            coordinates cSpot = new coordinates();
            cSpot.x = c.x + 1;
            cSpot.y = c.y;
            coordinates cOriginal = new coordinates();
            switch (MainPresenter.currentPlayer)
            {
                case 0:
                    cOriginal.x = o.x + 1;
                    cOriginal.y = o.y;
                    break;
                case 1:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y - 1;
                    break;
                case 2:
                    cOriginal.x = o.x - 1;
                    cOriginal.y = o.y;
                    break;
                case 3:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y + 1;
                    break;
            }
            spotCheck(cSpot, moves, cOriginal, adjacentAttack);
        }

        //check left spot, but only if it is adjacent
        if (!((c.x == -1 && c.y != 0) || c.x == -3 || (c.x == 2 && c.y != 0)))
        {
            coordinates cSpot = new coordinates();
            cSpot.x = c.x - 1;
            cSpot.y = c.y;
            coordinates cOriginal = new coordinates();
            switch (MainPresenter.currentPlayer)
            {
                case 0:
                    cOriginal.x = o.x - 1;
                    cOriginal.y = o.y;
                    break;
                case 1:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y + 1;
                    break;
                case 2:
                    cOriginal.x = o.x + 1;
                    cOriginal.y = o.y;
                    break;
                case 3:
                    cOriginal.x = o.x;
                    cOriginal.y = o.y - 1;
                    break;
            }
            spotCheck(cSpot, moves, cOriginal, adjacentAttack);
        }
    }

    public static void spotCheck(coordinates c, int moves, coordinates o, bool adjacentAttack)
    {
        //Check if already in list of spots
        foreach(coordinates cs in coordinatesSpotted)
        {
            if(cs.x == c.x && cs.y == c.y)
            {
                return;
            }
        }

        //Check if that is the current unit's spot, if so skip it
        coordinates unitLocation = new coordinates();
        unitLocation.x = (int)unit.XLocation;
        unitLocation.y = (int)unit.YLocation;
        unitLocation = convertCoordinates(unitLocation, MainPresenter.currentPlayer);
        if (c.x == unitLocation.x && c.y == unitLocation.y)
        {
            return;
        }
        
        foreach (coordinates cToAvoid in coordinatesToAttack)
        {
            coordinates attack = convertCoordinates(cToAvoid, MainPresenter.currentPlayer);
            if (attack.x == c.x && attack.y == c.y)
            {
                //Check if unit is out of attacks and if enemy is adjacent
                if(unit.attacksRemaining != 0 && unit.Name == "archer")
                {
                    if(moves == 1)
                    {
                        makeSpace(c, selectionType.attack, moves, o, null);
                    }
                    else if(moves == 2)
                    {
                        makeSpace(c, selectionType.attack, moves, o, null);
                    }
                }
                else if (unit.attacksRemaining != 0 && moves == 1)
                    makeSpace(c, selectionType.attack, moves, o, null);
                return;
            }
        }

        if (unit.attacksRemaining != 0 && unit.Name == "archer" && moves < 2)
        {
            checkAdjacentSpaces(c, 2, o, false);
        }

        //Check if unit is out of moves and within their personal movement range (units cannot move into other player's castles)
        if (unit.movesRemaining != 0 && (Math.Abs(c.x) <= 2 && c.y <= 2) && adjacentAttack)
        {
            makeSpace(c, selectionType.move, moves, o, null);
        }
    }

    public static void makeSpace(coordinates c, selectionType type, int moves, coordinates o, UnityAction method)
    {
        float width = Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.width / 10);
        float height = Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.height / 10);

        //When setting selections make sure to add or subtract width / 15.68f and height / 8.8138f
        GameObject selection = GameObject.Instantiate(MainPresenter.highlightedSpace, new Vector3(width / 4.1925f +  c.x*(width / 15.68f), height / 1.7348f + c.y * (height / 8.8138f), 0), Quaternion.identity, MainPresenter.canvas.transform);
        Button button = selection.GetComponent<Button>();
        selections.Add(selection);
        coordinatesSpotted.Add(c);
        if(type == selectionType.move)
        {
            selection.GetComponent<Image>().color = new Color(0, 1, 0, 0.49f);
            button.onClick.AddListener(delegate { moveUnit(moves, c, o); });
            if(moves < unit.movesRemaining)
            {
                checkAdjacentSpaces(c, moves + 1, o, true);
            }
        }
        else if(type == selectionType.attack)
        {
            selection.GetComponent<Image>().color = new Color(1, 0, 0, 0.49f);
            button.onClick.AddListener(delegate { attackUnit(o); });
        }
        else
        {
            selection.GetComponent<Image>().color = new Color(1, 1, 0, 0.49f);
            button.onClick.AddListener(method);
        }
    }

    public static void moveUnit(int moves, coordinates newPosition, coordinates newCoordinates)
    {
        unitUsed = true;

        float width = Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.width / 10);
        float height = Convert.ToSingle(Camera.main.orthographicSize * 2.0 * Screen.height / 10);

        unit.Card.transform.position = new Vector3(width / 4.1925f + newPosition.x*(width / 15.68f), height / 1.7348f + newPosition.y*(width / 15.68f), 0);
        unit.movesRemaining -= moves;

        foreach(Unit u in Game_Details_UI.players[MainPresenter.currentPlayer].units.Where(s => s != null))
        {
            if(u.XLocation == unit.XLocation && u.YLocation == unit.YLocation)
            {
                u.XLocation = newCoordinates.x;
                u.YLocation = newCoordinates.y;
                break;
            }
        }

        unit.XLocation = newCoordinates.x;
        unit.YLocation = newCoordinates.y;

        destroySpots();

        if(unit.Name == "Arsonist" && (Math.Abs(newPosition.x) > 2 || newPosition.y > 2))
        {
            paused = true;
            Button[] buttons = MainPresenter.showButtons(String.Format("{0}", MainPresenter.player.name), "Would you like to destroy a building?", "Yes", "No");
            List<UnityAction> listeners = new List<UnityAction>();
            listeners.Add(delegate { selectBuilding(MainPresenter.player, null, true); });
            listeners.Add(delegate { finishAttack(unit, MainPresenter.player); });
            MainPresenter.addListeners(buttons.ToList(), listeners);
        }
    }

    public static void attackUnit(coordinates c)
    {
        unitUsed = true;

        if (attackStarted)
            return;

        attackStarted = true;

        if (!unit.CanMoveAfterAttack)
            unit.movesRemaining = 0;

        unit.attacksRemaining -= 1;

        //Check if the click is in a castle
        bool castleClicked = false;
        if(Math.Abs(c.x) > 2 || Math.Abs(c.y) > 2)
        {
            castleClicked = true;
        }

        //Find which unit is in the space being clicked
        bool unitClicked = false;
        Unit unitAttacked = new Unit();
        Player player = new Player();
        int i = 0;
        foreach (Player p in Game_Details_UI.players)
        {
            if (unitClicked)
                break;
            foreach (Unit u in p.units.Where(s => s != null))
            {
                if(c.x == u.XLocation && c.y == u.YLocation)
                {
                    unitClicked = true;
                    unitAttacked = u;
                    player = p;
                    break;
                }
            }
            i++;
        }
        destroySpots();

        if(castleClicked)
        {
            if (c.x < -2)
            {
                player = Game_Details_UI.players[1];
            }
            else if (Math.Abs(c.x) < 2)
            {
                player = Game_Details_UI.players[2];
            }
            else
            {
                player = Game_Details_UI.players[3];
            }
        }

        if(unitClicked && !castleClicked)
            attackUnit(unitAttacked, player);
        else if(!unitClicked && castleClicked)
            attackCastle(player);
        else
        {
            Button[] buttons = MainPresenter.showButtons(String.Format("{0}", MainPresenter.player.name), String.Format("Attack the {0} or the castle?", unitAttacked.Name), "Unit", "Castle");
            List<UnityAction> listeners = new List<UnityAction>();
            listeners.Add(delegate { attackUnit(unitAttacked, player); });
            listeners.Add(delegate { attackCastle(player); });
            MainPresenter.addListeners(buttons.ToList(), listeners);
        }
    }

    public static void attackUnit(Unit u, Player p)
    {
        if (unitAttacked)
            return;

        unitAttacked = true;

        u.Damage += unit.Atk;
        if (u.Atk > 0 && unit.CounterAttacks && u.CounterAttacks)
        {
            Button[] buttons = MainPresenter.showButtons(String.Format("{0}", p.name), "Would you like to counter-attack?", "Yes", "No");
            List<UnityAction> listeners = new List<UnityAction>();
            listeners.Add(delegate { counterAttack(u, p); });
            listeners.Add(delegate { finishAttack(u, p); });
            MainPresenter.addListeners(buttons.ToList(), listeners);
        }
        else
        {
            paused = true;
            finishAttack(u, p);
        }
    }

    public static void attackCastle(Player p)
    {
        if (castleAttacked)
            return;

        castleAttacked = true;

        List<Unit> defenseList = new List<Unit>();
        foreach(Unit u in p.units.Where(s => s != null))
        {
            if(Math.Abs((int)u.XLocation) > 2 || Math.Abs((int)u.YLocation) > 2)
            {
                defenseList.Add(u);
            }
        }

        //Check for walls or fortification
        List<Building> attackRequiredBuildings = new List<Building>();
        foreach (Building b in p.buildings.Where(s => s != null && (s.Name == "Walls" || s.Name == "Fortification")))
        {
            attackRequiredBuildings.Add(b);
        }

        if (defenseList.Count == 0)
        {
            if (attackRequiredBuildings.Count == 1)
                damageCastleOrBuilding(p, attackRequiredBuildings[0], false);
            else if (attackRequiredBuildings.Count > 1)
                selectBuilding(p, attackRequiredBuildings, false);
            else if (p.buildings.Where(s => s != null).Count() > 0)
                attackCastleOrBuilding(p);
            else
                damageCastleOrBuilding(p, null, false);
        }
        else
        {
            Button[] buttons = MainPresenter.showButtons(String.Format("{0}", p.name), "Would you like to defend your castle with a unit?", "Yes", "No");
            List<UnityAction> listeners = new List<UnityAction>();
            listeners.Add(delegate { defendCastle(defenseList, p); });

            if (p.buildings.Where(s => s != null).Count() > 0)
            {
                if(attackRequiredBuildings.Count == 0)
                    listeners.Add(delegate { attackCastleOrBuilding(p); });
                else if (attackRequiredBuildings.Count == 1)
                    listeners.Add(delegate { damageCastleOrBuilding(p, attackRequiredBuildings[0], false); });
                else
                    listeners.Add(delegate { attackCastleOrBuilding(p); });
            }
            else
                listeners.Add(delegate { damageCastleOrBuilding(p, null, false); });
            MainPresenter.addListeners(buttons.ToList(), listeners);
            paused = true;
        }
    }

    public static void attackCastleOrBuilding(Player p)
    {
        if (castleOrBuilding)
            return;

        castleOrBuilding = true;

        Button[] buttons = MainPresenter.showButtons(String.Format("{0}", MainPresenter.player.name), "Would you like to attack a building?", "Yes", "No");
        List<UnityAction> listeners = new List<UnityAction>();
        listeners.Add(delegate { selectBuilding(p, null, false); });
        listeners.Add(delegate { damageCastleOrBuilding(p, null, false); });
        MainPresenter.addListeners(buttons.ToList(), listeners);
        paused = true;
    }

    public static void selectBuilding(Player p, List<Building> requiredBuildings, bool destroyBuilding)
    {
        if (buildingSelection)
            return;

        buildingSelection = true;

        if (p.buildings.Where(n => n != null).Count() == 1)
        {
            damageCastleOrBuilding(p, p.buildings[0], destroyBuilding);
        }
        else
        {
            List<GameObject> cards = new List<GameObject>();
            List<int> intList = new List<int>();
            List<Building> buildings = new List<Building>();
            if(requiredBuildings == null)
            {
                buildings = p.buildings.Where(s => s != null).ToList();
            }
            else
            {
                buildings = requiredBuildings;
            }
            for (int i = 0; i < requiredBuildings.Count(); i++)
            {
                GameObject card = GameObject.Instantiate(MainPresenter.buildingObject, new Vector3(), Quaternion.identity, MainPresenter.cardChoice.transform);
                cards.Add(card);
            }
            List<Vector3> positions = MainPresenter.showCards("Attack!", cards);
            int n = 0;
            foreach (GameObject card in cards)
            {
                card.transform.position = positions[n];
                GameObject defendButtonObj = GameObject.Instantiate(MainPresenter.defendButton, card.transform.position - new Vector3(0, (110f / 760f) * MainPresenter.height), Quaternion.identity, card.transform);
                defendButtonObj.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
                defendButtonObj.GetComponent<Button>().onClick.AddListener(delegate { damageCastleOrBuilding(p, buildings[n], destroyBuilding); });
                n++;
            }
        }
    }

    public static void damageCastleOrBuilding(Player p, Building b, bool destroy)
    {
        if (castleDamaged)
            return;

        castleDamaged = true;

        if (b == null)
            p.hp -= 1;
        else
            b.Damage += 1;

        if(unit.CounterAttacks)
        {
            unit.Damage += 1;
            if(b != null && unit.Damage < unit.Hp && b.Name == "Guard Tower" && b.TurnAbilityCount > 0)
            {
                unit.Damage += 1;
                b.TurnAbilityCount -= 1;
            }
        }
        paused = true;
        Unit castle = new Unit();
        if (b != null && b.Damage >= b.Time)
            addToResearch(b, p, null);
        finishAttack(castle, p);
    }

    public static void defendCastle(List<Unit> defenseList, Player p)
    {
        if (defenseStarted)
            return;

        defenseStarted = true;

        if(defenseList.Count == 1)
        {
            unitDefense(defenseList[0], p);
        }
        else
        {
            List<GameObject> cards = new List<GameObject>();
            List<int> intList = new List<int>();
            for (int i = 0; i < defenseList.Count; i++)
            {
                GameObject card = GameObject.Instantiate(defenseList[i].Card, new Vector3(), Quaternion.identity, MainPresenter.cardChoice.transform);
                card.GetComponent<RectTransform>().localScale += new Vector3(0.67f, 0.67f, 0);
                cards.Add(card);
            }
            List<Vector3> positions = MainPresenter.showCards("Defend!", cards);
            int n = 0;
            foreach (GameObject card in cards)
            {
                card.transform.position = positions[n];
                GameObject defendButtonObj = GameObject.Instantiate(MainPresenter.defendButton, card.transform.position - new Vector3(0, (110f / 760f) * MainPresenter.height), Quaternion.identity, card.transform);
                defendButtonObj.GetComponent<RectTransform>().localScale -= new Vector3(0.67f, 0.67f, 0);
                defendButtonObj.GetComponent<Button>().onClick.AddListener(delegate { unitDefense(defenseList[n], p); });
                n++;
            }
        }
    }

    public static void unitDefense(Unit u, Player p)
    {
        if (castleDefended)
            return;

        castleDefended = true;

        attackUnit(u, p);
    }

    public static void counterAttack(Unit u, Player p)
    {
        if (countered)
            return;

        countered = true;

        MainPresenter.hideNotification();

        unit.Damage += u.Atk;
        finishAttack(u, p);
    }

    public static void finishAttack(Unit u, Player p)
    {
        if (attackFinished)
            return;

        attackFinished = true;

        MainPresenter.hideNotification();

        paused = false;

        //Check if either unit died.  If so, destroy their game object and remove them from their player's units
        //TODO: Add the option to pay two coins to add the unit to research
        if (unit != null && unit.Damage >= unit.Hp)
        {
            askToResearchUnit(unit, Game_Details_UI.players[MainPresenter.currentPlayer], p);
        }
        else if(u != null && u.Damage >= u.Hp)
        {
            askToResearchUnit(u, p, null);
        }
        else if (u == null && p.hp <= 0)
        {
            destroyCastle(p);
        }

        return;
    }

    public static void askToResearchUnit(Unit u, Player p, Player p2)
    {
        if (p.gold >= 2)
        {
            Button[] buttons = MainPresenter.showButtons(String.Format("{0}", p.name), String.Format("Would you like to pay 2 gold to add the {0} to your research?", u.Name), "Yes", "No");
            List<UnityAction> listeners = new List<UnityAction>();
            listeners.Add(delegate { addToResearch(u, p, p2); });
            listeners.Add(delegate { killUnit(u, p, p2); });
            MainPresenter.addListeners(buttons.ToList(), listeners);
            paused = true;
        }
        else
        {
            killUnit(u, p, p2);
        }
    }

    public static void addToResearch(object o, Player p, Player p2)
    {
        if (unitAddedToResearch)
            return;

        unitAddedToResearch = true;

        p.gold -= 2;
        if(p.research.Count(s => s != null) == 2)
            MainPresenter.removeOneFromResearch();

        if(o.GetType() == typeof(Unit))
        {
            Unit u = (Unit) o;
            p.research[p.research.Count(s => s != null)] = u;
            killUnit(u, p, p2);
        }
        else
        {
            Building b = (Building)o;
            p.research[p.research.Count(s => s != null)] = b;
            destroyBuilding(p, b);
        }
    }

    public static void killUnit(Unit u, Player p, Player p2)
    {
        if (unitKilled)
            return;

        unitKilled = true;

        MainPresenter.hideNotification();

        if(u.Name == "Civilians")
        {
            MainPresenter.changeVP(-1, p);
        }

        Destroy(u.Card);
        for(int i = 0; i < p.units.Count(s => s != null); i++)
        {
            if(p.units[i].XLocation == u.XLocation && p.units[i].YLocation == u.YLocation)
            {
                //Move all other units down one, if any exist.  Then set final to null.
                for(int n = i; n < p.units.Count(s => s != null) - 1; n++)
                {
                    p.units[n] = p.units[n + 1];
                }
                p.units[p.units.Count(s => s != null) - 1] = null;
            }
        }
        paused = false;
        if(p2 != null)
        {
            if (u.Damage > u.Hp)
            {
                unitAddedToResearch = false;
                askToResearchUnit(u, p2, null);
                return;
            }
        }
        return;
    }

    static void destroyCastle(Player p)
    {
        foreach(Unit u in p.units.Where(s => s != null))
        {
            Destroy(u.Card);
        }
        p.defeated = true;

        MainPresenter.changeVP(2, MainPresenter.player);

        var ds = new DataService("Furtherance.db");
        List<Worker> getWorker = ds.GetWorker("Worker").ToList();
        MainPresenter.player.workers[MainPresenter.player.workers.Count()] = getWorker[0];
        MainPresenter.player.workers[MainPresenter.player.workers.Where(s => s != null).Count()] = getWorker[0];
    }

    static void destroyBuilding(Player p, Building b)
    {
        bool buildingDestroyed = false;
        for(int n = 0; n < p.buildings.Where(s => s != null).Count(); n++)
        {
            if(buildingDestroyed)
            {
                if (n < p.buildings.Where(s => s != null).Count() - 1)
                    p.buildings[n] = p.buildings[n + 1];
                else
                    p.buildings[n] = null;
            }
            else if(p.buildings[n] == b)
            {
                if (n < p.buildings.Where(s => s != null).Count() - 1)
                    p.buildings[n] = p.buildings[n+1];
                else
                    p.buildings[n] = null;
                buildingDestroyed = true;
            }
        }
        List<Building> bToDestroy = new List<Building>();
        foreach(Building bg in p.buildings.Where(s => s != null && s.Name == "Fortification"))
        {
            bg.Time = p.buildings.Where(s => s != null).Count();
            if(bg.Damage >= bg.Time)
            {
                bToDestroy.Add(bg);
            }
        }
        for(int i = 0; i < bToDestroy.Count(); i++)
        {
            destroyBuilding(p, bToDestroy[i]);
        }
    }
}