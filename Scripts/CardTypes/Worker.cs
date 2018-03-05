using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker {

    //Sprite Image;
    public int Id { get; set; }
    public string Name { get; set; }
    public int TurnsLeft { get; set; }
    public bool IsActive { get; set; }
    public Building Building { get; set; }

    public Worker(int id, string name, int turnsLeft, bool isActive)
    {
        Id = id;
        Name = name;
        TurnsLeft = turnsLeft;
        IsActive = isActive;
    }
    public Worker()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
