using System;
using System.Collections;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


public class Player : CharacterMovement, IPointerEnterHandler, IPointerExitHandler
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
    
    public int healthPoints = 10;
    
    public CharacterStat attack;
    public CharacterStat defense;
    public CharacterStat agility;
    public CharacterStat health;
    
    private float deathTime = Mathf.Infinity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();
        
         System.Random ran = new System.Random();
         agility = new CharacterStat(ran.Next(5, 10));
         attack = new CharacterStat(ran.Next(5, 10));
         defense = new CharacterStat(ran.Next(5, 10));
         health = new CharacterStat(10);
        
        //Debug.Log(name + " has " + health.getValue());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (mouseOver && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!isTurn && hasTurnAvailable)
            {
                //setIsActive(Random.Range(4,6));
                setIsActive();
            }
        }
        
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
                }else if (hit.collider.tag == "Enemy" && state == "attack")
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    GridSquare gridSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.z));

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

    public override void FindSelectableTiles()
    {
        if (currentGridSquare)
        {
            currentGridSquare.current = true;
        }

        base.FindSelectableTiles();
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
    
    public void setIsActive( )
    {
        Debug.Log(name + " has " + health.getValue());
        isTurn = true;
        SetMoveRange((int)agility.getValue());
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " mouse has exit.");
        mouseOver = false;
    }

    public void StartNewRound()
    {
        gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.07f, 0.59f, 0.03f));
        hasTurnAvailable = true;
    }

    public virtual void Attack(Enemy enemy)
    {
        
        Debug.Log(enemy.name + " has been selected for attack.");

        System.Random rnd = new System.Random();

        int missFactor = rnd.Next(1, 11);

        if (missFactor < 6)
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
            //enemy.setHP(-1);
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

    public virtual int getHP()
    {
        return -999999;
    }

    public virtual void setHP(int points)
    {
        
    }
}