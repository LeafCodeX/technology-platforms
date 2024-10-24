package com.laboratory5;

public class Main {
    public static void main(String[] args) {

        MageRepository mageRepository = new MageRepository();
        MageController mageController = new MageController(mageRepository);

        System.out.println("=========================================");
        System.out.println("1. Tru to delete a non-existing Mage:");
        System.out.println(mageController.delete("Mage1")); // should print "not found"

        System.out.println("=========================================");
        System.out.println("2. Save a new Mage:");
        System.out.println(mageController.save("Mage1", 50)); // should print "done"

        System.out.println("=========================================");
        System.out.println("3. Try to save a Mage with the same name:");
        System.out.println(mageController.save("Mage1", 100)); // should print "bad request"

        System.out.println("=========================================");
        System.out.println("4. Find an existing Mage:");
        System.out.println(mageController.find("Mage1")); // should print ">> Mage -> (name{Gandalf}, level{5})"

        System.out.println("=========================================");
        System.out.println("5. Try to find a non-existing Mage:");
        System.out.println(mageController.find("Mage2")); // should print "not found"

        System.out.println("=========================================");
        System.out.println("6. Delete an existing Mage:");
        System.out.println(mageController.delete("Mage1")); // should print "done"

        System.out.println("=========================================");
        System.out.println("7. Try to delete a non-existing Mage:");
        System.out.println(mageController.delete("Mage1")); // should print "not found"

        System.out.println("=========================================");
    }
}