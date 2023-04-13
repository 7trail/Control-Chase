using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpeedDisplay : MonoBehaviour
{
    public CarController car;
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI lapText;
    public TMPro.TextMeshProUGUI timeText;
    public UnityEngine.UI.Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = car.health;
        text.text = (car.finalSpeed*3).ToString("F0") + " u/s";
        lapText.text = "Lap " + (car.lap+1) + "/" + RaceManager.instance.laps;
        
        timeText.text = FormatTime(car.canDrive?(car.isFinished?car.finishTime: Time.time-car.startTime):0);
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
