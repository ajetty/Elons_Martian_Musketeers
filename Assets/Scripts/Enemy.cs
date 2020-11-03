using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Resources;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Enemy : CharacterMovement, IPointerClickHandler
    {
        //this game object has the tag of "Enemy"

        private Animator anim;
        private CharacterController controller;

        private GridSquare moveTargetSquare;

        public int healthPoints;

        public GameObject DeathFX;

        private float deathTime = Mathf.Infinity;
        
        public GameObject gameMaster;
        
        public GameObject hitFX;
        public GameObject missFX;

        public AudioClip hitSound;
        public AudioClip missSound;
    
        public AudioSource audioSource;
        
        public CameraSystem cameraSystem;
        
        public Canvas enemyMenu;
        
        public string eName;
        public CharacterStat health;
        public CharacterStat defense;
        public CharacterStat agility;
        public CharacterStat attack;

        // Start is called before the first frame update
        protected override void Start()
        {
            //Init(5, 2);
            base.Start();
            System.Random ran = new System.Random();
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();
            health = new CharacterStat(1);
            defense = new CharacterStat(0);
            agility = new CharacterStat(ran.Next(5, 10));
            attack = new CharacterStat(ran.Next(5, 10));
            
            if (this.gameObject.name == "Enemy")
            {
                eName = "Enemy 1";
            }
            else
            {
                eName = "Enemy 2";
            }
            
            hideEnemyStatDisplay();
            
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Beginning: " + isTurn);
            if (isTurn)
            {
                cameraSystem.SetCameraOrigin(gameObject.transform);
                
                Player target = FindNearestTarget("Player");
                moveTargetSquare = gridPlane.GetSquareAtCoord(Mathf.RoundToInt(target.transform.position.x),
                    Mathf.RoundToInt(target.transform.position.z));
                
                
                if (!moving)
                {
                    if (Vector3.Distance(transform.position, target.transform.position) > agility.getValue())
                    {
                        MoveToGridSquare(moveTargetSquare);
                    }
                    else
                    {
                        Attack(target);
                    }
                }

                if (moving)
                {
                    Move();
                }

            }

            if (health.getValue() <= 0.0f && isDead == false)
            {
                Death();
                deathTime = Time.time;
            }

            if (isDead && Time.time > deathTime + 0.5f)
            {
                //Destroy(gameObject);
                gameObject.SetActive(false);
                gameMaster.GetComponent<TurnRoster>().enemyLiveCount--; 
            }
        }

        private void Attack(Player target)
        {
            
            //Debug.Log("CAMERA SYSTEM: " + cameraSystem.transform.position);
            
            Debug.Log(target.name + " has been selected for attack.");
        
            System.Random rnd = new System.Random();

            int missFactor = rnd.Next(1, 11);

            if (missFactor < 3)
            {
                Instantiate(missFX, transform.position, Quaternion.identity);
                audioSource.PlayOneShot(missSound);
            }
            else
            {
                Instantiate(hitFX, target.transform.position, Quaternion.identity);
                target.health.decrement(attack.getValue());
                Debug.Log(target.name + " has taken " + attack.getValue() + " damage.");
                //target.setHP(-10);
                audioSource.PlayOneShot(hitSound);
            }
            isTurn = false;
        }

        private void Death()
        {
            Debug.Log(name + " is dead.");
            anim.SetBool("Dieing", true);
            Instantiate(DeathFX, transform.position, Quaternion.identity);
            isDead = true;
        }


        void CheckMouse()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "GridSquare")
                    {
                        GridSquare gridSquare = hit.collider.GetComponent<GridSquare>();

                        if (gridSquare.selectable)
                        {
                            MoveToGridSquare(gridSquare);
                        }
                    }
                }
            }
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            showEnemyStatDisplay();
        }

        public override void SetGridSquare(GridSquare square)
        {
            if (currentGridSquare != null)
            {
                currentGridSquare.current = false;
            }

            base.SetGridSquare(square);
            //currentGridSquare.current = true;
            isTurn = false;
        }

        public override void FindSelectableTiles()
        {
            if (currentGridSquare)
            {
                currentGridSquare.current = true;
            }

            base.FindSelectableTiles();
        }

        public override void Move()
        {
            anim.SetInteger("AnimationPar", 1);

            base.Move();

            if (moving == false)
            {
                anim.SetInteger("AnimationPar", 0);
                reachedDestination = false;
                moveTargetSquare.target = false;
            }
        }

        public override void MoveToGridSquare(GridSquare gridSquare)
        {
            path.Clear();
            gridSquare.target = true;
            moving = true;

            List<GridSquare> AList = PathToTarget(gridSquare);

            AList.RemoveAt(0);
            //int newLength = Mathf.Min(AList.Count, move);
            int newLength = (int)Mathf.Min(AList.Count, agility.getValue());
            AList = AList.GetRange(AList.Count - newLength, newLength);

            path = new Stack<GridSquare>(AList);
            

        }

        public int getHP()
        {
            return healthPoints;
        }

        public void setHP(int points)
        {
            healthPoints += points;
        }
        
        public void updateEnemyStatDisplay()
        {
            TextMeshProUGUI statDisplay = enemyMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            statDisplay.text = statsToString();
        }
        
        public void hideEnemyStatDisplay()
        {
            enemyMenu.gameObject.SetActive(false);
        }
        
        public void showEnemyStatDisplay()
        {
            updateEnemyStatDisplay();
            enemyMenu.gameObject.SetActive(true);
        }
        
        public string statsToString()
        {
            string r = eName + "\n\n";
            r += "HP  // " + health.getValue();
            r += "\nDEF // " + defense.getValue();
            return r;
        }
    }
}