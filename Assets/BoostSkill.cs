using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSkill : Skill
{
    public bool stillRunning = false;
    public override bool CanFire()
    {
        return base.CanFire() && !stillRunning;
    }
    public override void Activate()
    {
        base.Activate();
    }
}
