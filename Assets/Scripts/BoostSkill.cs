using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSkill : Skill
{
    public bool stillRunning = false;
    public AudioClip boostClip;
    public override bool CanFire()
    {
        return base.CanFire() && !stillRunning;
    }
    public override void Activate()
    {
        base.Activate();
        stillRunning = true;
        StartCoroutine(BoostRoutine());
        soundSrc.PlayOneShot(boostClip);
    }

    IEnumerator BoostRoutine()
    {
        
        float time = Time.time + 1f;
        while (Time.time< time)
        {
            if (car.boostGroundMultiplier < 1.2f)
            {
                car.boostGroundMultiplier += 3f*Time.deltaTime;
            }
            yield return null;
        }
        stillRunning = false;
    }
}
