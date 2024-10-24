package com.laboratory1;

import java.util.Comparator;

public class MageComparator implements Comparator<Mage> {
    @Override
    public int compare(Mage mage1, Mage mage2) {
        if (mage1.level != mage2.level) {
            return Integer.compare(mage1.level, mage2.level);
        }

        if (!mage1.name.equals(mage2.name)) {
            return mage1.name.compareTo(mage2.name);
        }

        return Double.compare(mage1.power, mage2.power);
    }
}
