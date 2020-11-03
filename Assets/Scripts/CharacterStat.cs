using System;
using System.Collections.Generic;

public class CharacterStat {
    private float baseValue;
    private float finalValue;

    private Boolean isModified;
    private List<StatModifier> statModifiers;

    public float getValue(){
        if (isModified) {
            return calculateFinalValue();
        }
        else {
            return baseValue;
        }
    }

    public CharacterStat(float baseValue) {
        this.baseValue = baseValue;
        statModifiers = new List<StatModifier>();
    }

    public void setModified(Boolean isModified) {
        this.isModified = isModified;
    }

    public void addModifier(StatModifier mod) {
        this.statModifiers.Add(mod);
    }

    public void removeModifier(StatModifier mod) {
        this.statModifiers.Remove(mod);
    }

    public void clearModifiers() {
        this.statModifiers.Clear();
    }

    public void increment(float valueToAdd)
    {
        this.baseValue += valueToAdd;
    }

    public void decrement(float valueToSubtract)
    {
        this.baseValue -= valueToSubtract;
    }

    public void decrementModDurations()
    {
        foreach (StatModifier mod in this.statModifiers)
        {
            mod.remainingDuration -= 1;
        }
    }

    public void checkModDurations()
    {
        List<StatModifier> modsToDelete = new List<StatModifier>();

        foreach (StatModifier mod in this.statModifiers)
        {
            if (mod.remainingDuration <= 0)
            {
                modsToDelete.Add(mod);
            }
        }
        foreach (StatModifier m in modsToDelete)
        {
            this.statModifiers.Remove(m);
        }
        
        if (this.statModifiers.Count == 0)
        {
            setModified(false);
        }
    }

    public float calculateFinalValue() {
        float final = 0;
        foreach (StatModifier mod in this.statModifiers) {
            if (mod.isFlat) {
                final += mod.value;
            }
            else {
                final += baseValue * mod.value;
            }
        }
        return (float) Math.Round(final, 4);
    }
}
