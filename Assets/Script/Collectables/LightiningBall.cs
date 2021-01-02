using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightiningBall : Collectable
{
    protected override void ApplyEffect()
    {
        foreach (Ball ball in BallsManager.Instance.Balls)
        {
            ball.StartLightningBall();
        }
    }
}
