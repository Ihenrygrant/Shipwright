using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInfo : MonoBehaviour {
    public Factions shipFaction = Factions.Default;
    public List<GameObject> turrets = new List<GameObject>();
}

public enum Factions{
    Player,
    Neutral,
    Enemy,
    Default
 }