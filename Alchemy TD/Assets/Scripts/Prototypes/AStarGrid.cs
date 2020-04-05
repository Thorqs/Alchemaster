/*
	Created by: Lech Szymanski
				lechszym@cs.otago.ac.nz
				Dec 29, 2015			
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/* This file constitutes to core of the pathfinidng library.  A* pathfinidng algorithm
 * operates on a weighted graph of nodes, where nodes correspond to different positions
 * on some grid travelled through and the edges are the paths between the nodes with
 * associated cost.
 * 
 * This library implements the classes for the edges and nodes as well as the
 * graph combining those nodes and edges into a grid spanning a 2D surface.
 */


/* Class representing an edge in the pahtfinding graph */
public class AStarEdge {
	private AStarNode nodeLeft;		//Node at one end of the edge
	private AStarNode nodeRight;	//Node at the other end of the edge
	public float cost;				//Weight of the connection

	/* Creates an edge between two nodes and assigns a weight */
	public AStarEdge(AStarNode nodeLeft, AStarNode nodeRight, float cost) {
		this.nodeLeft = nodeLeft;
		this.nodeRight = nodeRight;
		this.cost = cost;
	}

	/* Returns the other node of the edge
	 * 
	 * param: node - one node attached to the edge
	 * 
	 * returns: AStarNode - the other node
	 */
	public AStarNode getNeighbour(AStarNode node) {
		if (node == nodeLeft) {
			return nodeRight;
		} else if (node == nodeRight) {
			return nodeLeft;
		} else {
			return null;
		}
	}

}

/* Class representing a node in the pathfinding graph */
public class AStarNode
{
	public Vector2 position; //Position of the node in the scene
	public bool walkable    = true;	//Identifies whether object can traverse through the node or not

	//Path variables
	public AStarNode prev;	//Used for forming paths whe calculating shortest distanfde - 
							//points to the previous node in the path
	public float cost = 0f;	//Total cost of the path
	public float guess = 0f; //Guess value from the heuristic to the destination

	//List of all the edges going out from this node
	public List<AStarEdge> neighbours;

	/* Creates a new node at a given position
	 */
	public AStarNode(Vector2 position, bool walkable = true)
	{
		this.position = position;
		this.walkable = walkable;
		this.neighbours = new List<AStarEdge> ();
		this.reset ();
	}

	/* Resets path to null and cost to 0
	 */
	public void reset() {
		this.cost = 0f;
		this.prev = null;
	}

	/* Checks for connection to other node
	*
	* param: node - other node
	* 
	* returns: bool - true if connected to node, false if not
	*/
	public bool isConnected(AStarNode node) {

		// Check if the node is on the list of this node's
		// neighbours
		foreach (AStarEdge e in neighbours) {
			if(e.getNeighbour(this) == node) {
				return true;
			}
		}

		return false;
	}

	/* Connects a node to another (by creating an edge between them)
	 * 
	 * param: node - node to connect
	 * param: cost - the cost of the path between two nodes
	 */
	public void connect(AStarNode node, float cost) {
		// Create a weighted edge between this node and the specified node
		AStarEdge edge = new AStarEdge (this, node, cost);
		// Add the edge to this node's list of edges
		this.neighbours.Add (edge);
		// Add the edge to the other node's list of edges
		node.neighbours.Add (edge);
	}

	/* Converts a reverse path (given by links from nodes to previous
	 * nodes) to a forward path, which is a list of positions that object
	 * should travel through
	 * 
	 * returns: List<Vector2> - series of positions to mark the path to follow
	 */
	public List<Vector2> getPath() {
		List<Vector2> path;
		if (prev == null) {
			path = new List<Vector2> ();
		} else {
			path = prev.getPath ();
			path.Add (position);
		}
		return path;
	}

	/* Command useful for debugging, prints path to console*/
	public string DebugShowPath() {
		if (prev == null) {
			return position.ToString ();
		} else {
			return prev.DebugShowPath() + "->" + position.ToString();
		}
	}


}

