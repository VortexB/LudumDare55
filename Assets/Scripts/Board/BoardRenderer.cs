using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum RenderTypes
{
    Render2D,
    Redner2DIsometric
}

public abstract class BoardRenderer : MonoBehaviour
{
    protected Board Board { get => GetComponent<Board>(); }
    [SerializeField] protected Vector3 offset = new Vector2(0.5f, 0.5f);

    [SerializeField] public Vector2 cellSize = new Vector2(1f, 1f);

    public abstract Vector3 VisualWorldPosition(Vector2Int boardPosition);


    public abstract Vector2Int StrictBoardPosition(Vector3 worldPosition);


    public abstract Direction ActorDirection(Transform actor);

}
