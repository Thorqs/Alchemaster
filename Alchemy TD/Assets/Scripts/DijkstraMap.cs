/*
    Based on AStarGrid by Lech Szymanski and FloodSpill by Paweł Ślusarczyk
        AStarGrid link: https://altitude.otago.ac.nz/cosc360/Pathfinding
        FloodSpill link: https://github.com/azsdaja/FloodSpill-CSharp
*/

using UnityEngine;
using System.Collections.Generic;

public class DijkstraNode
{
    public float x;
    public float y;
    public int goalDistance;


    public DijkstraNode(float nodeX, float nodeY, int goalDistance)
    {
        this.x = nodeX;
        this.y = nodeY;
        this.goalDistance = goalDistance;
    }
}

public class DijkstraMap : MonoBehaviour
{
    // public grid specifications (initialise in Inspector)
    public float Tilesize = 1f; // default value
    public Vector2 MapStartPosition;
    public Vector2 MapEndPosition;
    public List<string> DisallowedTags;
    public GameObject target = null;

    // private variables (don't touch these in inspector, though)
    public int width; // height and width measured in tiles
    public int height;
    public int [,] Map;

    [HideInInspector]
    public Dictionary<string, bool> placedStructureLocations;

    private int numBlockedNodes;
    public GameObject newWall = null;

    // public vars for testing
    // public bool DrawMapInDebug = false; // false by default