/* Class used for creating a grid, a graph of nodes and connections that
 * covers 2D surface, over which game objects can travel using A* pathfinding.
 *
 * Provides a method for finding shortest path between two positions. 
 */
public class AStarGrid : MonoBehaviour {
	public float Tilesize = 1f;	//Size of the grid tile
								//(initialise via the Inspector Panel)		

	public Vector2 MapStartPosition;	//Bottom-left corner of the grid
										//(initialise via the Inspector Panel)	
	public Vector2 MapEndPosition;		//Top-right corner of the grid
										//(initialise via the Inspector Panel)	

	private AStarNode[,] Map = null;	//List of nodes in the grid

	public List<string> DisallowedTags;	//List of tags for game object marking non-walkable tiles

	public bool DrawGridInEditor = false;	//Flag for drawing the grid in the "Scene" view
											//(set via the Inspector Panel)	

	private int width;	//widht and height of the grid (in number of tiles)
	private int height;

#if UNITY_EDITOR
	// List of nodes examine to display - for debugging only
	private List<AStarNode> gizmos;
#endif

	void Start()
	{
		//Tilesize must be positive
		if (Tilesize <= 0)
		{
			Tilesize = 1;
		}
#if UNITY_EDITOR
		// Initialise list of examined nodes for display - debugging only */
		gizmos = new List<AStarNode> ();
#endif
		//Create the grid map
		CreateMap();
	}


	private bool CheckForDisallowedTag(Transform t) {
		if (DisallowedTags.Contains (t.tag)) {
			return true;
		} else if (t.parent != null) {
			return CheckForDisallowedTag (t.parent.transform);
		}
		return false;
	}

	/* Creates grid map based on the start and end position of the grid */
	private void CreateMap()
	{
		//Find positions for start and end of map
		float startX = MapStartPosition.x;
		float startY = MapStartPosition.y;
		float endX = MapEndPosition.x;
		float endY = MapEndPosition.y;
		
		//Find tile width and height
		width = (int) Mathf.Floor ( (endX - startX) / Tilesize);
		height = (int) Mathf.Floor( (endY - startY) / Tilesize);
		
		//Create double array representing the nodes in the grid
		Map = new AStarNode[width, height];

		Vector2 nodePosition = new Vector2();
		Vector2 boxCastSize = new Vector2 (Tilesize / 2, Tilesize / 2);
		
		//Create nodes in the map
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				//Compute nodes position
				nodePosition.x = startX + (i * Tilesize);
				nodePosition.y = startY + (j * Tilesize);

				//Create a node at that position
				Map[i, j] = new AStarNode(nodePosition, true); 

				//Ray cast to see if there is a 2D collider at that position 
				RaycastHit2D[] hit2D = Physics2D.BoxCastAll(nodePosition, boxCastSize, 0f, Vector2.left, 0f);

				//For any 2D collider found at the position, check if the associated game object
				//is tagged with one of the disallowed tags
				foreach (RaycastHit2D h in hit2D)
				{
					//If tagged with a tag on the disallowed tags list, make the node
					//non-walkable
					if (CheckForDisallowedTag(h.transform))
					{
						Map[i, j].walkable = false;
						break;
					}
				}
			}
		}

		//Given the 2D array of nodes, connect the neighbouring tiles.  There can be a maximum of
		//eight connections from a given node: up and down, left and right,  left-down, left-up, right-down and right-up
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				//Get the node from the map
				AStarNode nodeLeft = Map[i,j];

				//Check if the node is walkable
				if( !nodeLeft.walkable) {
					continue;
				}

				//Cycle through all the neighbours of the node (based on 2D arrangement)
				for( int ni = i-1; ni<=i+1; ni++) {
					if( ni < 0 || ni >= width) {
						continue;
					}
					for( int nj = j-1; nj<=j+1; nj++) {
						if( nj < 0 || nj >= height) {
							continue;
						}

						AStarNode nodeRight = Map[ni,nj];

						//Do not connect to self
						if(nodeRight == nodeLeft) {
							continue;
						}

						//If the neighbour is not walkable, do not connect
						if(!nodeRight.walkable) {
							continue;
						}

						//Do not connect the same node twice
						if(nodeLeft.isConnected(nodeRight)) {
							continue;
						}
						//The weight of the connection is the Eucledean distance between
						//the nodes
						float cost = Vector2.Distance (nodeLeft.position,nodeRight.position);

						//Connect the nodes
						nodeLeft.connect(nodeRight,cost);
					}
				}
				//Sort the game nodes by cost - this will speed up pathfinding later on
				nodeLeft.neighbours = nodeLeft.neighbours.OrderBy(x => x.cost).ToList();
			}
		}
	}

	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		if (DrawGridInEditor == true && Map != null) {
			// Draw grid lines- debugging only */
			DrawGridLines ();
		}
