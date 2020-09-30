using System.Collections;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : CharacterMovement
    {
        //this game object has the tag of "Player"


        public bool isTurn;

        // Start is called before the first frame update
        void Start()
        {
            Init(5, 2);
            //SetGridSquare(gridPlane.GetSquareAtCoord(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
            //isTurn = true;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Beginning: " + isTurn);
            if (isTurn)
            {

                if (!moving)
                {
                    FindSelectableTiles();
                    CheckMouse();
                }
                else
                {
                    Move();
                    Debug.Log("AFter: " + isTurn);
                }
            }
        }


        void CheckMouse()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "GridSquare")
                    {
                        GridSquare gridSquare = hit.collider.GetComponent<GridSquare>();

                        if (gridSquare.selectable)
                        {
                            MoveToGridSquare(gridSquare);
                        }
                    }
                }
                
            }
        }

        public override void SetGridSquare(GridSquare square)
        {
            if (currentGridSquare != null)
            {
                currentGridSquare.current = false;
            }

            base.SetGridSquare(square);
            //currentGridSquare.current = true;
            isTurn = false;
        }

        public bool getIsTurn()
        {
            return isTurn;
        }

        override public void FindSelectableTiles()
        {
            if (currentGridSquare)
            {
                currentGridSquare.current = true;
            }
            base.FindSelectableTiles();
        }
    }
}