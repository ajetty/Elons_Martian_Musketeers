using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterMovement : MonoBehaviour
    {
        public int move;
        public int speed;
        public float yOffset = 0.0f;

        public bool isTurn;
        
        public bool reachedDestination;

        public bool isDead;

        Vector3 velocity = new Vector3();

        //keeps track of distance between player/npc and target grid square (square we want to move to)
        Vector3 heading = new Vector3();

        //selectable grid squares for movement made with BFS - this is for selection for movement
        List<GridSquare> selectableGridSquares = new List<GridSquare>();

        //list of all grid squares in scene
        private GameObject[] gridSquares;

        //stores the grid squares that create our path to the target grid square - this is for the actual movement
        protected Stack<GridSquare> path = new Stack<GridSquare>();

        //tile that player/npc is currently on
        protected GridSquare currentGridSquare;

        public bool moving;

        public GridPlane gridPlane;

        // protected void Init(int move, int speed)
        // {
        //     this.move = move;
        //     this.speed = speed;
        //     gridSquares = GameObject.FindGameObjectsWithTag("GridSquare");
        // }
        
        protected virtual void Start()
        {
            this.reachedDestination = false;
            this.speed = 2;
            gridSquares = GameObject.FindGameObjectsWithTag("GridSquare");
        }

        //breadth first search algorithm for grid square selectable grid squares
        public virtual void FindSelectableTiles()
        {
            if (currentGridSquare == null) return;
            Queue<GridSquare> process = new Queue<GridSquare>();
            GridSquare startSquare = currentGridSquare;
            process.Enqueue(startSquare);
            HashSet<GridSquare> squaresVisited = new HashSet<GridSquare>();
            squaresVisited.Add(startSquare);
            Dictionary<GridSquare, int> distanceToSquare = new Dictionary<GridSquare, int>();
            distanceToSquare[startSquare] = 0;
            //current parent is null - we are at the head

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

        virtual public void MoveToGridSquare(GridSquare gridSquare)
        {
            path.Clear();
            gridSquare.target = true;
            moving = true;

            GridSquare next = gridSquare;
            while (next != null)
            {
                path.Push(next);
                next = next.parent;
            }
        }

        //player or npc moves along target path until path count is zero
        virtual public void Move()
        {
            if (path.Count > 0)
            {
                GridSquare g = path.Peek();
                //adding Vector3.up * yOffset so that we're not targeting the ground, but above the ground - keeps character from sinking into ground
                Vector3 target = g.transform.position + Vector3.up * yOffset;

                if (Vector3.Distance(transform.position, target) >= 0.05f)
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();

                    transform.forward = heading;
                    transform.position += velocity * Time.deltaTime;
                }
                else
                {
                    //reached target's center
                    GridSquare destination = path.Pop();
                    if (path.Count == 0)
                    {
                        SetGridSquare(destination);
                        RemoveSelectableGridSquares();
                        reachedDestination = true;
                        moving = false;
                    }
                }
            }
        }

        virtual public void SetGridSquare(GridSquare square)
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

        //find the nearest target by game object tag using rays
        public Player FindNearestTarget(String targetTag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

            float closestDistance = Mathf.Infinity; //start with the largest number since we want the smallest

            GameObject nearest = null;

            foreach (GameObject item in targets)
            {
                float currentDistance = Vector3.Distance(transform.position, item.transform.position);

                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    nearest = item;
                }
            }

            //Debug.Log("Calculated Target!: " + nearest.transform.position);
            return nearest.GetComponent<Player>();
        }

        //find a path to the target using A*
        public List<GridSquare> PathToTarget(GridSquare target)
        {
            Dictionary<GridSquare, GridSquare> cameFrom = new Dictionary<GridSquare, GridSquare>();
            Dictionary<GridSquare, int> costSoFar = new Dictionary<GridSquare, int>();

            var start = currentGridSquare;
            var goal = target;

            cameFrom[start] = null;
            costSoFar[start] = 0;

            var pathTaken = new SimplePriorityQueue<GridSquare>();

            pathTaken.Enqueue(start, 0);

            while (pathTaken.Count > 0)
            {
                var current = pathTaken.Dequeue();

                if (current.Equals(goal)) break;

                // This should be for each neighbor not for each gridSquare that exists --James
                foreach (var next in gridPlane.FindNeighbors(current.xCoordinate, current.zCoordinate))
                {
                    int newCost = costSoFar[current] + 1;

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        int heuristic = Mathf.Abs(next.xCoordinate - goal.xCoordinate) +
                                        Mathf.Abs(next.zCoordinate - goal.zCoordinate);
                        int priority = newCost + heuristic;
                        pathTaken.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            GridSquare newCurrent = goal;
            List<GridSquare> path = new List<GridSquare>();
            while (newCurrent != start)
            {
                path.Add(newCurrent);
                newCurrent = cameFrom[newCurrent];
            }

            //path.Reverse();

            return path;
        }

        private float Heuristic(Transform target)
        {
            return Mathf.Abs(transform.position.x - target.transform.position.x) +
                   Mathf.Abs(transform.position.z - target.transform.position.z);
        }
        
        public void SetMoveRange(int move)
        {
            this.move = move;
        }
    }
}