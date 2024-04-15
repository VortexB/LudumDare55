using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicActor : BoardActor
{
    protected override bool MovedApon(BoardActor actor)
    {
        return true;
    }

    protected override bool CanMoveApon(BoardActor actor)
    {
        return true;
    }
}
