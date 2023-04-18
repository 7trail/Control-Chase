using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI startText;
    public ShipData shipData;

    [Header("Track-Specific Data")]
    public List<GameObject> checkpoints = new List<GameObject>();
    public int laps = 3;

    public static RaceManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        int playerIndex = PlayerPrefs.GetInt("Player1Vehicle", 0);
        GameObject car = Instantiate(shipData.ships[playerIndex],Vector3.zero,Quaternion.identity);
        car.GetComponent<CarController>().canDrive = false;
        car.GetComponent<CarController>().skillStat = PlayerPrefs.GetInt("Player1Skill", -1);
        car.GetComponent<CarController>().accelStat = PlayerPrefs.GetFloat("Player1Accel", 0);
        StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(1);
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
            c.startTime = Time.time;
        }
        yield return new WaitForSeconds(2);
        startText.text = "";
    }

    IEnumerator EndRoutine(bool newRecord)
    {
        startText.text = newRecord ? "NEW RECORD!" : "END";
        yield return new WaitForSeconds(5);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }

    IEnumerator DeathRoutine()
    {
        startText.text = "FAIL";
        yield return new WaitForSeconds(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static string Track = "Gilded Cup 1";
    public static void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Track);
    }

    public void FinishGame(bool b)
    {
        StartCoroutine(EndRoutine(b));
    }

    public void FinishWithDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    public static bool SubmitFinalTime(float time)
    {
        float f = PlayerPrefs.GetFloat("Record " + Track,0.0f);
        if (time>f)
        {
            PlayerPrefs.SetFloat("Record " + Track, time);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
}
