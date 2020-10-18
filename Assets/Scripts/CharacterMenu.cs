using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class CharacterMenu : MonoBehaviour

{
    public GameObject gameMaster;
    public Player activeCharacter;
    public Button moveButton;
    public Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.Find("GameMaster");
        activeCharacter = gameMaster.GetComponent<GameMaster>().getActiveCharacter();
        this.gameObject.SetActive(false);

        
        this.moveButton.onClick.AddListener(() =>
        {
            gameMaster.GetComponent<GameMaster>().moveButtonPressed = true;
        });
        

        this.closeButton.onClick.AddListener(() =>
        {
            activeCharacter.setAfterTurn();
        });
    }

    // Update is called once per frame
    void Update()
    {
        activeCharacter = gameMaster.GetComponent<GameMaster>().getActiveCharacter();
    }

}

