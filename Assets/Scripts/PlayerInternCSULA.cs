using System.Collections;
using System.Resources;
using System.Runtime.InteropServices;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInternCSULA : Player
{
    public CharacterStat agility;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        System.Random ran = new System.Random();
        agility = new CharacterStat(ran.Next(1, 6));

        Debug.Log("CSULA intern has agility stat " + agility.getValue());

        //Init((int)this.agility.getValue(), 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseOver && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTurn)
            {
                GetComponent<CharacterMovement>().SetMoveRange((int)this.agility.getValue());
                //setAfterTurn();
            }
            else
            {
                setIsActive((int)agility.getValue());
            }
        }
        if (gameMaster.GetComponent<GameMaster>().moveButtonPressed && isTurn)
        {
            if (!moving)
            {
                FindSelectableTiles();
                CheckMouse();
            }
            else if (moving && !reachedDestination)
            {
                //Debug.Log("Character is moving.");
                //Debug.Log("Current grid square is " + currentGridSquare.xCoordinate + ", " + currentGridSquare.zCoordinate);
                //Move();
            }
            else if (!moving && reachedDestination)
            {
                //setAfterTurn();
            }
        }
    }
}
