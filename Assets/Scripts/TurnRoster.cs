using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            /*
            if (listOfCharacters[turnIndex].GetComponent<Player>().isTurn == false)
            {
                turnIndex = (turnIndex + 1) % listOfCharacters.Count;
                listOfCharacters[turnIndex].GetComponent<Player>().isTurn = true;
            }
            */
        }

        public void SetTurnList(List<GameObject> listOfCharacters)
        {
            /*
            this.listOfCharacters = listOfCharacters;
            listOfCharacters[1].GetComponent<Player>().isTurn = true;
            */
        }

    }
}