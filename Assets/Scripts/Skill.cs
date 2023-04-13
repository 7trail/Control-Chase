﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public CarController car;
    public AudioSource soundSrc;
    public AudioClip rejectClip;
    [Header("Base Skill Parameters")]
    public float healthCost = 0;

    public virtual bool CanFire()
    {
        return car.health > healthCost;
    }

    public virtual bool CheckFire()
    {
        if (!CanFire())
        {
            soundSrc.PlayOneShot(rejectClip);
            return false;
        }
        return true;
    }

    public virtual void Activate()
    {
        car.health -= healthCost;
    }
}
