using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To do:
//add a way to add a player or npc to list of characters
//-------who tells roster to add? do we need a game battle class to keep track of battle scene transitions?
//add a way to remove a player or npc from list of characters
//-------listen to player npc object and it tells roster to remove

namespace Assets.Scripts
{
    public class TurnRoster : MonoBehaviour
    {
        public List<GameObject> listOfPlayers;
        public List<GameObject> listOfEnemies;
        public int turnIndex;
        public int playerCount;
        public int enemyCount;
        private bool enemyTurn;
        private bool playerTurn;
        public CameraSystem cameraSystem;
        public bool win;
        public bool lose;
        public int enemyLiveCount;
        public int playerLiveCount;

        //private int count = 0;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

            if (playerLiveCount == 0)
            {
                lose = true;
            }
            else if (playerTurn)
            {
                if (playerCount == 0)
                {
                    enemyTurn = true;
                    playerTurn = false;
                    //enemy turn initialization
                    turnIndex = 0;
                    listOfEnemies[turnIndex].GetComponent<CharacterMovement>().isTurn = true;
                }
            }

            
            if(enemyLiveCount == 0)
            {
                win = true;
            }
            else if (enemyTurn)
            {
                CharacterMovement currentCharacter = listOfEnemies[turnIndex].GetComponent<CharacterMovement>();
                if (currentCharacter.isTurn == false || currentCharacter.isDead == true)
                {
                    turnIndex = turnIndex + 1;
                    if (turnIndex < enemyCount)
                    {
                        listOfEnemies[turnIndex].GetComponent<CharacterMovement>().isTurn = true;
                    }
                    else //end of enemy turn
                    {
                        enemyTurn = false;
                        
                        //reset each player gem back to green
                        foreach (GameObject player in listOfPlayers)
                        {
                            player.GetComponent<Player>().StartNewRound();
                            playerCount = playerLiveCount;
                        }

                        playerTurn = true;
                    }
                }
            }

        }
        
        public void SetTurnList(List<GameObject> listOfPlayers, List<GameObject> listOfEnemies)
        {
            playerTurn = true;
            this.listOfPlayers = listOfPlayers;
            this.listOfEnemies = listOfEnemies;
            playerCount = listOfPlayers.Count;
            playerLiveCount = playerCount;
            enemyCount = listOfEnemies.Count;
            enemyLiveCount = enemyCount;
        }
        
        
    }
}