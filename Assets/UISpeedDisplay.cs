using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpeedDisplay : MonoBehaviour
{
    public CarController car;
    public TMPro.TextMeshProUGUI text;
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
    }
}
