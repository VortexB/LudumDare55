using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacleActor : BoardActor
{
    protected override bool MovedApon(BoardActor actor)
    {
        if (actor is PlayerActor)
        {
            return false;
        }
        return true;
    }

    protected override bool CanMoveApon(BoardActor actor)
    {
        if (actor is PlayerActor)
        {
            return false;
        }
        return true;
    }
}
