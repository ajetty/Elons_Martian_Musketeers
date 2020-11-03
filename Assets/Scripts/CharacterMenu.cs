using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using TMPro;

public class CharacterMenu : MonoBehaviour

{
    public GameObject gameMaster;
    public Player activeCharacter;
    public Button moveButton;
    public Button attackButton;
    public Button defendButton;
    public Button closeButton;

    public TextMeshProUGUI statDisplay;

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.Find("GameMaster");
        activeCharacter = gameMaster.GetComponent<GameMaster>().getActiveCharacter();
        this.gameObject.SetActive(false);

        
        this.moveButton.onClick.AddListener(() =>
        {
            if (activeCharacter.canMove)
            {
                gameMaster.GetComponent<GameMaster>().moveButtonPressed = true;
            }
        });

        this.attackButton.onClick.AddListener(() =>
        {
            if (activeCharacter.canAct)
            {
                gameMaster.GetComponent<GameMaster>().attackButtonPressed = true;
            }
        });

        this.defendButton.onClick.AddListener(() =>
        {
            if (activeCharacter.canAct)
            {
                activeCharacter.defend();
            }
        });

        this.closeButton.onClick.AddListener(() =>
        {
            activeCharacter.closeMenu();
        });
    }

    // Update is called once per frame
    void Update()
    {
        activeCharacter = gameMaster.GetComponent<GameMaster>().getActiveCharacter();
    }

    public void updateStatDisplay()
    {
        statDisplay.text = activeCharacter.statsToString();
    }
}

