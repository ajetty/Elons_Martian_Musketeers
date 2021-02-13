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
    public Button skillButton;
    public Button skill1Button;
    public Button skill2Button;
    public Button defendButton;
    public Button closeButton;

    public bool skillsShowing;

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
        
        // this.defendButton.onClick.AddListener(() =>
        // {
        //     if (!activeCharacter.moving)
        //     {
        //         gameMaster.defendButtonPressed = true;
        //         gameMaster.moveButtonPressed = false;
        //         gameMaster.attackButtonPressed = false;
        //     }
        // });

        // this.closeButton.onClick.AddListener(() =>
        // {
        //     if (!activeCharacter.isTurn)
        //     {
        //         activeCharacter.charMenu.gameObject.SetActive(false);
        //     }
        // });

        // this.skillButton.onClick.AddListener(() =>
        // {
        //     if (skillsShowing)
        //     {
        //         hideSkillButtons();
        //     }
        //     else
        //     {
        //         showSkillButtons();
        //     }
        // });

        // this.skill1Button.onClick.AddListener(() =>
        // {
        //     //
        //     activeCharacter.activeSkill = activeCharacter.skill1;
        //     gameMaster.skillButtonPressed = true;
        // });
        //
        // this.skill2Button.onClick.AddListener(() =>
        // {
        //     //
        //     activeCharacter.activeSkill = activeCharacter.skill2;
        //     gameMaster.skillButtonPressed = true;
        // });
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

    // public void updateSkillDisplay()
    // {
    //     skill1Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(activeCharacter.skill1.name);
    //     skill2Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(activeCharacter.skill2.name);
    // }

    // public void hideSkillButtons()
    // {
    //     this.skill1Button.gameObject.SetActive(false);
    //     this.skill2Button.gameObject.SetActive(false);
    //     skillsShowing = false;
    // }

    // public void showSkillButtons()
    // {
    //     this.skill1Button.gameObject.SetActive(true);
    //     this.skill2Button.gameObject.SetActive(true);
    //     updateSkillDisplay();
    //     skillsShowing = true;
    //
    //
    // }

}