#endif
	}

	/* Finds the closest node to a position.  This is necessary for finding nodes 
	 * corresponding to an arbitarry starting position
	 * 
	 * param: position - 2D position for which we want to find the closest walkable node
	 * returns: AStarNode - closest node, null if no walkable node found
	 */
	public AStarNode closestNode(Vector2 position) {
		//Compute the grid tile the position corresponds to
		int i = (int)Mathf.Round ((position.x - MapStartPosition.x) / Tilesize);
		int j = (int)Mathf.Round ((position.y - MapStartPosition.y) / Tilesize);
		
		if (i >= 0 && i < width && j>=0 && j < height && Map[i,j].walkable) {
			return Map[i,j];
		}

		List<AStarNode> nodes = new List<AStarNode> ();

		//Create a list of allwalkable  nodes around the position
		for (int ioffset=-1; ioffset<=1; ioffset++) {
			for (int joffset=-1; joffset<=1; joffset++) {
				int i2 = i+ioffset;
				int j2 = j+joffset;
				if (i2 >= 0 && i2 < width && j2>=0 && j2 < height && Map[i2,j2].walkable) {
					// Use the node's guess variable to store the distance from position
					// to the node
					Map[i2,j2].guess = Vector2.Distance (position, Map[i2,j2].position);
					nodes.Add (Map[i2,j2]);
				}
			}
		}

		// Sort the list of nodes around the position based on the guess (distance).  Pick the
		// one with the shortest distance.  That's the closest node
		if (nodes.Count > 0) {
			nodes = nodes.OrderBy(x => x.guess).ToList();
			return nodes.First ();
		}

		return null;
	}

	/* Find the shortest path from a start to target position
	 * 
	 * param: start - start position
	 * param: target - end position
	 * returns: List<Vector2> - list of positions corresponding to grid nodes
	 * of the shortest path between start and target, null if start or target
	 * not on the walkable part of the grid
	 * 
	 * This method implements the A* algorithm.  It maintains a list of open nodes (to
	 * which path from starting position has been found).  At the beginning, only the 
	 * starting node is added to the list of open nodes.  In each iteration, connections from
	 * all open nodes are considered, and one that gives the smallest cost + guess is the next
	 * node chosen for travelling through.  If the next node is not on open list, it gets added
	 * to the open list.  This continues until the next node is the target node.
	 * 
	 * The paths are maintained by saving recording in each node the prevoius node in the path.  
	 * Tracing path of previous nodes from target to start node allows reconstruction of the path taken.
	 * 
	 */
	public List<Vector2> ShortestPath(Vector2 start, Vector2 target) {

		// List of nodes that have a path to from the start node
		List<AStarNode> open = new List<AStarNode> ();
		// List of nodes that are dead ends - cannot take a new path from
		// those nodes to the target
		List<AStarNode> closed = new List<AStarNode> ();

		//Find the closest node to the start position
		AStarNode startNode = closestNode (start);
		//Find the closest node to the target position
		AStarNode targetNode = closestNode (target);

		if (startNode==null || !startNode.walkable) {
			return null;
		}

		if (targetNode == null || !targetNode.walkable) {
			return null;
		}

		if (startNode == targetNode) {
			return null;
		}

		//Reset the start node cost - clears the path links from the prevoius run of A*
		startNode.reset ();
		//Compute the guess value - Euclidean distance from start to target
		startNode.guess = Vector2.Distance (startNode.position, targetNode.position);
		//Add the start node to the list of nodes to consider for taking on the next
		//iteration
		open.Add (startNode);

#if UNITY_EDITOR
		// Create a list of examined nodes - debugging only */
		gizmos = new List<AStarNode>();
		gizmos.Add(startNode);
#endif

		//Keep traversing the grid until get to the target node
		while(true) {
			//Sort the list of open nodes according to path cost+guess - this assures 
			// that open nodes with smallest total cost will be considered first
			open = open.OrderBy(x => x.cost + x.guess).ToList();

			List<AStarNode> removeFromOpen = new List<AStarNode>();

			AStarNode nextNode = null;

			//Examine connections of each open node.
			foreach(AStarNode node in open) {

				AStarEdge nextEdge = null;

				//For each connection of the open node...
				foreach(AStarEdge edge in node.neighbours) {
					AStarNode neighbour = edge.getNeighbour(node);

					//Check if there is a new path to consider.
					if(!closed.Contains (neighbour)) {
						if(!open.Contains (neighbour)) {
							nextEdge = edge;
							break;
						} else {
							float newGuess = Vector2.Distance (neighbour.position,targetNode.position);

							// If there is a new path to consider, compute its cost - save the edge
							// that gives new path with the lowest cost
							float newCost = node.cost+edge.cost;
							if((newCost + newGuess < neighbour.cost + neighbour.guess)) {
								nextEdge = edge;
								break;
							}
						}
					}


				}

				//If no new paths were found, move the node from the opne nodes list
				//to the closed list (it won't be considered for new paths)
				if(nextEdge == null) {
					removeFromOpen.Add (node);
					closed.Add (node);
				} else {

					//If a new path has been found, take it, and update the cost in the
					//node
					nextNode = nextEdge.getNeighbour(node);
#if UNITY_EDITOR
					/* Save new noded to the list of examined nodes - debugging only */
					if(!gizmos.Contains(nextNode)) {
						gizmos.Add(nextNode);
					}
#endif

					float newGuess = Vector2.Distance (nextNode.position,targetNode.position);
					float newCost = node.cost+nextEdge.cost;
					nextNode.prev = node;
					nextNode.cost = newCost;
					nextNode.guess = newGuess;

					//If the new path takes us to the target node, we are done - return
					//the path taken to this node
					if(nextNode == targetNode) {
						return targetNode.getPath();
					} else {
						break;
					}
				}
			}

			if(nextNode == null) {
				break;
			}

			//If the next node is not on open nodes list, add it to open
			//nodes list
			if(!open.Contains (nextNode)) {
				open.Add (nextNode);
			}

			//Clean up - remove any nodes that have been added to the closed
			//node list from te open nodes list
			foreach(AStarNode node in removeFromOpen) {
				open.Remove (node);
			}
		}

		return null;
	}

