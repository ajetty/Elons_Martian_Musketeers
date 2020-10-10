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
                    FindSelectableTiles();
                    CheckMouse();
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

            base.Move();

            if (moving == false)
            {
                anim.SetInteger("AnimationPar", 0);
            }
        }
    }
}