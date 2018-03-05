using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    
    public int Id { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
    public bool UseAnytime { get; set; }

    public Item(int id, string name, int cost, bool useAnytime)
    {
        //Image = Image;
        Id = Id;
        Name = name;
        Cost = cost;
        UseAnytime = useAnytime;
    }

    public Item ()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
