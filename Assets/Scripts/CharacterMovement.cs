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

        protected void Init(int move, int speed)
        {
            this.move = move;
            this.speed = speed;
            gridSquares = GameObject.FindGameObjectsWithTag("GridSquare");
       
        }

        //breadth first search algorithm for grid square selectable tiles
        virtual public void FindSelectableTiles()
        {
            if (currentGridSquare == null) return;
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
                    foreach (GridSquare square in gridPlane.FindNeighbors(x,z))
                    {
                        if (! squaresVisited.Contains(square))
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

            GridSquare next = gridSquare;
            while (next != null)
            {
                path.Push(next);
                next = next.parent;
            }
        }

        //player or npc moves along target path until path count is zero
        public void Move()
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
                        moving = false;
                    }
                }
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
    }
}
