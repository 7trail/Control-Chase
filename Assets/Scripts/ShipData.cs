using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship Data", menuName = "Control Chase/Ship Data", order = 1)]
public class ShipData : ScriptableObject
{
    public List<string> shipNames;
    public List<string> shipDescriptions;
    public List<GameObject> ships;
    public List<string> skillNames;
}
