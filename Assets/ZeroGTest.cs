using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGTest : MonoBehaviour
{

    public Vector3 upDirection = Vector3.up;

    public float gravityIntensity = -1;

    public Rigidbody rb;

    public float speed = 9;
    public float gravIncrease = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 belowAngle = Vector3.zero;
        RaycastHit hit;
        Physics.Raycast(transform.position,transform.forward,out hit,15);
        if (hit.collider!=null)
        {
            upDirection = Vector3.Lerp(upDirection, hit.normal, Time.deltaTime * 6);
        } else
        {
            Physics.Raycast(transform.position, -transform.up, out hit, 15);
            if (hit.collider != null)
            {
                upDirection = Vector3.Lerp(upDirection, hit.normal, Time.deltaTime * 6);
            }
        }

        gravityIntensity -= gravIncrease * Time.deltaTime;

        rb.velocity = (upDirection * gravityIntensity) + transform.forward*speed;
    }

    private void OnCollisionStay(Collision collision)
    {
        gravityIntensity = -1;
    }

}
