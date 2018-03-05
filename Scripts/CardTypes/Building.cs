using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
    public int Damage { get; set; }
    public int Time { get; set; }
    public int? TurnAbilityCount { get; set; }

    public Building(int id, string name, int cost, int damage, int time)
    {
        Id = id;
        Name = name;
        Cost = cost;
        Damage = damage;
        Time = time;
    }

    public Building()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
