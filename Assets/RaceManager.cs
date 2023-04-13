using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI startText;
    public List<GameObject> vehicles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        int playerIndex = PlayerPrefs.GetInt("Player1Vehicle", 0);
        GameObject car = Instantiate(vehicles[playerIndex],Vector3.zero,Quaternion.identity);
        car.GetComponent<CarController>().canDrive = false;
        StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(2);
        startText.text = "3";
        yield return new WaitForSeconds(1);
        startText.text = "2";
        yield return new WaitForSeconds(1);
        startText.text = "1";
        yield return new WaitForSeconds(1);
        startText.text = "DO IT!";
        foreach(CarController c in FindObjectsOfType<CarController>())
        {
            c.canDrive = true;
        }
        yield return new WaitForSeconds(2);
        startText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
