using System;

public class StatModifier {
    public float value;
    public Boolean isFlat; //if this is true, modifier adds a flat amount to the base stat. otherwise it is a percentage


    public StatModifier(float value, Boolean isFlat) {
        this.value = value;
        this.isFlat = isFlat;
    }
}