    // Executed when this comes into being
    void Start()
    {
        placedStructureLocations = new Dictionary<string, bool>();

        //force positive tile sizes
        if(Tilesize <= 0)
        {
            Tilesize = 1;
        }

        //  vvv Creates a new map so that there is never a null map vvv

        // Find start and end positions
        float startX = MapStartPosition.x;
        float startY = MapStartPosition.y;
        float endX = MapEndPosition.x;
        float endY = MapEndPosition.y;

        // Find tile width and height
        width = (int)Mathf.Floor((endX - startX) / Tilesize);
        height = (int)Mathf.Floor((endY - startY) / Tilesize);

        Map = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Map[i, j] = -1;
            }
        }
        //'Empty' map created.

        CalcMap();
    }


    public bool CheckPlaced(float x, float y)
    {
        string key = x + "," + y;
        if (placedStructureLocations.ContainsKey(key))
        {
            return false;
        }
        else
        {
            placedStructureLocations.Add(key, true);
            return true;
        }
    }

    public void RemoveStructureReference(float x, float y)
    {
        string key = x + "," + y;
        placedStructureLocations.Remove(key);
    }


    private bool CheckForDisallowedTag(Transform t)
    {
        if (DisallowedTags.Contains(t.tag))
        {
            return true;
        }
        else if (t.parent != null)
        {
            return CheckForDisallowedTag(t.parent.transform);
        }
        return false;
    }

    /// <summary>
    /// Calls CalcMap() when a wall is placed.
    /// </summary>
    /// <param name="wall">The wall that called for a new map.</param>
    public void CalcMap(GameObject wall)
    {
        newWall = wall;
        if (!CheckPlaced(wall.transform.position.x, wall.transform.position.y))
        {
            Debug.Log("Already a wall here!");
            newWall.GetComponent<Sellable>().isInvalid = true;
            Destroy(newWall.gameObject);
            return;
        }
        CalcMap();
    }

    // Creates a grid map based on the start and end position of the grid
    // need to add colour-coded visualization
    // greener closer to goal, redder farther.
    public void CalcMap()
    {
        // Find start positions
        float startX = MapStartPosition.x;
        float startY = MapStartPosition.y;

        // Create 2D array representing the grid nodes
        int [,] newMap = new int [width, height];
        // then fill it with values indicating unvisited (negative, here)
        for(int i = 0; i < width; i ++)
        {
            for(int j = 0; j < height; j++)
            {
                newMap[i, j] = -1;
            }
        }

        DijkstraNode curNode = new DijkstraNode(target.transform.position.x, target.transform.position.y, 0);
        Vector2 boxCastSize = new Vector2(Tilesize/2, Tilesize/2);

        // Create nodes in the map starting from origin
        // We want to use a queue for this
        int xPos;
        int yPos;
        Queue<DijkstraNode> route = new Queue<DijkstraNode>(height+width);
        route.Enqueue(curNode);
        while(route.Count != 0)
        {
            // Get current node to check from
            curNode = route.Dequeue();


            // Get its relative location in our Map
            xPos = (int) Mathf.Floor((curNode.x-startX)/Tilesize);
            yPos = (int) Mathf.Floor((curNode.y-startY)/Tilesize);
            //Debug.Log("xPos: " + xPos + ", yPos: " + yPos);
            newMap[xPos, yPos] = curNode.goalDistance;

            // Check if it's a valid location
            // Ray cast to see if there is a 2D collider there
            Vector2 castVector = new Vector2(curNode.x, curNode.y);
            RaycastHit2D[] hit2D = Physics2D.BoxCastAll(castVector, boxCastSize, 0f, Vector2.left, 0f);
            // For any 2D collider found, check if the associated object has a disallowed tag
            foreach (RaycastHit2D h in hit2D)
            {
                if(CheckForDisallowedTag(h.transform))
                {
                    newMap[xPos, yPos] = 999; // some really big number to make it never path through
                }
            }

            // Add neightbours if they're on the grid and unvisited (cell value < 0)
            // left, up, right, down for now
            if(newMap[xPos, yPos] != 999) // we aren't at a wall
            {
                if((xPos-1 >= 0) && newMap[xPos-1, yPos] < 0) // left
                {
                    route.Enqueue(new DijkstraNode(curNode.x-Tilesize, curNode.y, curNode.goalDistance+1));
                    newMap[xPos-1, yPos] = 900;
                }

                if(xPos+1 < width && newMap[xPos+1, yPos] < 0) // right
                {
                    route.Enqueue(new DijkstraNode(curNode.x+Tilesize, curNode.y, curNode.goalDistance+1));
                    newMap[xPos+1, yPos] = 900;
                }

                if(yPos+1 < height && newMap[xPos, yPos+1] < 0) // up
                {
                    route.Enqueue(new DijkstraNode(curNode.x, curNode.y+Tilesize, curNode.goalDistance+1));
                    newMap[xPos, yPos+1] = 900;
                }

                if(yPos-1 >= 0 && newMap[xPos, yPos-1] < 0) // down
                {
                    route.Enqueue(new DijkstraNode(curNode.x, curNode.y-Tilesize, curNode.goalDistance+1));
                    newMap[xPos, yPos-1] = 900;
                }
            }
        }

        //Check if a blockage has been created
        for (int r = 0; r < width; r++)
        {
            for (int c = 0; c < height; c++)
            {
                if (newMap[r, c] < 0 && Map[r, c] >= 0 && Map[r, c] != 999)
                {
                    Debug.Log(r + " " + c);
                    OnBlockageCreated();
                    return;
                }
            }
        }

        //If no blockage has been created, then the new map can be used.
        Map = newMap;
        Debug.Log("Map Updated!");

        //Debug.Log(CreateMatrixString());

    }

    /// <summary>
    /// Called when a placed wall creates a blockage.
    /// </summary>
    public void OnBlockageCreated()
    {
        Debug.Log("Path blockage created, cannot place wall here!");
        Destroy(newWall);
    }

    /// <summary>
    /// Creates a string representation of the map.
    /// </summary>
    /// <returns>The map in string form.</returns>
    private string CreateMatrixString()
    {
        string matrixRep = "";
        for (int j = height - 1; j >= 0; j--)
        {
            for (int i = 0; i < width; i++)
            {
                if (Map[i, j] < 100)
                {
                    matrixRep += "  ";
                    if (Map[i, j] < 10)
                    {
                        matrixRep += "  ";
                    }
                }
                matrixRep += Map[i, j] + " ";
            }
            matrixRep += "\n";
        }
        return matrixRep;
    }
}
