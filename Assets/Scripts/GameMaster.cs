using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class GameMaster : MonoBehaviour
{
    //this game object has the tag of "GameMaster"
    [FormerlySerializedAs("characterList")] public List<GameObject> playerList;
    public List<GameObject> enemyList;
    public GridPlane gridPlane;
    public TurnRoster turnRoster;

    public Canvas enemyMenu;

    public Player activeCharacter;
    public Enemy enemyToAttack;
    public Enemy enemyToDisplay;

    public bool moveButtonPressed;
    public bool attackButtonPressed;

    private List<(int, int)> unwalkableSquares = new List<(int, int)>()
    {
        (1, 1), (1, 8), (5, 5), (7, 1), (9, 7),
        (23,2), (22, 2), (22, 3), (23, 3), (24, 3), (22, 4), (23, 4), (24, 4), (21,5), (22, 5), (23, 5), (24, 5), (21,6), (22, 6), (23, 6), (24, 6), (21, 7), (22, 7), (23, 7), (24, 7), (22, 8), (23, 8), (24, 8),
        (23,13),(24,13),(25,13), (21,14), (22,14), (23,14), (24,14), (25,14), (26,14), (27,14), (28,14), (20, 15), (21,15), (22,15), (23,15), (24,15), (25,15), (26,15), (27,15), (28,15), (29,15), (30,15), (19,16), (20, 16), (21,16), (22,16), (23,16), (24,16), (25,16), (26,16), (27,16), (28,16), (29,16), (30,16), (31,16), (19,17), (20, 17), (21,17), (22,17), (23,17), (24,17), (25,17), (26,17), (27,17), (28,17), (29,17), (30,17), (31,17), (32,17), (19,18), (20, 18), (21,18), (22,18), (23,18), (24,18), (25,18), (26,18), (27,18), (28,18), (29,18), (30,18), (31,18), (32,18), (20, 19), (21,19), (22,19), (23,19), (24,19), (25,19), (26,19), (27,19), (28,19), (29,19), (30,19), (31,19), (20, 20), (21,20), (22,20), (23,20), (24,20), (25,20), (26,20), (27,20), (28,20), (29,20),
        (6,30),(7,30),(8,30),(6,29),(7,29),(8,29),(9,29),(10,29),(6,28),(7,28),(8,28),(9,28),(10,28),(7,27),(8,27),(9,27),(10,27),(11,27),(12,27),(8,26),(9,26),(10,26),(11,26),(12,26),(13,26),(9,25),(10,25),(10,25),(11,25),(12,25),(13,25),(10,24),(11,24),(12,24),(13,24),(14,24),(11,23),(12,23),(13,23),(14,23),(13,22),
        (25,39),(25,38),(25,37),(25,36),(25,35),(25,34),(26,33),(27,32),(28,31),(29,30),(30,29),(31,29),(32,29),(33,28),(34,28),(35,28),(36,28),(37,28),(38,28),(39,28)
    };


    // Start is called before the first frame update
    void Start()
    {
        //get turn roster component from game master
        turnRoster = GetComponent<TurnRoster>();

        //find all characters using their tags and put reference of them into list
        playerList = GameObject.FindGameObjectsWithTag("Player").ToList();
        enemyList = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        gridPlane = GameObject.FindObjectOfType<GridPlane>();

        //hide enemy stat menu
        enemyMenu.gameObject.SetActive(false);

        if (gridPlane)
        {
            foreach (GameObject character in playerList)
            {
                CharacterMovement charMovComp = character.GetComponent<CharacterMovement>();

                int x = Mathf.RoundToInt(character.transform.position.x);
                int z = Mathf.RoundToInt(character.transform.position.z);
                charMovComp.SetGridSquare(gridPlane.GetSquareAtCoord(x, z));
                charMovComp.gridPlane = gridPlane;

                Debug.Log(character.name + " : " + character.tag + " : " + playerList.IndexOf(character));
            }
            foreach (GameObject character in enemyList)
            {
                CharacterMovement charMovComp = character.GetComponent<CharacterMovement>();

                int x = Mathf.RoundToInt(character.transform.position.x);
                int z = Mathf.RoundToInt(character.transform.position.z);
                charMovComp.SetGridSquare(gridPlane.GetSquareAtCoord(x, z));
                charMovComp.gridPlane = gridPlane;

                Debug.Log(character.name + " : " + character.tag + " : " + enemyList.IndexOf(character));
            }
        }
        else
        {
            Debug.Log("ERROR: The grid plane being referred to in game master is null.");
        }

        turnRoster.SetTurnList(playerList, enemyList);

        foreach (var VARIABLE in unwalkableSquares)
        {
            gridPlane.GetSquareAtCoord(VARIABLE.Item1, VARIABLE.Item2).walkable = false;
        }
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
                    activeCharacter.GetComponent<CharacterMovement>().FindSelectableTiles();
                    activeCharacter.CheckMouse();
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
                turnRoster.playerCount--;
                moveButtonPressed = false;
                activeCharacter = null;
            }

            if (attackButtonPressed && activeCharacter.isTurn && activeCharacter.canAct)
            {
                if (enemyToAttack != null)
                {
                    attack(activeCharacter, enemyToAttack);

                    activeCharacter.setAfterAction();

                    attackButtonPressed = false;

                    enemyToAttack = null;
                    activeCharacter = null;
                }
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

    public void attack(Player attacker, Enemy attacked)
    {
        attacked.health.decrement(attacker.attack.getValue());
        Debug.Log("Enemy was attacked. Current health is now " + attacked.health.getValue());

        if (enemyToDisplay != null)
        {
            if (enemyToDisplay == attacked)
            {
                updateEnemyStatDisplay(attacked);
            }
        }
    }

    public void updateEnemyStatDisplay(Enemy enemy)
    {
        TextMeshProUGUI statDisplay = enemyMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        statDisplay.text = enemy.statsToString();
    }

    public void hideShowEnemyStatDisplay(Enemy enemy)
    {
        if (this.enemyMenu.gameObject.active && enemy != this.enemyToDisplay) 
        {
            updateEnemyStatDisplay(enemy);
        }
        else if (this.enemyMenu.gameObject.active && enemy == this.enemyToDisplay)
        {
            enemyMenu.gameObject.SetActive(false);
        }
        else  if (!this.enemyMenu.gameObject.active && enemy == this.enemyToDisplay)
        {
            enemyMenu.gameObject.SetActive(true);
            updateEnemyStatDisplay(enemy);
        }
    }
}