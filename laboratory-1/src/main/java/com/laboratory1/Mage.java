package com.laboratory1;

import java.util.Set;

public class Mage implements Comparable<Mage> {
    public final String name;
    public final int level;
    public final double power;
    public final Set<Mage> apprentices;

    public Mage(String name, int level, double power, boolean isSorted, MageComparator comparator) {
        this.name = name;
        this.level = level;
        this.power = power;

        if (isSorted) {
            this.apprentices = createSortedApprenticesSet(comparator);
        } else {
            this.apprentices = createUnsortedApprenticesSet();
        }
    }

    private Set<Mage> createSortedApprenticesSet(MageComparator comparator) {
        return (comparator != null) ? new java.util.TreeSet<>(comparator) : new java.util.TreeSet<>();
    }

    private Set<Mage> createUnsortedApprenticesSet() {
        return new java.util.HashSet<>();
    }


    public String toString() {
        return String.format("Mage{name='%s', level=%d, power=%.2f}", name, level, power);
    }

    public void print(int depth) {
        System.out.println("-".repeat(depth) + this);
        for (Mage apprentice : apprentices) {
            apprentice.print(depth + 1);
        }
    }

    @Override
    public int hashCode() {
        int prime1 = 31;
        int prime2 = 67;
        int prime3 = 97;
        int prime4 = 2137;

        int hashName = name.hashCode();
        int hashPower = (int) power;

        int result = prime1 * hashName;
        result += prime2 * level;
        result += prime3 * hashPower;
        result *= prime4;

        return result;
    }


    @Override
    public boolean equals(Object object) {
        if (!(object instanceof Mage mage)) {
            return false;
        }
        return level == mage.level && Double.compare(mage.power, power) == 0 && name.equals(mage.name);
    }

    @Override
    public int compareTo(Mage object) {
        return this.name.equals(object.name)
                ? this.level == object.level
                ? Double.compare(this.power, object.power)
                : Integer.compare(this.level, object.level)
                : this.name.compareTo(object.name);
    }

}