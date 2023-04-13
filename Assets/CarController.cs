﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int bodyStat = 0;
    public int speedStat = 0;
    public int handlingStat = 0;
    public int skillStat = -1;
    public float gravity = 24;

    public float height = 2;

    public LayerMask groundedLayerMask;

    float baseRotation = 120;
    public float maxRotation
    {
        get
        {
            return baseRotation + (25 * handlingStat) * (isDrifting?2:1);
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
            return speed * (currSpeedMult+boostGroundMultiplier);
        }
    }

    [Header("Object References")]
    public Rigidbody rb;
    public GameObject shipModel;
    public GameObject gravityAligner;
    public GameObject cameraAligner;
    public GameObject shipDisplay;
    public Camera camera;

    [Header("Skills")]
    public List<Skill> skills;

    [Header("Gameplay Variables")]
    public float speed = 0;
    public float groundSpeedMultiplier = 1;
    public float boostGroundMultiplier = 0;
    float currSpeedMult = 1;
    public Vector3 gravityDirection = Vector3.up;
    public float gravityIntensity = -1;

    public bool isAccelerating = false;
    public bool isDrifting = false;
    public bool isGrounded = true;
    public bool driftingCanEnd = true;
    public bool canDrive = true;
    public float driftAngle = 0;
    public float health = 1;
    public bool canChangeRotation = false;

    public bool canGroundBoost = true;

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
        if (skillStat == -1)
        {
            bodyStat += 1;
            speedStat += 1;
            handlingStat += 1;
        }
    }
    float rotation = 0;
    float turnDir = 0;
    // Update is called once per frame
    void Update()
    {
        if (!canDrive) { return; }
        if (health<=0) { return; }

        float fac = (1 + speed / maxSpeed);

        isGrounded = Physics.Raycast(transform.position, -gravityDirection, height * fac * 2f, groundedLayerMask);
        bool hittingForward = Physics.Raycast(transform.position, transform.forward, 50 * (speed / maxSpeed));
        if (hittingForward)
        {
            fac += 1.5f;
        }

        //Inputs

        isAccelerating = Input.GetButton("Fire1");
        float h = Input.GetAxis("Horizontal");

        //Skills

        if (skillStat!=-1)
        {
            if (Input.GetButtonDown("Fire3"))
            {
                if (skills[skillStat].CheckFire())
                {
                    skills[skillStat].Activate();
                }
            }
        }

        //Drifting

        if (Input.GetButton("Fire2") && !isDrifting && Mathf.Abs(h)>0.3f)
        {
            isDrifting = true;
            driftingCanEnd = false;
            if (h > 0)
            {
                driftAngle = 1;
            } else
            {
                driftAngle = -1;
            }
            //turnDir = driftAngle * 1.5f;
        }
        if (Input.GetButtonUp("Fire2") && isDrifting)
        {
            driftingCanEnd = true;
        }

        if (isDrifting && (!driftingCanEnd|| (h * driftAngle)>0.2))
        {
            float f = ((h) * (3f + 0.5f * handlingStat));
            if ((h * driftAngle) < 0)
            {
                f *= 0.75f;
            }
            if (h*driftAngle>0.4f)
            {
                f += driftAngle;
            }
            if (driftAngle > 0)
            {
                turnDir = Mathf.Clamp(turnDir + f * Time.deltaTime*4, 0.5f, 2.5f);
            } else
            {
                turnDir = Mathf.Clamp(turnDir + f * Time.deltaTime * 4, -2.5f, -0.5f);
            }
        } else
        {
            turnDir = h;
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

        //turnSpeed = Mathf.Lerp(turnSpeed,isDrifting?0.1f:1,Time.deltaTime* (isDrifting?0.5f:6));
        
        if ((!isDrifting&& Mathf.Abs(h) < 0.03f) ||driftingCanEnd)
        {
            rotation  = Mathf.Lerp(rotation,0,Time.deltaTime*10*(isDrifting?2:1));
            if (Mathf.Abs(rotation)<0.5f)
            {
                rotation = 0;
            }
            if (driftingCanEnd && Mathf.Abs(rotation)<2)
            {
                isDrifting = false;
                driftingCanEnd = false;
            }
        } else
        {
            rotation = Mathf.Clamp(turnDir*maxRotation * (isGrounded?1.2f:0.25f), -maxRotation*(isDrifting?fac+0.5f:1), maxRotation * (isDrifting ?fac+0.5f : 1));
        }
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

            if (material.name.ToLower().Contains("boost") && canGroundBoost)
            {
                boostGroundMultiplier = 1;
                canGroundBoost = false;
            } else
            {
                canGroundBoost = true;
            }

            if (material.name.ToLower().Contains("bounce"))
            {
                Jump(10);
            }

            if (material.name.ToLower().Contains("heal"))
            {
                health = Mathf.Clamp(health+Time.deltaTime/2,0,1);
            }
        }
        currSpeedMult = Mathf.Lerp(currSpeedMult, groundSpeedMultiplier, Time.deltaTime * 3);
        boostGroundMultiplier = Mathf.Lerp(boostGroundMultiplier, 0, Time.deltaTime);

        //Debug.Log($"Material: {(material!=null?material.name:"Null")}");

        if (hit.collider!=null)
        {
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

        //Gravity

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

        camera.fieldOfView = Mathf.Clamp(Mathf.LerpUnclamped(40, 70, Mathf.Log((finalSpeed / maxSpeed)+1, 2)),30,120);
        shipDisplay.transform.localEulerAngles = new Vector3(0, h*8 * (isDrifting ? 1.5f : 1), h*-15 * (isDrifting ? 0.5f : 1));

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

    public void Jump(float intensity)
    {
        gravityIntensity = intensity;
    }
    List<Vector3> positions = new List<Vector3>();
    void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        if (isDrifting)
        {
            positions.Add(transform.position);

        }
        foreach (Vector3 pos in positions)
        {
            Gizmos.DrawSphere(pos, 2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            speed *= -0.25f;
            health -= 0.01f;
            //rb.MovePosition(transform.position+transform.TransformDirection(-Vector3.forward));
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag=="Obstacle")
        {
            health -= 0.005f * Time.deltaTime;
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
