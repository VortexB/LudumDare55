using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleActor : BoardActor
{
    protected override bool MovedApon(BoardActor actor)
    {
        if (actor.isActorType<PlayerActor>()||actor.isActorType<SkeletonActor>()||actor.isActorType<PushableActor>() || actor.isActorType<ObstacleActor>())
        {
            return false;
        }
        return true;
    }

        protected override bool CanMoveApon(BoardActor actor)
    {
        if (actor.isActorType<PlayerActor>()||actor.isActorType<SkeletonActor>()||actor.isActorType<PushableActor>()|| actor.isActorType<ObstacleActor>())
        {
            return false;
        }
        return true;
    }
}
