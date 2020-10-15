using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //this game object has the tag of "GameMaster"
    public List<GameObject> characterList;
    public GridPlane gridPlane;
    public TurnRoster turnRoster;
    public Player activeCharacter;
    public bool moveButtonPressed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //get turn roster component from game master
        turnRoster = GetComponent<TurnRoster>();

        //find all characters using their tags and put reference of them into list
        characterList = GameObject.FindGameObjectsWithTag("Player").ToList();
        gridPlane = GameObject.FindObjectOfType<GridPlane>();
        
        if (gridPlane != null)
        {
            foreach (GameObject character in characterList)
            {
                CharacterMovement charMovComp = character.GetComponent<CharacterMovement>();

                int x = Mathf.RoundToInt(character.transform.position.x);
                int z = Mathf.RoundToInt(character.transform.position.z);
                charMovComp.SetGridSquare(gridPlane.GetSquareAtCoord(x, z));
                charMovComp.gridPlane = gridPlane;

            }
        }

        turnRoster.SetTurnList(characterList);

    }

    // Update is called once per frame
    void Update()
    {
        if (activeCharacter != null)
        {
            if (moveButtonPressed && activeCharacter.isTurn)
            {
                if (!activeCharacter.GetComponent<CharacterMovement>().moving)
                {
                    //Debug.Log("Character is not moving, so we find selectable tiles.");
                    //activeCharacter.GetComponent<CharacterMovement>().moving = true;
                    //activeCharacter.GetComponent<CharacterMovement>().FindSelectableTiles();
                    //activeCharacter.CheckMouse();
                }
                else
                {
                    //Debug.Log("Character is moving.");
                    activeCharacter.GetComponent<CharacterMovement>().Move();
                    //activeCharacter.setAfterTurn();
                    //Debug.Log("After: ");
                }
            }
            if (activeCharacter.isTurn && activeCharacter.GetComponent<CharacterMovement>().reachedDestination)
            {
                activeCharacter.setAfterTurn();
                moveButtonPressed = false;
                activeCharacter = null;
            }
        }
    }

    public void setActiveCharacter(Player activeCharacter)
    {
        this.activeCharacter = activeCharacter;
    }

    public Player getActiveCharacter()
    {
        return this.activeCharacter;
    }
}
