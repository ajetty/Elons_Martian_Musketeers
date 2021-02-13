using System;
using System.Collections;
using System.Collections.Generic;
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

    public CharacterStat health;
    public CharacterStat ap;
    public CharacterStat attack;
    public CharacterStat defense;
    public CharacterStat intelligence;
    public CharacterStat agility;

    public PlayerSkill skill1;
    public PlayerSkill skill2;
    public PlayerSkill activeSkill;

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
        health = new CharacterStat(50);
        ap = new CharacterStat(20);

        attack = new CharacterStat(UnityEngine.Random.Range(5, 10));
        defense = new CharacterStat(UnityEngine.Random.Range(5, 10));
        intelligence = new CharacterStat(UnityEngine.Random.Range(10, 15));
        agility = new CharacterStat(UnityEngine.Random.Range(5, 10));

        if (gameObject.name == "Player")
        {
            skill1 = new PlayerSkill("PowerATK", "attack with 20% extra dmg", "attack", new List<string> { "HP" }, this.attack, 3, this.attack.getValue() * 0.20f, false, 0);
            //attack target with 20% extra damage from atk stat

            skill2 = new PlayerSkill("IncATK", "double attack for 2 turns", "mod", new List<string> { "ATK" }, this.attack, 7, 1, true, 2);
            //double target's attack for two turns
        }
        else if (gameObject.name == "Player2")
        {
            skill1 = new PlayerSkill("Heal", "heal party member", "regen", new List<string> {"HP"}, this.intelligence, 5, 5, false, 0);
            //heal target for health equal to int stat plus 5

            skill2 = new PlayerSkill("buffDEF", "self buff DEF", "mod", new List<string> {"DEF"}, this.intelligence, 3, 0.50f, true, 3); 
            //buff target's defense by 50% of own int stat for 3 turns
        }
        else
        {
            skill1 = new PlayerSkill("INTatk", "attack HP and AP with doubled int", "attack", new List<string> {"HP","AP"}, this.intelligence, 10, this.intelligence.getValue(), false, 0);
            //attack target's HP and AP dealing damage equal to double own int stat. damaged is adjusted based on target's RES

            skill2 = new PlayerSkill("decRES", "decrease enemy RES by 10% of int for 3 turns", "mod", new List<string> { "RES" }, this.intelligence, 5, -0.10f, true, 3);
            //decrease target's RES stat by 10% of own int for 3 turns
        }

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
                bool isPlayerHit = hit.collider.tag == "Player";
                bool isEnemyHit = hit.collider.tag == "Enemy";
                bool isGridSqrHit = hit.collider.tag == "GridSquare";

                if (isGridSqrHit && state == "moving")
                {
                    GridSquare gridSquare = hit.collider.GetComponent<GridSquare>();

                    if (gridSquare.selectable)
                    {
                        MoveToGridSquare(gridSquare);
                    }
                }
                else if (isEnemyHit && state == "attack")
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
                else if ((isPlayerHit | isEnemyHit) && state == "skill")
                {
                    GridSquare gridSquare = null;
                    if (isPlayerHit)
                    {
                        Player player = hit.collider.GetComponent<Player>();
                        gridSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(player.transform.position.x),
                            Mathf.RoundToInt(player.transform.position.z));
                    }
                    else
                    {
                        Enemy enemy = hit.collider.GetComponent<Enemy>();
                        gridSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(enemy.transform.position.x),
                            Mathf.RoundToInt(enemy.transform.position.z));
                    }

                    if (gridSquare.selectable)
                    {
                        if (isPlayerHit)
                        {
                            Player target = hit.collider.GetComponent<Player>();
                            activeSkill.setTargetStatsPlayer(target);

                            activeSkill.execute(this, null);
                            activeSkill.scrubTargetStats();
                            charMenu.GetComponent<CharacterMenu>().updateStatDisplay();

                            activeSkill = null;
                            return true;
                        }
                        if (isEnemyHit)
                        {
                            Enemy target = hit.collider.GetComponent<Enemy>();
                            activeSkill.setTargetStatsEnemy(target);

                            activeSkill.execute(this, target);
                            activeSkill.scrubTargetStats();
                            target.updateEnemyStatDisplay();

                            activeSkill = null;
                            return true;
                        }
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
        //charMenu.GetComponent<CharacterMenu>().hideSkillButtons();
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
        r += "\nAP // " + ap.getValue();
        r += "\nAttack // " + attack.getValue();
        r += "\nDefense // " + defense.getValue();
        r += "\nIntelligence // " + intelligence.getValue();
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