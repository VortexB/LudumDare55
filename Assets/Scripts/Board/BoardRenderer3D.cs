using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer3D : BoardRenderer
{
    [SerializeField] bool drawGizmos = true;

    
    public override Direction ActorDirection(Transform actor)
    {
        return Direction.Down;
    }

    public override Vector2Int StrictBoardPosition(Vector3 worldPosition)
    {
        Vector2Int possibleBoardPosition = Vector2Int.RoundToInt(new Vector3((int)Math.Floor(worldPosition.x / cellSize.x), (int)Math.Floor(worldPosition.z/cellSize.y)) - new Vector3(transform.position.x,transform.position.z));
        if (possibleBoardPosition.x > Board.boardSize.x)
        {
            possibleBoardPosition.x = Board.boardSize.x;
        }
        if (possibleBoardPosition.x < 0)
        {
            possibleBoardPosition.x = 0;
        }
        if (possibleBoardPosition.y > Board.boardSize.y)
        {
            possibleBoardPosition.y = Board.boardSize.y;
        }
        if (possibleBoardPosition.y < 0)
        {
            possibleBoardPosition.y = 0;
        }
        return possibleBoardPosition;
    }

    public override Vector3 VisualWorldPosition(Vector2Int boardPosition)
    {
        return new Vector3(boardPosition.x * cellSize.x, offset.y, boardPosition.y*cellSize.y) + transform.position + offset;
        // return (new Vector2(1,0.5f)*boardPosition.x/2)+ (new Vector2(-1, 0.5f) * boardPosition.y/2)+offset+_position; //isometric

    }

    
    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            for (int i = 0; i < Board.boardSize.x; i++)
            {
                for (int j = 0; j < Board.boardSize.y; j++)
                {
                    Gizmos.color = new Color(1,1,1,0.2f);
                    Gizmos.DrawCube(VisualWorldPosition(new Vector2Int(i,j)),new Vector3(0.9f*cellSize.x,0.9f,0.9f*cellSize.y));
                }
            }
        }
    }
}
