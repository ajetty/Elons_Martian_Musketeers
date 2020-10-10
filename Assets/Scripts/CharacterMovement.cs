using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterMovement : MonoBehaviour
    {
        public int move;
        public int speed;
        public float yOffset = 0.5f;

        public bool reachedDestination;

        Vector3 velocity = new Vector3();

        //keeps track of distance between player/npc and target grid square (square we want to move to)
        Vector3 heading = new Vector3();

        //selectable grid squares for movement made with BFS - this is for selection for movement
        List<GridSquare> selectableGridSquares = new List<GridSquare>();

        //list of all grid squares in scene
        private GameObject[] gridSquares;

        //stores the grid squares that create our path to the target grid square - this is for the actual movement
        Stack<GridSquare> path = new Stack<GridSquare>();

        //tile that player/npc is currently on
        protected GridSquare currentGridSquare;

        public bool moving;

        public GridPlane gridPlane;

        /*
        public void Init(int move, int speed)
        {
            this.move = move;
            this.speed = speed;
            gridSquares = GameObject.FindGameObjectsWithTag("GridSquare");

        }
        */

        void Start()
        {
            this.reachedDestination = false;
            this.speed = 2;
            gridSquares = GameObject.FindGameObjectsWithTag("GridSquare");
        }

        //breadth first search algorithm for grid square selectable tiles
        virtual public void FindSelectableTiles()
        {
            if (currentGridSquare == null)
            {
                Debug.Log("Player has current square null.");
                return;
            }
            Queue<GridSquare> process = new Queue<GridSquare>();
            GridSquare startSquare = currentGridSquare;
            process.Enqueue(startSquare);
            HashSet<GridSquare> squaresVisited = new HashSet<GridSquare>();
            //currentGridSquare.visited = true;
            squaresVisited.Add(startSquare);
            Dictionary<GridSquare, int> distanceToSquare = new Dictionary<GridSquare, int>();
            Dictionary<GridSquare, GridSquare> reachedFrom = new Dictionary<GridSquare, GridSquare>();
            distanceToSquare[startSquare] = 0;
            //current parent is null

            while (process.Count > 0)
            {
                GridSquare pathCurrentSquare = process.Dequeue();

                selectableGridSquares.Add(pathCurrentSquare);
                pathCurrentSquare.selectable = true;
                int distance = distanceToSquare[pathCurrentSquare];

                if (distance < move)
                {
                    //foreach (GridSquare square in pathCurrentSquare.AdjacencyList)
                    int x = pathCurrentSquare.xCoordinate;
                    int z = pathCurrentSquare.zCoordinate;
                    foreach (GridSquare square in gridPlane.FindNeighbors(x, z))
                    {
                        if (!squaresVisited.Contains(square))
                        {
                            square.parent = pathCurrentSquare;
                            //square.visited = true;
                            squaresVisited.Add(square);
                            //square.distance = 1 + pathCurrentSquare.distance;
                            distanceToSquare[square] = 1 + distance;
                            process.Enqueue(square);
                        }
                    }
                }
            }
        }

        public void MoveToGridSquare(GridSquare gridSquare)
        {
            path.Clear();
            gridSquare.target = true;
            moving = true;
            Debug.Log("Moving was set to true.");

            GridSquare next = gridSquare;
            while (next != null)
            {
                path.Push(next);
                Debug.Log("Path has new square pushed. Coordinates: " + next.xCoordinate + ", " + next.zCoordinate);
                next = next.parent;
            }
        }

        //player or npc moves along target path until path count is zero
        public void Move()
        {
            if (path.Count > 0)
            {
                Debug.Log("Path count is greater than 0.");
                GridSquare g = path.Peek();
                //adding Vector3.up * yOffset so that we're not targeting the ground, but above the ground - keeps character from sinking into ground
                Vector3 target = g.transform.position + Vector3.up * yOffset;

                if (Vector3.Distance(transform.position, target) >= 0.05f)
                {
                    Debug.Log("Distance is now being calculated.");
                    CalculateHeading(target);
                    SetHorizontalVelocity();

                    transform.forward = heading;
                    transform.position += velocity * Time.deltaTime;
                }
                else
                {
                    //reached target's center
                    GridSquare destination = path.Pop();
                    Debug.Log("Path has been popped.");
                    Debug.Log("Destination coordinates: " + destination.xCoordinate + ", " + destination.zCoordinate);
                    //SetGridSquare(destination);

                    if (path.Count == 0)
                    {
                        Debug.Log("Path count is 0 and now we remove selectable grid squares.");
                        SetGridSquare(destination);
                        RemoveSelectableGridSquares();
                        reachedDestination = true;
                        moving = false;
                    }
                }
            }
            else
            {
                Debug.Log("Path count is 0.");
            }
        }

        public virtual void SetGridSquare(GridSquare square)
        {

            transform.position = square.transform.position + Vector3.up * yOffset;
            currentGridSquare = square;
        }

        public void RemoveSelectableGridSquares()
        {
            foreach (GridSquare square in selectableGridSquares)
            {
                square.Reset();
            }

            selectableGridSquares.Clear();
        }

        //calculate distance between target and npc/player
        public void CalculateHeading(Vector3 target)
        {
            //normalized (magnitude of 1) distance between player/npc and target
            heading = target - transform.position;
            heading.Normalize();
        }

        public void SetHorizontalVelocity()
        {
            //have velocity equal the forward direction we're heading times speed
            velocity = heading * speed;
        }

        public void SetMoveRange(int move)
        {
            this.move = move;
        }
    }
}
