using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    public int phase = 0;
    public int stageIndex = 0;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI trackNameText;
    public TextMeshProUGUI trackDescText;
    public Image trackImg;
    public GameObject settingsMenu;
    public GameObject raceMenu;

    public TrackData trackData;

    // Start is called before the first frame update
    void Start()
    {
        stageIndex = trackData.trackNames.IndexOf(RaceManager.Track);
        SetStage();
    }
    bool canCycle = true;

    public void SetStage()
    {
        RaceManager.Track = trackData.trackNames[stageIndex];
        trackNameText.text = trackData.trackNames[stageIndex];
        scoreText.text ="High Score: "+UISpeedDisplay.FormatTime(PlayerPrefs.GetFloat("Record " + RaceManager.Track, 0.0f));
        trackDescText.text = trackData.trackDescriptions[stageIndex];
        trackImg.sprite = trackData.trackImages[stageIndex];
        
    }
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        if (phase == 1)
        {
            if (canCycle)
            {
                if (h < -0.3f)
                {
                    canCycle = false;
                    stageIndex--;
                    if (stageIndex<0)
                    {
                        stageIndex = trackData.trackNames.Count-1;
                    }
                    SetStage();
                }
                if (h > 0.3f)
                {
                    canCycle = false;
                    stageIndex = (stageIndex + 1) % trackData.trackNames.Count;
                    SetStage();
                }
            }
            if (GameInputManager.GetKeyDown("1-Fire1"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Select");
            }
        }
        if (phase == 0) {
            if (h < -0.5f)
            {
                canCycle = false;
                phase = -1;
                mainText.gameObject.SetActive(false);
                settingsMenu.SetActive(true);
            }
            if (h > 0.5f)
            {
                canCycle = false;
                phase = 1;
                mainText.gameObject.SetActive(false);
                raceMenu.SetActive(true);
            }
            
        }

        if (Mathf.Abs(h) < 0.2f)
        {
            canCycle = true;
        }

        if (phase!= 0 && GameInputManager.GetKeyDown("1-Fire2"))
        {
            phase = 0;
            mainText.gameObject.SetActive(true);
            settingsMenu.SetActive(false);
            raceMenu.SetActive(false);
        }
    }
}
