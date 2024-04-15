using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardTile
{
    BoardActor[] actors;
    public BoardActor[] Actors { get => actors; set => actors = value; }
    public BoardTile()
    {
        actors = new BoardActor[0];
    }

}

