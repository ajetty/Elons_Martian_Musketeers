using System;
using System.Collections;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


public class Player : CharacterMovement, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //this game object has the tag of "Player"

    public bool mouseOver;

    public GameObject charMenu;
    public GameObject gameMaster;

    public LineRenderer lazerFX;
    public GameObject hitFX;
    public GameObject missFX;
    public GameObject DeathFX;

    public AudioClip hitSound;
    public AudioClip missSound;

    public AudioSource audioSource;
    //public bool isTurn;

    private Animator anim;
    private CharacterController controller;
    public bool hasTurnAvailable = true;

    public CharacterStat attack;
    public CharacterStat defense;
    public CharacterStat agility;
    public CharacterStat health;

    private float deathTime = Mathf.Infinity;


    public Shader originalShaderPlayer;
    public Shader originalShaderDiamond;

    public bool isClickable;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();

        //System.Random ran = new System.Random();
        agility = new CharacterStat(UnityEngine.Random.Range(5, 10));
        attack = new CharacterStat(UnityEngine.Random.Range(5, 10));
        defense = new CharacterStat(UnityEngine.Random.Range(5, 10));
        health = new CharacterStat(10);

        isClickable = true;

        //Debug.Log(name + " has " + health.getValue());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        
        if (health.getValue() <= 0.0f && isDead == false)
        {
            Debug.Log(name + " has died!");
            Death();
            deathTime = Time.time;
        }

        if (isDead && Time.time > deathTime + 0.5f)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            gameMaster.GetComponent<TurnRoster>().playerLiveCount--;
            gameMaster.GetComponent<TurnRoster>().playerCount--;
        }
    }

    public bool CheckMouse(string state)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "GridSquare" && state == "moving")
                {
                    GridSquare gridSquare = hit.collider.GetComponent<GridSquare>();

                    if (gridSquare.selectable)
                    {
                        MoveToGridSquare(gridSquare);
                    }
                }
                else if (hit.collider.tag == "Enemy" && state == "attack")
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    GridSquare gridSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(enemy.transform.position.x),
                        Mathf.RoundToInt(enemy.transform.position.z));

                    if (gridSquare.selectable)
                    {
                        Attack(enemy);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void SetGridSquare(GridSquare square)
    {
        if (currentGridSquare != null)
        {
            currentGridSquare.current = false;
        }

        base.SetGridSquare(square);

        //isTurn = false;
    }

    public override void FindSelectableTiles(bool attackMode)
    {
        if (currentGridSquare)
        {
            currentGridSquare.current = true;
        }

        base.FindSelectableTiles(attackMode);
    }

    public bool getIsTurn()
    {
        return isTurn;
    }

    public override void Move()
    {
        
        anim.SetInteger("AnimationPar", 1);

        base.Move();

        if (moving == false)
        {
            anim.SetInteger("AnimationPar", 0);
        }
    }

    public void setIsActive()
    {
        //isTurn = true;
        SetMoveRange((int) agility.getValue());
        gameMaster.GetComponent<GameMaster>().setActiveCharacter(this);
        charMenu.GetComponent<CharacterMenu>().statDisplay.text = this.statsToString();
        charMenu.SetActive(true);
    }

    public void setAfterTurn()
    {
        isTurn = false;
        reachedDestination = false;
        charMenu.SetActive(false);
        gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        hasTurnAvailable = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " mouse has entered.");
        mouseOver = true;
        originalShaderPlayer = gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.shader;
        originalShaderDiamond = gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.shader;
        gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[0].shader =
            Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
        gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].shader =
            Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
        gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[2].shader =
            Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
        gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.shader =
            Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " mouse has exit.");
        mouseOver = false;
        gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[0].shader = originalShaderPlayer;
        gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].shader = originalShaderPlayer;
        gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[2].shader = originalShaderPlayer;
        gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.shader = originalShaderDiamond;
    }

    public void StartNewRound()
    {
        gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material
            .SetColor("_Color", new Color(0.07f, 0.59f, 0.03f));
        hasTurnAvailable = true;
    }

    public virtual void Attack(Enemy enemy)
    {
        Debug.Log(enemy.name + " has been selected for attack.");


        int missFactor = UnityEngine.Random.Range(1, 11);

        if (missFactor < 0)
        {
            Instantiate(missFX, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(missSound);
        }
        else
        {
            Vector3 spawnPosition = transform.position + Vector3.up * 1.0f;
            LineRenderer lazerLineRenderer = Instantiate(lazerFX, spawnPosition, Quaternion.identity);
            lazerLineRenderer.SetPosition(0, enemy.transform.position - transform.position);
            Vector3 hitPosition = enemy.transform.position + Vector3.up;
            hitPosition += Vector3.Normalize(Camera.main.transform.position - hitPosition) * 1;
            Instantiate(hitFX, hitPosition, Quaternion.identity);
            enemy.health.decrement(attack.getValue());
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void Death()
    {
        Debug.Log(name + " is dead.");
        anim.SetBool("Dieing", true);
        Instantiate(DeathFX, transform.position, Quaternion.identity);
        isDead = true;
        currentGridSquare.occupant = null;
    }

    public string statsToString()
    {
        string r = "Health // " + health.getValue();
        r += "\nAttack // " + attack.getValue();
        r += "\nDefense // " + defense.getValue();
        r += "\nAgility // " + agility.getValue();
        return r;
    }

    public void defend()
    {
        defense.addModifier(new StatModifier(1.50f, false, 2));
        defense.setModified(true);
        hasTurnAvailable = false;
        charMenu.GetComponent<CharacterMenu>().updateStatDisplay();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hasTurnAvailable && isClickable)
        {
            setIsActive();
        }
    }
}