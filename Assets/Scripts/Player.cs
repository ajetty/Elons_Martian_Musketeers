using System.Collections;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts;
using UnityEngine.EventSystems;


public class Player : CharacterMovement, IPointerEnterHandler, IPointerExitHandler
{
    //this game object has the tag of "Player"

    public bool mouseOver;

    public GameObject charMenu;
    public GameObject gameMaster;

    //public bool isTurn;
    
    private Animator anim;
    private CharacterController controller;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // //Debug.Log("Beginning: " + isTurn);
        // if (isTurn)
        // {
        //     if (!moving)
        //     {
        //         FindSelectableTiles();
        //         CheckMouse();
        //     }
        //     else
        //     {
        //         Move();
        //     }
        // }
    }

    protected void CheckMouse()
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

        //isTurn = false;
    }

    public override void FindSelectableTiles()
    {
        if (currentGridSquare)
        {
            currentGridSquare.current = true;
        }

        base.FindSelectableTiles();
    }
    
    public bool getIsTurn()
    {
        return isTurn;
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
}