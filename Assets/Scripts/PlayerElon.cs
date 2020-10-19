using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerElon : Player
{
    public CharacterStat agility;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        System.Random ran = new System.Random();
        agility = new CharacterStat(ran.Next(1, 6));

        Debug.Log("Elon has agility stat " + agility.getValue());

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
    }
}
