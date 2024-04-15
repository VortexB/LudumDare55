using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ActorRenderer:MonoBehaviour
{
    [SerializeField] public GameObject RendererObject;
    public abstract void Rotate(Direction dir);

}