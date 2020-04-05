using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPusher : MonoBehaviour {

    public PushDirection pushDirection;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster") {
            Debug.Log("Here");
            switch (pushDirection)
            {
                case PushDirection.Down:
                    collision.transform.parent.position += Vector3.down / 4;
                    Debug.Log("Down");
                    break;
                case PushDirection.Left:
                    collision.transform.parent.position += Vector3.left / 4;
                    break;
                case PushDirection.Right:
                    collision.transform.parent.position += Vector3.right / 4;
                    break;
                case PushDirection.Up:
                    collision.transform.parent.position += Vector3.right / 4;
                    break;
                default:
                    Debug.Log("Not implemented");
                    break;
            }
        }
        
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            Debug.Log("Here");
            switch (pushDirection)
            {
                case PushDirection.Down:
                    collision.transform.parent.position += Vector3.down / 10;
                    Debug.Log("Down");
                    break;
                default:
                    Debug.Log("Not implemented");
                    break;
            }
        }
    }

}

public enum PushDirection { Up, Down, Left, Right}
