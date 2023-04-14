using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Track Data", menuName = "Control Chase/Track Data", order = 1)]
public class TrackData : ScriptableObject
{
    public List<string> trackNames;
    public List<Sprite> trackImages;
    public List<string> trackDescriptions;
}
