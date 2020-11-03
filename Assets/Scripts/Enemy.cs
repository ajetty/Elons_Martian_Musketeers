using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Enemy : CharacterMovement, IPointerEnterHandler, IPointerExitHandler
    {
        //this game object has the tag of "Enemy"
        
        private Animator anim;
        private CharacterController controller;
        public GameObject gameMaster;

        private GridSquare moveTargetSquare;

        public bool mouseOver;

        public string eName;
        public CharacterStat health;
        public CharacterStat defense;

        // Start is called before the first frame update
        protected override void Start()
        {
            //Init(5, 2);
            base.Start();
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();
            health = new CharacterStat(100);
            defense = new CharacterStat(0);
            mouseOver = false;

            if (this.gameObject.name == "Enemy")
            {
                eName = "Enemy 1";
            }
            else
            {
                eName = "Enemy 2";
            }
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
                    moveTargetSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(target.transform.position.x), Mathf.RoundToInt(target.transform.position.z));
                    // List<GridSquare> test = PathToTarget(gridSquare);

                    MoveToGridSquare(moveTargetSquare);
                    
                }
                else
                {
                    Move();
                }
            }

            if (mouseOver && Mouse.current.leftButton.wasPressedThisFrame && gameMaster.GetComponent<GameMaster>().attackButtonPressed)
            {
                gameMaster.GetComponent<GameMaster>().enemyToAttack = this;
            }

            if (mouseOver && Mouse.current.leftButton.wasPressedThisFrame && !gameMaster.GetComponent<GameMaster>().attackButtonPressed)
            {
                gameMaster.GetComponent<GameMaster>().enemyToDisplay = this;
                gameMaster.GetComponent<GameMaster>().hideShowEnemyStatDisplay(this);
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

            // foreach (var VARIABLE in path)
            // {
            //     Debug.Log("STACK: " + VARIABLE.xCoordinate + " " + VARIABLE.zCoordinate);
            // }
            

            base.Move();

            if (moving == false)
            {
                anim.SetInteger("AnimationPar", 0);
                reachedDestination = false;
                moveTargetSquare.target = false;
            }
        }
        
        public override void MoveToGridSquare(GridSquare gridSquare)
        {
            path.Clear();
            gridSquare.target = true;
            moving = true;
            
            List<GridSquare> AList = PathToTarget(gridSquare);

            AList.RemoveAt(0);
            
            
            // foreach (var VARIABLE in AList)
            // {
            //     Debug.Log("ALIST: " + VARIABLE.xCoordinate + " " + VARIABLE.zCoordinate);
            // }

            path = new Stack<GridSquare>(AList);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + " mouse has entered.");
            mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + " mouse has exit.");
            mouseOver = false;
        }

        public string statsToString()
        {
            string r = this.eName + "\n\n";
            r += "HP  // " + this.health.getValue();
            r += "\nDEF // " + this.defense.getValue();
            return r;
        }
    }
}