using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDetails {

    public Player[] players = new Player[4];
    public List<int> buildingsRevealed = new List<int>();
    public List<int> itemsRevealed = new List<int>();
    public List<int> unitsRevealed = new List<int>();
    public List<int> buildingsDeck = new List<int>();
    public List<int> itemsDeck = new List<int>();
    public List<int> unitsDeck = new List<int>();
    public List<int> workersDeck = new List<int>();
    public bool combatPhase = false;
    public int currentPlayer = 0;

    public GameDetails()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
