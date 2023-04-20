using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int bodyStat = 0;
    public int speedStat = 0;
    public int handlingStat = 0;
    public int accelerationStat = 0;
    public int skillStat = -1;
    public int weight = 1000;
    public float accelStat = 0;
    public float gravity = 24;

    public float height = 2;


    public int PlayerID = 1;

    public LayerMask groundedLayerMask;

    float baseRotation = 120;

    public float weightMultiplier
    {
        get
        {
            return weight / 1000f;
        }
    }

    public float GetWeightImpact(float multiplier)
    {
        return 1 + (weightMultiplier - 1) * multiplier;
    }


    public float maxRotation
    {
        get
        {
            return (baseRotation+ (10 * handlingStat))/ weightMultiplier;
        }
    }

    public float maxSpeed
    {
        get
        {
            return (400 + (30 * speedStat) + (100*(1-accelValue))) / GetWeightImpact(0.5f);
        }
    }

    public float finalSpeed
    {
        get
        {
            return speed * (currSpeedMult+boostGroundMultiplier);
        }
    }

    public float accelValue
    {
        get
        {
            return (Mathf.Pow(1 - accelStat,1.75f)/weightMultiplier) * (1+accelerationStat*0.15f);
        }
    }

    public float maxHealth
    {
        get
        {
            return (1 + (0.15f * bodyStat)) * weightMultiplier;
        }
    }

    public bool inJump
    {
        get
        {
            return Time.time < lastJumpTime+1;
        }
    }

    [Header("Object References")]
    public Rigidbody rb;
    public GameObject shipModel;
    public GameObject gravityAligner;
    public GameObject cameraAligner;
    public GameObject shipDisplay;
    public Camera camera;
    public AudioSource audioSrc;
    public TMPro.TextMeshProUGUI lapText;
    public AudioClip lapSound;
    public AudioClip[] hitSounds;

    [Header("Skills")]
    public List<Skill> skills;

    [Header("Gameplay Variables")]
    public float speed = 0;
    public float groundSpeedMultiplier = 1;
    public float boostGroundMultiplier = 0;
    public int lap = 0;
    public int checkpointIndex = 0;
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
    public bool isFinished;
    public float startTime = 0;
    public float finishTime = 0;

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
        health = maxHealth;
    }
    float rotation = 0;
    float turnDir = 0;

    bool hasDied = false;
    // Update is called once per frame
    void Update()
    {
        if (!canDrive) { return; }
        if (health<=0) { 
            if (!hasDied && !isFinished)
            {
                hasDied = true;
                RaceManager.instance.FinishWithDeath();
            }
            return; 
        }

        float fac = (1 + speed / maxSpeed);

        isGrounded = Physics.Raycast(transform.position, -gravityDirection, height * fac * 2f, groundedLayerMask);
        bool hittingForward = Physics.Raycast(transform.position, transform.forward, 25 * (speed / maxSpeed));
        if (hittingForward)
        {
            fac += 1.5f;
        }
        //Inputs

        isAccelerating = GameInputManager.GetKey("1-Fire1");
        float h = Input.GetAxis("Horizontal");

        //Skills

        if (skillStat!=-1)
        {
            if (GameInputManager.GetKeyDown("1-Fire3"))
            {
                if (skills[skillStat].CheckFire())
                {
                    skills[skillStat].Activate();
                }
            }
        }

        //Drifting

        if (GameInputManager.GetKey("1-Fire2") && isGrounded && !isDrifting && Mathf.Abs(h)>0.3f && !isFinished)
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
        if (GameInputManager.GetKeyUp("1-Fire2") && isDrifting)
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
                turnDir = Mathf.Clamp(turnDir + f * Time.deltaTime*4, 0.5f, 2f);
            } else
            {
                turnDir = Mathf.Clamp(turnDir + f * Time.deltaTime * 4, -2f, -0.5f);
            }
        } else
        {
            turnDir = h;
        }
        //Acceleration
        //
        //
        float accelSpeed = (200 + (20 * speedStat)) *(accelValue*0.75f+0.25f);
        float decelSpeed = (200 + (20 * speedStat)) / weightMultiplier;
        if (isAccelerating&&!isFinished)
        {
            if (speed < maxSpeed && isGrounded)
            {
                float mult = speed < maxSpeed*0.25f ? 4 : 1;
                //rb.AddRelativeForce(0, 0, accelSpeed);
                speed += accelSpeed*Time.deltaTime*mult;
            }
        } else if (speed > 20)
        {
            //rb.AddRelativeForce(0, 0, -accelSpeed * 2);
            speed -= decelSpeed * Time.deltaTime * (1.25f+handlingStat*0.1f);
            if (speed<20)
            {
                speed = 20;
            }
        } else if (speed < 0)
        {
            speed *= Mathf.Lerp(speed, 0, Time.deltaTime * 4);
        }

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        //Rotation
        //
        //

        //turnSpeed = Mathf.Lerp(turnSpeed,isDrifting?0.1f:1,Time.deltaTime* (isDrifting?0.5f:6));
        if (((!isDrifting&& Mathf.Abs(h) < 0.03f) ||driftingCanEnd)||isFinished)
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
            rotation = Mathf.Clamp(turnDir*maxRotation * (isGrounded?1.2f:0.15f) * (1+0.25f*handlingStat), -maxRotation*(isDrifting?fac+0.5f:1), maxRotation * (isDrifting ?fac+0.5f : 1));
        }
        shipModel.transform.Rotate(new Vector3(0, rotation, 0) * Time.deltaTime);

        //Hover
        //
        //

        RaycastHit hit;
        Physics.Raycast(transform.position, -gravityDirection, out hit, height * fac, groundedLayerMask);

        //Ground types
        //
        //

        Material material = TryGetMaterial(hit);
        if (material != null)
        {
            if (material.name.ToLower().Contains("slow"))
            {
                groundSpeedMultiplier = 0.33f;
            }
            else if (material.name.ToLower().Contains("fast"))
            {
                groundSpeedMultiplier = 1.5f * (OptionManager.IsEnabledOption("Ultraspeed")?2:1);
            }
            else
            {
                groundSpeedMultiplier = 1;
            }

            if (material.name.ToLower().Contains("boost") && canGroundBoost)
            {
                boostGroundMultiplier = 0.75f * (OptionManager.IsEnabledOption("Ultraspeed") ? 2 : 1);
                if (speed<maxSpeed)
                {
                    speed = maxSpeed;
                }
                canGroundBoost = false;
            } else
            {
                canGroundBoost = true;
            }

            if (material.name.ToLower().Contains("bounce"))
            {
                Jump(gravity);
            }

            if (material.name.ToLower().Contains("heal"))
            {
                health = Mathf.Clamp(health+Time.deltaTime/2,0,maxHealth);
            }
        }
        currSpeedMult = Mathf.Lerp(currSpeedMult, groundSpeedMultiplier, Time.deltaTime * 3);
        boostGroundMultiplier = Mathf.Lerp(boostGroundMultiplier, 0, Time.deltaTime);

        //Debug.Log($"Material: {(material!=null?material.name:"Null")}");

        if (hit.collider!=null && !inJump)
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
            if (Physics.Raycast(transform.position, -gravityDirection, height* 2f, groundedLayerMask) && !inJump)
            {
                gravityIntensity = -1;
            }
            gravityIntensity -= gravity*weightMultiplier*Time.deltaTime;
        }

        //Gravity

        RaycastHit hit2 = new RaycastHit();
        if (canChangeRotation)
        {
            
            Physics.Raycast(transform.position, -gravityDirection, out hit2, height * fac * 5f);
            if (hit2.collider != null && hit2.collider.tag!="Obstacle" && hit2.normal!=gravityDirection) {
                //lastSwitchTime = Time.time + 0.15f;
                gravityDirection = hit2.normal;//Vector3.Lerp(gravityDirection, hit.normal, Time.deltaTime*6);
                //Debug.Log(gravityDirection);
            }

        }

        Quaternion q = Quaternion.FromToRotation(gravityAligner.transform.up, gravityDirection) * gravityAligner.transform.rotation;
        gravityAligner.transform.rotation = Quaternion.Lerp(gravityAligner.transform.rotation,q,Time.deltaTime*8);
        if (hittingForward||(hit2.collider!= null && hit2.distance > height*5))
        {
            gravityAligner.transform.rotation = q;
        }
        //gravityAligner.transform.rotation = q;
        float scaleSpeed = 4;
        if (Mathf.Abs(gravityAligner.transform.localEulerAngles.x-cameraAligner.transform.localEulerAngles.x)>35)
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

    float lastJumpTime = 0;
    public void Jump(float intensity)
    {
        lastJumpTime=Time.time;
        gravityIntensity = intensity;
    }
    Coroutine textRoutine;
    public void DisplayText(string text)
    {
        if (this.lapText!=null)
        {
            if (textRoutine!=null)
            {
                StopCoroutine(textRoutine);
            }
            textRoutine = StartCoroutine(displayText(text));
        }
    }

    IEnumerator displayText(string t)
    {
        lapText.text = t;
        yield return new WaitForSeconds(2);
        lapText.text = "";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            speed *= -0.2f;
            health -= 0.02f;
            audioSrc.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
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
    float ckptBuffer = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Trigger")
        {
            other.GetComponent<Trigger>().OnTriggerActivate(this);
        }
        if (other.tag=="Checkpoint")
        {
            if (checkpointIndex >= 0 && RaceManager.instance.checkpoints.Count>0 && RaceManager.instance.checkpoints[checkpointIndex]==other.gameObject)
            {
                checkpointIndex++;
            }
        }
        if (other.tag=="Lap" && Time.time>ckptBuffer)
        {
            ckptBuffer = Time.time + 1;
            if (checkpointIndex == RaceManager.instance.checkpoints.Count || RaceManager.instance.checkpoints.Count == 0)
            {
                checkpointIndex = 0;
                
                if (lap == RaceManager.instance.laps-1)
                {
                    isFinished = true;
                    rb.velocity = Vector3.zero;
                    finishTime = Time.time - startTime;
                    RaceManager.instance.FinishGame(RaceManager.SubmitFinalTime(finishTime));
                } else
                {
                    DisplayText("Lap");
                    audioSrc.PlayOneShot(lapSound);
                    lap++;
                    health = Mathf.Clamp(health + 0.25f, 0, maxHealth);
                }       
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag=="Bounds")
        {
            health = -1;
        }
    }
}