#if UNITY_EDITOR
	/* Draws the grid in the "Scene" view - for debugging purposes */
	void DrawGridLines()
	{
			for (int i = 0; i < Map.GetLength(1); i++)
			{
				for (int j = 0; j < Map.GetLength(0); j++)
				{
					if (!Map[j, i].walkable)
						continue;


					for (int y = i - 1; y < i + 2; y++)
					{
						for (int x = j - 1; x < j + 2; x++)
						{
							if (y < 0 || x < 0 || y >= Map.GetLength(1) || x >= Map.GetLength(0))
								continue;

							if (!Map[x, y].walkable)
								continue;

							Vector3 start = new Vector3(Map[j, i].position.x, Map[j, i].position.y, transform.position.z);
							Vector3 end = new Vector3(Map[x, y].position.x, Map[x, y].position.y, transform.position.z);

							UnityEngine.Debug.DrawLine(start, end, Color.green);
						}
					}
				}
			}
	}

	/* Draws nodes examined - debugging only */
	/* param: gimzoColor - colour of the nodes
	*/
	public void DrawNodesConsidered(Color gizmoColor) {
		foreach (AStarNode node in gizmos) {
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere ( new Vector3(node.position.x, node.position.y, transform.position.z), 0.1f);
		}
	}
#endif

}
