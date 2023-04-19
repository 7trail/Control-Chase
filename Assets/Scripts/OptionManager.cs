using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public static List<Option> options = new List<Option>()
    {
        new Option()
        {
            displayName= "Randomizer",
            identitifer="R?",
            description = "Randomize terrain types?"
        },
        new Option()
        {
            displayName= "No Walls?",
            identitifer="NW",
            description = "Remove all walls from tracks?"
        },
        new Option()
        {
            displayName= "Ultraspeed",
            identitifer="US",
            description = "Make boosts WAY faster?"
        },
        new Option()
        {
            displayName= "Boostless",
            identitifer="NB",
            description = "Removes all speed-based terrain from the track."
        }
    };
    public static bool IsEnabledOption(string o)
    {
        List<Option> op = options.Where(z => z.displayName == o).ToList();
        return op.Count > 0 && op[0].enabled;
        //return PlayerPrefs.GetInt("ActiveOption-"+o, 0) != 0;
    }
    public static string GetActiveOptions(string delim = "*BRICK")
    {
        string str = "";
        foreach(Option o in options)
        {
            if (o.enabled)
            {
                str += o.identitifer+delim;
            }
        }
        return str;
    }


    public List<Material> materialOptionsRandomizer = new List<Material>();
    public List<Material> materialOptionsBoostless = new List<Material>();


    // Start is called before the first frame update
    void Start()
    {
        if (IsEnabledOption("No Walls?"))
        {
            foreach(GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.ToLower().Contains("walls"))
                {
                    obj.SetActive(false);
                }
            }
        }

        if (IsEnabledOption("Randomizer"))
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.ToLower().Contains("track"))
                {
                    for(int i = 0; i < obj.GetComponent<MeshRenderer>().materials.Length; i++)
                    {
                        obj.GetComponent<MeshRenderer>().materials[i] = materialOptionsRandomizer[Random.Range(0, materialOptionsRandomizer.Count)];
                    }
                }
            }
        }

        if (IsEnabledOption("Boostless"))
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.name.ToLower().Contains("track"))
                {
                    for (int i = 0; i < obj.GetComponent<MeshRenderer>().materials.Length; i++)
                    {
                        if (!materialOptionsBoostless.Contains(obj.GetComponent<MeshRenderer>().materials[i]))
                            obj.GetComponent<MeshRenderer>().materials[i] = materialOptionsRandomizer[0];
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public class Option
{
    public string displayName = "Name";
    public string identitifer = "0";
    public string description = "Description";
    public bool enabled = false;
}