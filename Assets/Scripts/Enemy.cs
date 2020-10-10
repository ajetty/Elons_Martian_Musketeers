using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Enemy : CharacterMovement
    {
        //this game object has the tag of "Enemy"
        
        private Animator anim;
        private CharacterController controller;

        // Start is called before the first frame update
        void Start()
        {
            Init(5, 2);
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Beginning: " + isTurn);
            if (isTurn)
            {

                if (!moving)
                {
                    //FindSelectableTiles();
                    //CheckMouse();
                    //FindSelectableTiles();
                    Transform target = FindNearestTarget("Player");
                    GridSquare gridSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(target.transform.position.x), Mathf.RoundToInt(target.transform.position.z));
                    // List<GridSquare> test = PathToTarget(gridSquare);

                    MoveToGridSquare(gridSquare);
                    
                }
                else
                {
                    Move();
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

        public override void FindSelectableTiles()
        {
            if (currentGridSquare)
            {
                currentGridSquare.current = true;
            }
            base.FindSelectableTiles();
        }
        
        public override void Move()
        {
            anim.SetInteger("AnimationPar", 1);

            foreach (var VARIABLE in path)
            {
                Debug.Log("STACK: " + VARIABLE.xCoordinate + " " + VARIABLE.zCoordinate);
            }
            

            base.Move();

            if (moving == false)
            {
                anim.SetInteger("AnimationPar", 0);
            }
        }
        
        public override void MoveToGridSquare(GridSquare gridSquare)
        {
            path.Clear();
            gridSquare.target = true;
            moving = true;
            
            List<GridSquare> AList = PathToTarget(gridSquare);

            AList.RemoveAt(0);
            
            
            foreach (var VARIABLE in AList)
            {
                Debug.Log("ALIST: " + VARIABLE.xCoordinate + " " + VARIABLE.zCoordinate);
            }

            path = new Stack<GridSquare>(AList);
        }
    }
}