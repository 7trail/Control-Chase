using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSkill : Skill
{
    public float jumpIntensity = 10;
    public override bool CanFire()
    {
        return base.CanFire() && car.isGrounded;
    }

    public override void Activate()
    {
        base.Activate();
        car.Jump(jumpIntensity);
    }
}
