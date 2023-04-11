using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRotation : Trigger
{
    public bool allowRotation;
    public bool forceRotation = true;
    public bool flipRotation = false;
    public override void OnTriggerActivate(CarController car)
    {
        base.OnTriggerActivate(car);
        car.ToggleRotation(allowRotation);
        if (forceRotation) car.gravityDirection = transform.up;

        if (flipRotation) {
            Vector3 v = car.gravityAligner.transform.InverseTransformDirection(car.gravityDirection);
            v.y *= -1;
            car.gravityDirection = car.gravityAligner.transform.TransformDirection(v);
        } //car.gravityDirection *= -1;

    }
}
