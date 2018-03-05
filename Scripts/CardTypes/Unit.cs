using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    public int Id {get; set;}
    public string Name { get; set; }
    public int Hp { get; set; }
    public int? Damage { get; set; }
    public int Atk { get; set; }
    public int Cost { get; set; }
    public int Movement { get; set; }
    public bool CanMoveAfterAttack { get; set; }
    public int Attacks { get; set; }
    public bool CounterAttacks { get; set; }
    public int? XLocation { get; set; }
    public int? YLocation { get; set; }
    public GameObject Card { get; set; }
    public int movesRemaining { get; set; }
    public int attacksRemaining { get; set; }
    public bool justPlayed { get; set; }

    public Unit(string name, int hp, int? damage, int atk, int cost, int movement, bool canMoveAfterAttack, int attacks, bool counterAttacks, int? xLocation, int? yLocation)
    {
        //Image = image;
        Name = name;
        Hp = hp;
        if(damage != null)
            Damage = damage;
        Atk = atk;
        Cost = cost;
        Movement = movement;
        CanMoveAfterAttack= canMoveAfterAttack;
        Attacks = attacks;
        CounterAttacks= counterAttacks;
        if(xLocation != null)
            XLocation = xLocation;
        if(yLocation != null)
            YLocation = yLocation;
    }

    public Unit()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
