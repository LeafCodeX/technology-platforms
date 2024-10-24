package com.laboratory4.classes;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.ManyToOne;

@Entity
public class Mage {
    @Id
    private String name;
    private int level;
    @ManyToOne
    private Tower tower;

    public Mage(String name, int level) {
        this.name = name;
        this.level = level;
    }

    public Mage() {

    }

    public void setTower(Tower tower) {
        this.tower = tower;
    }

    public int getLevel() {
        return this.level;
    }

    @Override
    public String toString() {
        return "Mage(" + "name{" + name + "}, lvl{" + level + "}, tower{" + (tower == null ? "NULL" : tower.getName()) + "})";
    }
}
