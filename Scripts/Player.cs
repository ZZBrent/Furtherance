using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    public string name = "";
    public int order = new int();
    public int gold = 4;
    public int hp = 10;
    public int vp = 0;
    public Leader leader;
    public Unit[] units = new Unit[6];
    public Building[] buildings = new Building[7];
    public Item[] items = new Item[5];
    public Worker[] workers = new Worker[4];
    public object[] research = new object[2];
    public bool earnedSecondWorker = false;
    public bool defeated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}