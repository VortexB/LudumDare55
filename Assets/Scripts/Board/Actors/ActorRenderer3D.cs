using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActorRenderer3D:ActorRenderer
{

    public override void Rotate(Direction dir)
    {
        // Transform spriteTransform = RendererObject.transform;
        // switch(dir){
        //     case Direction.Up:
        //         spriteTransform.rotation = Quaternion.Euler(Vector3.zero);
        //         spriteTransform.Rotate(new Vector3(0,0,0));
        //         break;
        //     case Direction.Right:
        //         spriteTransform.rotation = Quaternion.Euler(Vector3.zero);
        //         spriteTransform.Rotate(new Vector3(0,0,90));
        //         break;
        //     case Direction.Down:
        //         spriteTransform.rotation = Quaternion.Euler(Vector3.zero);
        //         spriteTransform.Rotate(new Vector3(0,0,180));
        //         break;
        //     case Direction.Left:
        //         spriteTransform.rotation = Quaternion.Euler(Vector3.zero);
        //         spriteTransform.Rotate(new Vector3(0,0,270));
        //         break;
        // }
    }
}