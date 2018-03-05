using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader
{ 
    public int Id { get; set; }
    public string Name { get; set; }
    public Leader(string name)
    {
        Name = name;
    }
    public Leader()
    {
    }
}