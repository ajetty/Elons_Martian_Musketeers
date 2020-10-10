﻿using System;
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
    private List<(int, int)> unwalkableSquares = new List<(int, int)>()
    {
        (1,1), (1,8), (5,5), (7,1), (9,7)
    };
    
    
    // Start is called before the first frame update
    void Start()
    {
        //get turn roster component from game master
        turnRoster = GetComponent<TurnRoster>();

        //find all characters using their tags and put reference of them into list
        characterList = GameObject.FindGameObjectsWithTag("Player").ToList();
        characterList.AddRange(GameObject.FindGameObjectsWithTag("Enemy").ToList());
        gridPlane = GameObject.FindObjectOfType<GridPlane>();
        
        if (gridPlane)
        {
            foreach (GameObject character in characterList)
            {
                CharacterMovement charMovComp = character.GetComponent<CharacterMovement>();

                int x = Mathf.RoundToInt(character.transform.position.x);
                int z = Mathf.RoundToInt(character.transform.position.z);
                charMovComp.SetGridSquare(gridPlane.GetSquareAtCoord(x, z));
                charMovComp.gridPlane = gridPlane;
                
                Debug.Log(character.name + " : " + character.tag + " : " + characterList.IndexOf(character));

            }
        }
        else
        {
            Debug.Log("ERROR: The grid plane being referred to in game master is null.");
        }

        turnRoster.SetTurnList(characterList);

        foreach (var VARIABLE in unwalkableSquares)
        {
            gridPlane.GetSquareAtCoord(VARIABLE.Item1, VARIABLE.Item2).walkable = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
