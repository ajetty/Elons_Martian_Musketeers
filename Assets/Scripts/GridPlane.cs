using System.Collections.Generic;
using UnityEngine;

//https://docs.unity3d.com/Manual/InstantiatingPrefabs.html

//This script allows us to create and position multiple GridSquare prefabs procedurally. 

namespace Assets.Scripts
{
    public class GridPlane : MonoBehaviour
    {
        public GridSquare gridSquare;
        private static int groundWidth = 12;
        private static int groundHeight = 12;

        private GridSquare[,] coordinates = new GridSquare[groundWidth, groundHeight];

        // Awake is called before the first frame update and before every game object
        void Awake()
        {
            //position tiles and give them a coordinate identity
            for (int z = 0; z < groundHeight; z++)
            {
                for (int x = 0; x < groundWidth; x++)
                {
                    //https://docs.unity3d.com/ScriptReference/Quaternion-identity.html
                    //https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                    coordinates[x, z] = Instantiate(gridSquare, new Vector3(x, 0, z), Quaternion.Euler(90,0,0));
                    coordinates[x, z].xCoordinate = x;
                    coordinates[x, z].zCoordinate = z;
                }
            }

        }

        //finds the neighbors of selected grid square
        public List<GridSquare> FindNeighbors(int x, int z)
        {
            List<GridSquare> neighbors = new List<GridSquare>();
            
            //right
            if (x + 1 < groundWidth)
            {
                if (coordinates[x + 1, z].walkable)
                {
                    neighbors.Add(coordinates[x+1,z]);
                }
            }
            
            //left
            if (x - 1 >= 0)
            {
                if (coordinates[x - 1, z].walkable)
                {
                    neighbors.Add(coordinates[x - 1, z]);
                }
            }
            
            //up
            if (z + 1 < groundHeight)
            {
                if(coordinates[x,z+1].walkable)
                {
                    neighbors.Add(coordinates[x,z+1]);
                }
            }
            
            //down
            if (z - 1 >= 0)
            {
                if(coordinates[x,z-1].walkable)
                {
                    neighbors.Add(coordinates[x,z-1]);
                }
            }

            return neighbors;

        }

        public GridSquare GetSquareAtCoord(int x, int z)
        {
            return coordinates[x, z];
        }
    }
}
