using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    public int phase = 0;
    public int stageIndex = 0;
    public int optIndex = 0;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI optionListText;
    public TextMeshProUGUI optionDescText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI trackNameText;
    public TextMeshProUGUI trackDescText;
    public Image trackImg;
    public GameObject settingsMenu;
    public GameObject raceMenu;
    public GameObject optionsMenu;

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
        scoreText.text =$"High Score ({OptionManager.GetActiveOptions("/")}): "+UISpeedDisplay.FormatTime(PlayerPrefs.GetFloat(RaceManager.GetRecordString(RaceManager.Track), 0.0f));
        trackDescText.text = trackData.trackDescriptions[stageIndex];
        trackImg.sprite = trackData.trackImages[stageIndex];
        
    }
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
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

            if (v > 0.5f)
            {
                canCycle = false;
                phase = 2;
                optionsMenu.SetActive(true);
                //mainText.gameObject.SetActive(false);
                //raceMenu.SetActive(true);
            }

        }

        if (phase == 2)
        {
            if (v > 0.5f && canCycle)
            {
                canCycle = false;
                optIndex--;
                if (optIndex<0)
                {
                    optIndex = OptionManager.options.Count-1;
                }
            }
            if (v < -0.5f && canCycle)
            {
                canCycle = false;
                optIndex++;
                if (optIndex > OptionManager.options.Count-1)
                {
                    optIndex = 0;
                }
            }
            optionListText.text = "";
            int i = 0;
            foreach(Option option in OptionManager.options)
            {
                optionListText.text += $"{option.displayName} ({option.identitifer}) - {(option.enabled ? "Enabled" : "Disabled")} {(i==optIndex?"<<":"")}\n";
                i++;
            }
            optionDescText.text = OptionManager.options[optIndex].description;
            if (GameInputManager.GetKeyDown("1-Fire1"))
            {
                OptionManager.options[optIndex].enabled = !OptionManager.options[optIndex].enabled;
            }
        }

        if (Mathf.Abs(h) < 0.2f&& Mathf.Abs(v) < 0.2f)
        {
            canCycle = true;
        }

        if (phase!= 0 && GameInputManager.GetKeyDown("1-Fire2"))
        {
            phase = 0;
            mainText.gameObject.SetActive(true);
            settingsMenu.SetActive(false);
            raceMenu.SetActive(false);
            optionsMenu.SetActive(false);
        }
    }
}
