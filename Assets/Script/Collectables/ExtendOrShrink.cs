using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendOrShrink : Collectable {

    public float NewWidth = 1.0f;

    protected override void ApplyEffect()
    {
        if(Paddle.Instance != null && !Paddle.Instance.PaddleIsTransforming)
        {
            Paddle.Instance.StartWidthAnimation(NewWidth);
        }
    }
}
