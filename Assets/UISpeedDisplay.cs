using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpeedDisplay : MonoBehaviour
{
    public CarController car;
    public TMPro.TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (car.finalSpeed*3).ToString("F0") + " u/s";
    }
}
