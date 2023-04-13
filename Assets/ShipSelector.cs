using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    public int player = 1;
    public int shipIndex = 0;
    public int skillIndex = -1;
    public int phase = 0;
    public float accel = 0.5f;
    public ShipData ships;

    public GameObject shipSpawn;

    public GameObject ship=null;

    public TMPro.TextMeshProUGUI shipName;
    public TMPro.TextMeshProUGUI shipStats;
    public TMPro.TextMeshProUGUI shipSkill;
    public TMPro.TextMeshProUGUI selectPhase;
    public UnityEngine.UI.Slider accelSlider;

    public Dictionary<int, string> letterScores = new Dictionary<int, string>()
    {
        [0] = "E",
        [1] = "D",
        [2] = "C",
        [3] = "B",
        [4] = "A",
        [5] = "S",
        [6] = "X",
        [-1] = "F",
        [-2] = ":(",
    };

    CarController car;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool canSwitch = true;
    int letterMod
    {
        get
        {
            return skillIndex == -1 ? 1 : 0;
        }
    }
    bool changed = false;
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        if (h < -0.1f)
        {
            if (canSwitch)
            {
                if (phase==0)
                {
                    shipIndex -= 1;
                    changed = true;
                    if (shipIndex < 0)
                    {
                        shipIndex = ships.ships.Count - 1;
                    }
                }
                if (phase == 1)
                {
                    skillIndex -= 1;
                    if (skillIndex < -1)
                    {
                        skillIndex = ships.skillNames.Count - 1;
                    }
                }
            }
            if (phase==2)
            {
                accel = Mathf.Clamp(accel - Time.deltaTime, 0, 1);
            }
            canSwitch = false;
        } else if (h > 0.1f)
        {
            if (canSwitch)
            {
                if (phase == 0)
                {
                    shipIndex += 1;
                    changed = true;
                    if (shipIndex >= ships.ships.Count)
                    {
                        shipIndex = 0;
                    }
                }
                if (phase == 1)
                {
                    skillIndex += 1;
                    if (skillIndex >= ships.skillNames.Count)
                    {
                        skillIndex = -1;
                    }
                }
            }
            if (phase == 2)
            {
                accel = Mathf.Clamp(accel + Time.deltaTime, 0, 1);
            }
            canSwitch = false;
        } else
        {
            canSwitch = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            phase++;
            if (phase>2)
            {
                RaceManager.LoadGame();
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            phase--;
            if (phase<0)
            {
                phase = 0;
            }
        }

        PlayerPrefs.SetInt($"Player{player}Vehicle", shipIndex);
        PlayerPrefs.SetInt($"Player{player}Skill", skillIndex);
        PlayerPrefs.SetFloat($"Player{player}Accel", accel);

        car = ships.ships[shipIndex].GetComponent<CarController>();
        shipName.text = ships.shipNames[shipIndex];
        shipStats.text = "Speed: " + letterScores[car.speedStat+letterMod] + "\nHandling: " + letterScores[car.handlingStat + letterMod] + "\nBody: " + letterScores[car.bodyStat + letterMod];
        shipSkill.text = "Skill: " + (skillIndex == -1 ? "None" : ships.skillNames[skillIndex]);
        selectPhase.text = "Choose " + (new string[] { "Ship", "Skill", "Accel","DONE" })[phase];
        accelSlider.gameObject.SetActive(phase == 2);
        accelSlider.value = accel;
        

        if (phase>=2)
        {
            if (ship != null)
            {
                Destroy(ship);
            }
        }
        else if (ship == null || changed)
        {
            changed = false;
            if (ship!=null)
            {
                Destroy(ship);
            }
            ship = Instantiate(ships.ships[shipIndex], shipSpawn.transform.position, Quaternion.identity,parent:shipSpawn.transform);
            Destroy(ship.GetComponent<Rigidbody>());
            Destroy(ship.GetComponentInChildren<Camera>().gameObject);
            Destroy(ship.GetComponentInChildren<Canvas>().gameObject);
            ship.GetComponent<CarController>().enabled = false;
        }

        shipSpawn.transform.Rotate(0, 45*Time.deltaTime, 0);

    }
}
