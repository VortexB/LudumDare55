using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableActor : BoardActor
{
    protected override bool MovedApon(BoardActor actor)
    {
        if (actor.isActorType<PlayerActor>() || actor.isActorType<SkeletonActor>()||actor.isActorType<PushableActor>())
        {
            Vector2Int delta = BoardPosition- actor.BoardPosition;
            if(CanMove(BoardPosition+delta)){
                Move(BoardPosition+delta);
                return true;
            }
            return false;
        }
        return true;
    }
    protected override bool CanMoveApon(BoardActor actor)
    {
        if (actor.isActorType<PlayerActor>() || actor.isActorType<SkeletonActor>()||actor.isActorType<PushableActor>())
        {
            Vector2Int delta = BoardPosition- actor.BoardPosition;
            if(CanMove(BoardPosition+delta)){
                return true;
            }
            return false;
        }
        return true;
    }
}
