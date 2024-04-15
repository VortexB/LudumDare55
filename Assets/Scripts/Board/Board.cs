using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] BoardTile[][] tileArray;
    [SerializeField] public Vector2Int boardSize = new Vector2Int(10, 10);

    [SerializeField] public float rotation = 0;

    public BoardRenderer BoardRenderer { get => GetComponent<BoardRenderer>(); }
    public BoardProperties BoardProperties { get => GetComponent<BoardProperties>(); }
    public Vector3 TransformPosition { get => transform.position; }
    public void GenerateBoard()
    {
        tileArray = new BoardTile[boardSize.x][];
        for (int i = 0; i < boardSize.x; i++)
        {
            tileArray[i] = new BoardTile[boardSize.y];
            for (int j = 0; j < boardSize.y; j++)
            {
                tileArray[i][j] = new BoardTile();
            }
        }
    }
    public Vector3 WorldPosition(Vector2Int boardPosition)
    {
        return BoardRenderer.VisualWorldPosition(boardPosition);
    }
    public Vector2Int ToBoardPosition(Vector3 worldPosition)
    {
        return BoardRenderer.StrictBoardPosition(worldPosition);
    }

    public Direction ActorDirection(Transform actor)
    {
        return BoardRenderer.ActorDirection(actor);
    }
    public bool isValidTile(Vector2Int position)
    {
        return !(position.x >= boardSize.x || position.x < 0 || position.y >= boardSize.y || position.y < 0);
    }
    public BoardActor[] GetActors()
    {
        List<BoardActor> actors = new List<BoardActor>();
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                actors.AddRange(tileArray[i][j].Actors);
            }
        }
        return actors.ToArray();
    }
    public T[] GetActors<T>()
    {
        var actors = GetActors();
        List<T> typedActors = new List<T>();
        foreach (var a in actors)
        {
            if (a.GetComponent<T>() != null)
            {
                typedActors.Add(a.GetComponent<T>());
            }
        }
        return typedActors.ToArray();
    }
    public BoardActor[] GetActors(Vector2Int position)
    {
        if (!isValidTile(position)) return new BoardActor[0];
        return tileArray[position.x][position.y].Actors;
    }
    public T[] GetActors<T>(Vector2Int position)
    {
        if (!isValidTile(position)) return new T[0];
        var actors = tileArray[position.x][position.y].Actors;
        List<T> typedActors = new List<T>();
        foreach (var a in actors)
        {
            if (a.GetComponent<T>() != null)
            {
                typedActors.Add(a.GetComponent<T>());
            }
        }
        return typedActors.ToArray();
    }
    public T[] GetActors<T>(Vector2Int[] positions)
    {
        List<T> typedActors = new List<T>();
        foreach (var p in positions)
        {
            if (!isValidTile(p)) continue;
            var actors = tileArray[p.x][p.y].Actors;
            foreach (var a in actors)
            {
                if (a.GetComponent<T>() != null)
                {
                    typedActors.Add(a.GetComponent<T>());
                }
            }
        }
        return typedActors.ToArray();
    }


    public bool AddActor(BoardActor actor, Vector2Int position)
    {
        if (!isValidTile(position)) return false;
        List<BoardActor> actors = new List<BoardActor>(tileArray[position.x][position.y].Actors);
        actors.Add(actor);
        tileArray[position.x][position.y].Actors = actors.ToArray();
        return true;
    }
    public bool RemoveActor(BoardActor actor, Vector2Int position)
    {
        if (!isValidTile(position)) return false;
        List<BoardActor> actors = new List<BoardActor>(tileArray[position.x][position.y].Actors);
        bool result = actors.Remove(actor);
        if (!result) return false;
        tileArray[position.x][position.y].Actors = actors.ToArray();

        return true;
    }
    public bool MoveActor(BoardActor actor, Vector2Int newPos)
    {
        Vector2Int prevPos = actor.BoardPosition;
        if (RemoveActor(actor, prevPos))
        {
            if (AddActor(actor, newPos))
            {
                return true;
            }
            AddActor(actor, prevPos);
        }
        return false;
    }


}
