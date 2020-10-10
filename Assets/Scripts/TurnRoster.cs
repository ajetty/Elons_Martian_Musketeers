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
        public List<GameObject> listOfCharacters;
        public int turnIndex;

        //private int count = 0;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (listOfCharacters[turnIndex].GetComponent<CharacterMovement>().isTurn == false)
            {
                turnIndex = (turnIndex + 1) % listOfCharacters.Count;
                listOfCharacters[turnIndex].GetComponent<CharacterMovement>().isTurn = true;
            }
        }

        /*else if (listOfCharacters[turnIndex].tag == "Enemy")
        {
            if (listOfCharacters[turnIndex].GetComponent<Enemy>().isTurn == false)
            {
                turnIndex = (turnIndex + 1) % listOfCharacters.Count;
                listOfCharacters[turnIndex].GetComponent<Enemy>().isTurn = true;
                Debug.Log("Enemy: " + listOfCharacters[turnIndex].ToString() + " turnIndex: " + turnIndex);
            }
        }*/
        /*foreach (GameObject item in listOfCharacters)
        {
            if (item.tag == "Player")
            {
                item.GetComponent<Player>().isTurn = true;
                Debug.Log("Player: " + item.ToString() + " turnIndex: " + turnIndex);
            }else if (item.tag == "Enemy")
            {
                item.GetComponent<Enemy>().isTurn = true;
                Debug.Log("Enemy: " + item.ToString() + " turnIndex: " + turnIndex);
            }
        }*/
        
        public void SetTurnList(List<GameObject> listOfCharacters)
        {
            this.listOfCharacters = listOfCharacters;
            listOfCharacters[0].GetComponent<CharacterMovement>().isTurn = true;
        }
    }
    
}

