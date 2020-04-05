/*
    Based on AStarPathfinder and Chase by Lech Szymanski and FloodSpill by Paweł Ślusarczyk
        AStarPathfinder link: https://altitude.otago.ac.nz/cosc360/Pathfinding
        FloodSpill link: https://github.com/azsdaja/FloodSpill-CSharp
*/

using UnityEngine;

public class DijkstraPath : MonoBehaviour
{
    // public vars to be set in Inspector
    public GameObject mapObject = null;
    public float speed;

    [HideInInspector]
    public Vector2 travelVec;
    public float currentSpeed;

    // private vars
    private DijkstraMap map = null;

    public int curMapY;
    public int curMapX;

    void Start()
    {
        currentSpeed = speed;
        // Get reference to the map attached to the mapObject
        if(mapObject == null)
        {
            Debug.LogError("Pathfinding map object not initialised!");
            return;
        }

        map = mapObject.GetComponent<DijkstraMap>();
    }

    void Update()
    {
        if(map == null)
        {
            Debug.LogError("Pathfinding map object is missing DijkstraMap component!");
            return;
        }

        // compute the distance to travel this frame
        float distAllowed = currentSpeed * Time.deltaTime;

        // Travel downhill
        // get current position
        Vector2 curPos = transform.position;
        curMapX = (int) Mathf.Round((curPos.x-map.MapStartPosition.x)/map.Tilesize);
        curMapY = (int) Mathf.Round((curPos.y-map.MapStartPosition.y)/map.Tilesize);
        //Debug.Log(curMapX + ", " + curMapY);

        if (curMapX < 5)
        {
            //transform.Translate(Vector3.right * speed * Time.deltaTime);
            Vector3 oldPos = transform.position;
            transform.position += Vector3.right * speed * Time.deltaTime;
            travelVec = transform.position - oldPos;
            return;
        }
        if (curMapX > 90)
        {
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            Vector3 oldPos = transform.position;
            transform.position += Vector3.left * speed * Time.deltaTime;
            travelVec = transform.position - oldPos;
            return;
        }
        if (curMapY < 0)
        {
            //transform.Translate(Vector3.up * speed * Time.deltaTime);
            Vector3 oldPos = transform.position;
            transform.position += Vector3.up * speed * Time.deltaTime;
            travelVec = transform.position - oldPos;
            return;
        }
        if (curMapY > 48)
        {
            //transform.Translate(Vector3.down * speed * Time.deltaTime);
            Vector3 oldPos = transform.position;
            transform.position += Vector3.down * speed * Time.deltaTime;
            travelVec = transform.position - oldPos;
            return;
        }

        if(map.Map[curMapX, curMapY] == -1)
        {
            Vector3.MoveTowards(transform.position, new Vector3(0, 0, 0), speed);
            return;
        }
        // Find next position (left, right, up, down for now)
        Vector2 nextPos = curPos;
        int nextVal = map.Map[curMapX, curMapY];
        if (curMapX - 1 >= 0 && map.Map[curMapX - 1, curMapY] < nextVal) // left
        {
            nextPos = new Vector2(curPos.x - map.Tilesize, curPos.y);
            nextVal = map.Map[curMapX-1, curMapY];

            /* Experimental diagonal pathing stuff */
            if (curMapY + 1 < map.height && map.Map[curMapX - 1, curMapY + 1] < nextVal) // up-left
            {
                nextPos = new Vector2(curPos.x - map.Tilesize, curPos.y + map.Tilesize);
                nextVal = map.Map[curMapX - 1, curMapY + 1];
            }
            if (curMapY - 1 >= 0 && map.Map[curMapX - 1, curMapY - 1] < nextVal) // down-left
            {
                nextPos = new Vector2(curPos.x - map.Tilesize, curPos.y - map.Tilesize);
                nextVal = map.Map[curMapX - 1, curMapY - 1];
            }
        }
        if(curMapX + 1 < map.width && map.Map[curMapX + 1, curMapY] < nextVal) // right
        {
            nextPos = new Vector2(curPos.x + map.Tilesize, curPos.y);
            nextVal = map.Map[curMapX + 1, curMapY];

            /* Experimental diagonal pathing stuff */
            if (curMapY + 1 < map.height && map.Map[curMapX + 1, curMapY + 1] < nextVal) // up-right
            {
                nextPos = new Vector2(curPos.x  + map.Tilesize, curPos.y + map.Tilesize);
                nextVal = map.Map[curMapX + 1, curMapY + 1];
            }
            if (curMapY - 1 >= 0 && map.Map[curMapX + 1, curMapY - 1] < nextVal) // down-right
            {
                nextPos = new Vector2(curPos.x + map.Tilesize, curPos.y - map.Tilesize);
                nextVal = map.Map[curMapX + 1, curMapY - 1];
            }
        }
        if (curMapY + 1 < map.height && map.Map[curMapX, curMapY + 1] < nextVal) // up
        {
            nextPos = new Vector2(curPos.x, curPos.y + map.Tilesize);
            nextVal = map.Map[curMapX, curMapY + 1];

            /* Experimental diagonal pathing stuff */
            if(curMapX + 1 < map.width && map.Map[curMapX + 1, curMapY + 1] < nextVal) // up-right
            {
                nextPos = new Vector2(curPos.x + map.Tilesize, curPos.y + map.Tilesize);
                nextVal = map.Map[curMapX + 1, curMapY + 1];
            }
            if (curMapX - 1 >= 0 && map.Map[curMapX - 1, curMapY + 1] < nextVal) // up-left
            {
                nextPos = new Vector2(curPos.x - map.Tilesize, curPos.y + map.Tilesize);
                nextVal = map.Map[curMapX - 1, curMapY + 1];
            }
        }
        if (curMapY - 1 >= 0 && map.Map[curMapX, curMapY - 1] < nextVal) // down
        {
            nextPos = new Vector2(curPos.x, curPos.y - map.Tilesize);
            nextVal = map.Map[curMapX, curMapY - 1];

            /* Experimental diagonal pathing stuff */
            if(curMapX + 1 < map.width && map.Map[curMapX + 1, curMapY - 1] < nextVal) // down-right
            {
                nextPos = new Vector2(curPos.x + map.Tilesize, curPos.y - map.Tilesize);
                nextVal = map.Map[curMapX + 1, curMapY - 1];
            }
            if (curMapX - 1 >= 0 && map.Map[curMapX - 1, curMapY - 1] < nextVal) // down-left
            {
                nextPos = new Vector2(curPos.x - map.Tilesize, curPos.y - map.Tilesize);
                nextVal = map.Map[curMapX - 1, curMapY - 1];
            }
        }
        // Get the travel vector for the next move
        travelVec = nextPos-curPos;
        // compute travel distance in the next move
        float travelDist = travelVec.magnitude;
        Vector2 nextMove;

        // If the travel distance is less than the total to travel
        // execute entire move
        if(travelDist < distAllowed)
        {
            nextMove = nextPos;
        }
        else
        {
            // if the travel distance is more than the total to travel
            // move along the travel vector, but only a bit
            nextMove = curPos+travelVec.normalized*distAllowed;
            travelDist = distAllowed;
        }

        // Get the next position based on the next move
        Vector3 nextPosition = new Vector3(nextMove.x, nextMove.y, transform.position.z);

        // update remaining distance to travel
        transform.position = nextPosition;
        distAllowed -= travelDist;
    }
}
