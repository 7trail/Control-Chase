﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int bodyStat = 0;
    public int speedStat = 0;
    public int handlingStat = 0;
    public float gravity = 24;

    public float height = 2;

    public LayerMask groundedLayerMask;

    float baseRotation = 120;
    public float maxRotation
    {
        get
        {
            return baseRotation + (25 * handlingStat);
        }
    }

    public float maxSpeed
    {
        get
        {
            return 400 + (30 * speedStat);
        }
    }

    public float finalSpeed
    {
        get
        {
            return speed * currSpeedMult;
        }
    }

    [Header("Object References")]
    public Rigidbody rb;
    public GameObject shipModel;
    public GameObject gravityAligner;
    public GameObject cameraAligner;
    public Camera camera;

    [Header("Gameplay Variables")]
    public float speed = 0;
    public float groundSpeedMultiplier = 1;
    float currSpeedMult = 1;
    public Vector3 gravityDirection = Vector3.up;
    public float gravityIntensity = -1;

    public bool isAccelerating = false;

    public bool canChangeRotation = false;

    public void ToggleRotation(bool b)
    {
        canChangeRotation = b;
        if (b)
        {
            //rb.constraints = RigidbodyConstraints.None;
        } else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }
    float rotation = 0;
    
    // Update is called once per frame
    void Update()
    {

        float fac = (1 + speed / maxSpeed);
        bool hittingForward = Physics.Raycast(transform.position, transform.forward, 50 * (speed / maxSpeed));
        if (hittingForward)
        {
            fac += 1.5f;
        }

        //Inputs

        if (Input.GetButtonDown("Fire1"))
        {
            isAccelerating = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isAccelerating = false;
        }

        //speed = transform.InverseTransformDirection(rb.velocity).z;

        //Acceleration
        //
        //
        float accelSpeed = 200 + (20 * speedStat);
        if (isAccelerating)
        {
            if (speed < maxSpeed)
            {
                float mult = speed < 200 ? 4 : 1;
                //rb.AddRelativeForce(0, 0, accelSpeed);
                speed += accelSpeed*Time.deltaTime*mult;
            }
        } else if (speed > 20)
        {
            //rb.AddRelativeForce(0, 0, -accelSpeed * 2);
            speed -= accelSpeed * 2 * Time.deltaTime;
        } else if (speed < 0)
        {
            speed *= Mathf.Lerp(speed, 0, Time.deltaTime * 4);
        }

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        //Rotation
        //
        //
        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) < 0.03f)
        {
            rotation  = Mathf.Lerp(rotation,0,Time.deltaTime*6);
            if (Mathf.Abs(rotation)<0.5f)
            {
                rotation = 0;
            }
        } else
        {
            //rotation += h * Time.deltaTime * baseRotation*6;
            rotation = Mathf.Clamp(h*maxRotation * (Physics.Raycast(transform.position, -gravityDirection, height * fac * 2f, groundedLayerMask)?1:0.25f), -maxRotation, maxRotation);
        }
        //totalRotation += rotation * Time.deltaTime;
        //if (totalRotation>720*7)
        //{
        //    totalRotation -= 720*7;
        //}
        shipModel.transform.Rotate(new Vector3(0, rotation, 0) * Time.deltaTime);

        //Hover
        //
        //
        

        RaycastHit hit;
        Physics.Raycast(transform.position, -gravityDirection, out hit, height * fac, groundedLayerMask);

        Material material = TryGetMaterial(hit);
        if (material != null)
        {
            if (material.name.ToLower().Contains("slow"))
            {
                groundSpeedMultiplier = 0.33f;
            }
            else if (material.name.ToLower().Contains("fast"))
            {
                groundSpeedMultiplier = 1.5f;
            }
            else
            {
                groundSpeedMultiplier = 1;
            }
        }
        currSpeedMult = Mathf.Lerp(currSpeedMult, groundSpeedMultiplier, Time.deltaTime * 3);

        //Debug.Log($"Material: {(material!=null?material.name:"Null")}");

        if (hit.collider!=null)
        {
            //float mult = 1;
            //gravityIntensity += gravity*Time.deltaTime*mult;
            //gravityIntensity = -2;
            if (gravityIntensity<0)
            {
                gravityIntensity /= 2;
                gravityIntensity += 1 * Time.deltaTime;
            } else
            {
                gravityIntensity = 6 / (hit.distance/(height*fac));
            }
        } else
        {
            //float mult = 1;
            if (Physics.Raycast(transform.position, -gravityDirection, height* 2f, groundedLayerMask))
            {
                gravityIntensity = -1;
            }
            gravityIntensity -= gravity*Time.deltaTime;
        }

        //Gravity and rotation

        if (canChangeRotation)
        {
            Physics.Raycast(transform.position, -gravityDirection, out hit, height * fac * 5f, groundedLayerMask);
            if (hit.collider != null && hit.normal!=gravityDirection) {
                //lastSwitchTime = Time.time + 0.15f;
                gravityDirection = hit.normal;//Vector3.Lerp(gravityDirection, hit.normal, Time.deltaTime*6);
                //Debug.Log(gravityDirection);
            }

        }

        Quaternion q = Quaternion.FromToRotation(gravityAligner.transform.up, gravityDirection) * gravityAligner.transform.rotation;
        gravityAligner.transform.rotation = Quaternion.Lerp(gravityAligner.transform.rotation,q,Time.deltaTime*8);
        float scaleSpeed = 4;
        if (Mathf.Abs(gravityAligner.transform.localEulerAngles.x-cameraAligner.transform.localEulerAngles.x)>25)
        {
            scaleSpeed = 8;
        }
        cameraAligner.transform.rotation = Quaternion.Lerp(cameraAligner.transform.rotation, shipModel.transform.rotation, Time.deltaTime * scaleSpeed);

        //VFX
        //
        //

        camera.fieldOfView = Mathf.Clamp(Mathf.LerpUnclamped(40, 70, finalSpeed / maxSpeed),30,90);

        //Final Movement Stuff
        //
        //
        Vector3 vector = shipModel.transform.TransformDirection(new Vector3(0, 0, finalSpeed));
        rb.velocity = (gravityDirection * gravityIntensity)+ vector;
    }

    public Material TryGetMaterial(RaycastHit hit)
    {
        //Debug.DrawLine(transform.position, hit.point, Color.cyan);
        if (hit.collider== null) { return null; }
        Mesh m = GetMesh(hit.collider.gameObject);
        if (m)
        {
            int[] hittedTriangle = new int[]
            {
                    m.triangles[hit.triangleIndex * 3],
                    m.triangles[hit.triangleIndex * 3 + 1],
                    m.triangles[hit.triangleIndex * 3 + 2]
            };
            for (int i = 0; i < m.subMeshCount; i++)
            {
                int[] subMeshTris = m.GetTriangles(i);
                for (int j = 0; j < subMeshTris.Length; j += 3)
                {
                    if (subMeshTris[j] == hittedTriangle[0] &&
                        subMeshTris[j + 1] == hittedTriangle[1] &&
                        subMeshTris[j + 2] == hittedTriangle[2])
                    {
                        //Debug.Log(string.Format("triangle index:{0} submesh index:{1} submesh triangle index:{2}", hit.triangleIndex, i, j / 3));
                        return hit.collider.GetComponent<MeshRenderer>().materials[i];
                    }
                }
            }
        }
        return null;
    }

    static Mesh GetMesh(GameObject go) { if (go) { MeshFilter mf = go.GetComponent<MeshFilter>(); if (mf) { Mesh m = mf.sharedMesh; if (!m) { m = mf.mesh; } if (m) { return m; } } } return (Mesh)null; }


private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            speed *= -0.25f;
            rb.MovePosition(transform.position+transform.TransformDirection(-Vector3.forward));
            if (Physics.Raycast(transform.position, transform.forward, 4))
            {
                isAccelerating = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag=="Obstacle")
        {
            speed *= 0.8f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Trigger")
        {
            other.GetComponent<Trigger>().OnTriggerActivate(this);
        }
    }
}
