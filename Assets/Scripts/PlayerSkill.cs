using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerSkill
{
    public string name;
    public string description;

    public int cost;//how much AP to use skill
    public float power;//boost to skill power, also determines buff or debuff depending on whether its positive or negative
    public int modDuration;//how long stat mod to last
    public bool modType;//1 for flat type, 0 for percentage type mods

    public string type; //will be regen, mod, or attack
    public List<string> targetStatTypes; //list all affected stats

    public CharacterStat dependentStat; //which stat to calculate damage with, if applicable
    public List<CharacterStat> targetStats; //instances of stats that will be modified


    public PlayerSkill(string name, string desc, string type, List<string> tsTypes, CharacterStat dStat, int cost, float power, bool modType, int modDuration)
    {
        this.name = name;
        this.description = desc;

        this.type = type;
        this.targetStatTypes = tsTypes;
        this.targetStats = new List<CharacterStat>();

        this.dependentStat = dStat;
        this.cost = cost;
        this.power = power;
        this.modType = modType;
        this.modDuration = modDuration;
    }

    public void setTargetStatsPlayer(Player p)
    {
        foreach (string s in targetStatTypes)
        {
            switch (s)
            {
                case "HP":
                    this.targetStats.Add(p.health);
                    break;
                case "AP":
                    this.targetStats.Add(p.ap);
                    break;
                case "ATK":
                    this.targetStats.Add(p.attack);
                    break;
                case "DEF":
                    this.targetStats.Add(p.defense);
                    break;
                case "INT":
                    this.targetStats.Add(p.intelligence);
                    break;
                case "AGI":
                    this.targetStats.Add(p.agility);
                    break;
            }
        }
    }

    public void setTargetStatsEnemy(Enemy e)
    {
        foreach (string s in targetStatTypes)
        {
            switch (s)
            {
                case "HP":
                    this.targetStats.Add(e.health);
                    Debug.Log("Enemy health added to target stat list. Original value is " + e.health.getValue());
                    break;
                case "ATK":
                    this.targetStats.Add(e.attack);
                    Debug.Log("Enemy attack added to target stat list. Original value is " + e.attack.getValue());
                    break;
                case "DEF":
                    this.targetStats.Add(e.defense);
                    Debug.Log("Enemy defense added to target stat list. Original value is " + e.defense.getValue());
                    break;
                case "RES":
                    this.targetStats.Add(e.resistance);
                    Debug.Log("Enemy resistance added to target stat list. Original value is " + e.resistance.getValue());
                    break;
            }
        }
    }

    public void setPower(int power)
    {
        this.power = power; //to reassign power if needed
    }

    public void scrubTargetStats()
    {
        this.targetStats.Clear();
    }

    public void execute(Player activeCharacter, Enemy targetEnemy)
    {
        float basePower = dependentStat.getValue();
        bool isPhysical = dependentStat == activeCharacter.attack;

        activeCharacter.ap.decrement(this.cost);

        switch (type)
        {
            case "regen":
                foreach (CharacterStat stat in targetStats)
                {
                    Debug.Log("Stat original value is " + stat.getValue() + " with " + (basePower + power) + " to be added.");
                    stat.increment(basePower + power);
                };
                break;

            case "mod":
                foreach (CharacterStat stat in targetStats)
                {
                    Debug.Log("Stat mod value is " + basePower * power);
                    stat.addModifier(new StatModifier(basePower * power, modType, modDuration));
                    stat.setModified(true);
                    Debug.Log("Target Stat has been modified to be " + stat.getValue());
                };
                break;

            case "attack":
                foreach (CharacterStat stat in targetStats)
                {
                    //if physical damage, flat reduction with defense
                    if (isPhysical)
                    {
                        Debug.Log("Stat original value is " + stat.getValue());
                        stat.decrement(basePower + power -  targetEnemy.defense.getValue());
                        Debug.Log("Stat value is now " + stat.getValue());
                    }
                    //if int damage, percentage reduction with enemy resistance
                    else
                    {
                        Debug.Log("Stat original value is " + stat.getValue());
                        stat.decrement((basePower + power) * (1 - targetEnemy.resistance.getValue()*0.01f));
                        Debug.Log("Stat value is now " + stat.getValue());
                    }
                };
                break;
        }
    }

}
