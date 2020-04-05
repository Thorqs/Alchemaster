using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    //This float gets passed to dijksta path to control the monsters speed.
    float currSpeed;

    Animator anim;
    DijkstraPath pathing;
    

    void Start()
    {
        anim = GetComponent<Animator>();
        pathing = GetComponentInParent<DijkstraPath>();
        currSpeed = pathing.speed;//pathing.speed is esentially the max speed
    }
    

    void Update()
    {
        //Variables for the animation controller, switches between idle and walking.
        if (pathing.speed > 0)
        {
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }

        pathing.currentSpeed = currSpeed;

        //ensures sprite faces towards the direction of movement. TravelVec is directed towards the next dijkstra node.
        transform.rotation = 
            Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(180, Vector3.forward) * (Vector3)pathing.travelVec);
    }

    public void UpdateSpeed(float slowFactor)
    {
        //Edits the max speed, and changes the current speed.
        if (pathing) { 
            pathing.speed *= (1 / slowFactor);
            currSpeed *= (1 / slowFactor);
        }
    }

    public void StartMoving()
    {
        //animation event function, sets to max speed.

        currSpeed = pathing.speed;
    }

    public void StopMoving()
    {
        //animation event function, sets to reduced speed.

        currSpeed = pathing.speed * 0.3f;
    }
}
