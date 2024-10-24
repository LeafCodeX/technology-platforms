package com.laboratory5;

public class Mage {
    private final String name;
    private final int level;

    public Mage(String name, int level) {
        this.name = name;
        this.level = level;
    }

    public String getName() {
        return name;
    }

    @Override
    public String toString() {
        return ">> Mage -> (" +
                "name{" + name +
                "}, level{" + level +
                "})";
    }
}
