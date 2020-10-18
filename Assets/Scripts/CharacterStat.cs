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
