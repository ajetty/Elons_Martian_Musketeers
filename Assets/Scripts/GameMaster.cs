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
        
    }
}
