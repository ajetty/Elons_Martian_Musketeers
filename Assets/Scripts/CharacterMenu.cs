using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;

public class CharacterMenu : MonoBehaviour

{
    public GameMaster gameMaster;
    public Player activeCharacter;
    public Button moveButton;
    public Button attackButton;
    public Button defendButton;
    public Button closeButton;
    //public TMP_Text health;
    public TextMeshProUGUI statDisplay;

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        activeCharacter = gameMaster.GetComponent<GameMaster>().getActiveCharacter();
        this.gameObject.SetActive(false);

        
        this.moveButton.onClick.AddListener(() =>
        {
            if (!activeCharacter.moving)
            {
                gameMaster.moveButtonPressed = true;
                gameMaster.attackButtonPressed = false;
                gameMaster.defendButtonPressed = false;
            }
        });
        
        this.attackButton.onClick.AddListener(() =>
        {
            if (!activeCharacter.moving)
            {
                gameMaster.attackButtonPressed = true;
                gameMaster.moveButtonPressed = false;
                gameMaster.defendButtonPressed = false;
            }
        });
        
        this.defendButton.onClick.AddListener(() =>
        {
            if (!activeCharacter.moving)
            {
                gameMaster.defendButtonPressed = true;
                gameMaster.moveButtonPressed = false;
                gameMaster.attackButtonPressed = false;
            }
        });

        this.closeButton.onClick.AddListener(() =>
        {
            if (!activeCharacter.moving)
            {
                activeCharacter.setAfterTurn();
            }
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        activeCharacter = gameMaster.getActiveCharacter();

    }
    
    public void updateStatDisplay()
    {
        statDisplay.text = activeCharacter.statsToString();
    }

}

