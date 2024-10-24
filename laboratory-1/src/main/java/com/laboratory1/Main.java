package com.laboratory1;

public class Main {
    public static void main(String[] args) {
        boolean isSorted = false;
        MageComparator comparator = null;

        if (args.length > 0) {
            switch (args[0]) {
                case "name":
                    isSorted = true;
                    break;
                case "level":
                    isSorted = true;
                    comparator = new MageComparator();
                    break;
                default:
                    // isSorted na false i comparator na null
            }
        }

        Mage mage1 = new Mage("M1", 1, 1, isSorted, comparator);
        Mage mage2 = new Mage("M2", 3, 3, isSorted, comparator);
        Mage mage3 = new Mage("M3", 5, 5, isSorted, comparator);
        Mage mage4 = new Mage("M4", 7, 7, isSorted, comparator);
        Mage mage5 = new Mage("M5", 11, 11, isSorted, comparator);
        Mage mage6 = new Mage("M6", 11, 11, isSorted, comparator);
        Mage mage7 = new Mage("M7", 13, 13, isSorted, comparator);
        Mage mage8 = new Mage("M8", 15, 15, isSorted, comparator);
        Mage mage9 = new Mage("M9", 17, 17, isSorted, comparator);
        Mage mage10 = new Mage("M10", 19, 19, isSorted, comparator);

        mage1.apprentices.add(mage2);
        mage2.apprentices.add(mage3);
        mage3.apprentices.add(mage6);
        mage3.apprentices.add(mage5);
        mage3.apprentices.add(mage4);
        mage2.apprentices.add(mage7);
        mage2.apprentices.add(mage8);
        mage1.apprentices.add(mage9);
        mage1.apprentices.add(mage10);

        mage1.print(0);
    }
}
