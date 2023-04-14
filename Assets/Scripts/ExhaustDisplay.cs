using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustDisplay : MonoBehaviour
{
    public MeshRenderer m;
    public CarController car;
    public Color clr1;
    public Color clr2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fac = (car.finalSpeed / car.maxSpeed);
        m.material.SetFloat("_Value", Mathf.Clamp(fac,0,1));
        if (fac > 1)
        {
            m.material.SetColor("_Color1", Color.Lerp(clr1,clr2,fac-1));
        } else
        {
            m.material.SetColor("_Color1", clr1);
        }
    }
}
