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
        public bool mouseOver;

        public GameObject charMenu;
        public GameObject gameMaster;

        public bool isTurn;

        // Start is called before the first frame update
        // protected override void Start()
        // {
        //     base.Start();
        //     //System.Random ran = new System.Random();
        //     //this.agility = new CharacterStat(ran.Next(1, 6));
        //
        //     //Init((int) this.agility.getValue(), 2);
        //     //SetGridSquare(gridPlane.GetSquareAtCoord(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
        //     //isTurn = true;
        // }

        // Update is called once per frame
        void Update()
        {

            /*
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
                    setAfterTurn();
                    Debug.Log("AFter: " + isTurn);
                }
            }
            */
        }


        public void CheckMouse()
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
            //Debug.Log("SetGridSquare called.");
            if (currentGridSquare != null)
            {
                currentGridSquare.current = false;
            }

            base.SetGridSquare(square);
            //currentGridSquare.current = true;
            //isTurn = false;

            //Debug.Log("CurrentGridSquare coords are " + square.transform.position.x + ", " + square.transform.position.z);
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

        public void setIsActive(int move)
        {
            //Debug.Log(gameObject.name + " is now active player.");
            isTurn = true;
            SetMoveRange(move);
            gameMaster.GetComponent<GameMaster>().setActiveCharacter(this);
            charMenu.SetActive(true);
        }

        public void setAfterTurn()
        {
            isTurn = false;
            reachedDestination = false;
            //gameMaster.GetComponent<GameMaster>().setActiveCharacter(null);
            //gameMaster.GetComponent<GameMaster>().moveButtonPressed = false;
            charMenu.SetActive(false);
        }

        public void OnMouseOver()
        {
            mouseOver = true;
        }

        public void OnMouseExit()
        {
            mouseOver = false;
        }

    }
}