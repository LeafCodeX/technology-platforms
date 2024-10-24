package com.laboratory4;

import com.laboratory4.classes.Mage;
import com.laboratory4.classes.Tower;
import org.hibernate.boot.registry.StandardServiceRegistryBuilder;
import org.hibernate.boot.registry.StandardServiceRegistry;
import org.hibernate.boot.MetadataSources;
import org.hibernate.SessionFactory;
import java.util.List;
import java.util.ArrayList;

public class Main {

    private static SessionFactory getHibernateSessionFactory(StandardServiceRegistry registry) {
        return new MetadataSources(registry)
                .addAnnotatedClasses(Mage.class, Tower.class)
                .buildMetadata()
                .buildSessionFactory();
    }

    private static void seedData(SessionFactory sessionFactory) {
        var session = sessionFactory.openSession();
        session.beginTransaction();
        var mage1 = new Mage("Mage_1", 75);
        var mage2 = new Mage("Mage_2", 40);
        var mage3 = new Mage("Mage_3", 25);
        var tower1 = new Tower("Tower_1", 50);
        tower1.addMage(mage1);
        mage1.setTower(tower1);
        tower1.addMage(mage2);
        mage2.setTower(tower1);
        var tower2 = new Tower("Tower_2", 75);
        tower2.addMage(mage3);
        mage3.setTower(tower2);
        session.persist(mage1);
        session.persist(mage2);
        session.persist(mage3);
        session.persist(tower1);
        session.persist(tower2);
        session.getTransaction().commit();
        session.close();
    }

    public static void main(String[] args) {
        final var registry = new StandardServiceRegistryBuilder()
                .build();
        try (var sessionFactory = getHibernateSessionFactory(registry)){
            seedData(sessionFactory);

            var shouldRun = true;
            var scanner = new java.util.Scanner(System.in);
            while(shouldRun) {
                System.out.println("=================================================================================");
                System.out.println(">> 1. NewMage - Creates a new magician!                                        <<");
                System.out.println(">> 2. NewTower - Creates a new tower!                                          <<");
                System.out.println(">> 3. DeleteMage - Deletes an existing magician!                               <<");
                System.out.println(">> 4. DeleteTower - Deletes an existing tower!                                 <<");
                System.out.println(">> 5. MageToTower - Assigns an existing magician to the tower!                 <<");
                System.out.println(">> 6. MageFromTower - Removes an existing magician from the tower!             <<");
                System.out.println(">> 7. Print - Prints all magicians and towers!                                 <<");
                System.out.println(">> 8. Lvl - Print all mages with a level higher than ...                       <<");
                System.out.println(">> 9. LvlInTower - Print all mages in the tower with a level higher than ...   <<");
                System.out.println(">> 10. TowerHeight - Print all towers with a height lower than ...             <<");
                System.out.println(">> 100. Exit - Exit the program!                                               <<");
                System.out.println("=================================================================================");
                System.out.print(">> Enter a command: ");
                var command = scanner.nextLine();
                //for (int i = 0; i < 50; i++) {
                //    System.out.println();
                //}
                if (command.equals("Exit")) {
                    shouldRun = false;
                } else if (command.equals("NewMage")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter mage name: ");
                    var mageName = scanner.nextLine();
                    var existingMage = session.get(Mage.class, mageName);
                    if (existingMage != null) {
                        System.out.println(">> [ERROR] Mage " + mageName + " already exists!");
                    } else {
                        System.out.print(">> Enter mage level: ");
                        while (!scanner.hasNextInt()) {
                            System.out.print(">> [ERROR] That's not a number! Please enter a number: ");
                            scanner.next();
                        }
                        var mageLevel = scanner.nextInt();
                        scanner.nextLine();
                        var mage = new Mage(mageName, mageLevel);
                        System.out.print(">> Enter tower name: ");
                        var towerName = scanner.nextLine();
                        if (!towerName.isEmpty()) {
                            var tower = session.get(Tower.class, towerName);
                            if (tower != null) {
                                tower.addMage(mage);
                                mage.setTower(tower);
                                session.persist(tower);
                            } else {
                                System.out.println(">> [ERROR] Tower " + towerName + " does not exist!");
                            }
                        }
                        session.persist(mage);
                        session.getTransaction().commit();
                        System.out.println(">> [INFO] Mage " + mageName + " has been created!");
                    }
                    session.close();
                } else if (command.equals("NewTower")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter tower name: ");
                    var towerName = scanner.nextLine();
                    var existingTower = session.get(Tower.class, towerName);
                    if (existingTower != null) {
                        System.out.println(">> [ERROR] Tower " + towerName + " already exists!");
                    } else {
                        System.out.print(">> Enter tower height: ");
                        while (!scanner.hasNextInt()) {
                            System.out.print(">> [ERROR] That's not a number! Please enter a number: ");
                            scanner.next();
                        }
                        var towerHeight = scanner.nextInt();
                        scanner.nextLine();
                        var tower = new Tower(towerName, towerHeight);
                        session.persist(tower);
                        session.getTransaction().commit();
                        System.out.println(">> [INFO] Tower " + towerName + " has been created!");
                    }
                    session.close();
                } else if (command.equals("DeleteMage")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter mage name: ");
                    var mageName = scanner.nextLine();
                    var mage = session.get(Mage.class, mageName);
                    if (mage != null) {
                        System.out.print(">> Enter tower name: ");
                        var towerName = scanner.nextLine();
                        var tower = session.get(Tower.class, towerName);
                        if (tower != null && tower.getMages().contains(mage)) {
                            tower.removeMage(mage);
                            mage.setTower(null);
                            session.persist(tower);
                        }
                        session.delete(mage);
                        session.getTransaction().commit();
                        System.out.println(">> [INFO] Mage " + mageName + " has been deleted!");
                    } else {
                        System.out.println(">> [ERROR] Mage " + mageName + " does not exist!");
                    }
                    session.close();
                } else if (command.equals("DeleteTower")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter tower name: ");
                    var towerName = scanner.nextLine();
                    var tower = session.get(Tower.class, towerName);
                    if (tower != null) {
                        for (Mage mage : tower.getMages()) {
                            mage.setTower(null);
                            session.persist(mage);
                        }
                        session.delete(tower);
                        session.getTransaction().commit();
                        System.out.println(">> [INFO] Tower " + towerName + " has been deleted!");
                    } else {
                        System.out.println(">> [INFO] Tower " + towerName + " does not exist!");
                    }
                    session.close();
                } else if (command.equals("MageToTower")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter mage name: ");
                    var mageName = scanner.nextLine();
                    var mage = session.get(Mage.class, mageName);
                    if (mage == null) {
                        System.out.println(">> [ERROR] Mage " + mageName + " does not exist!");
                    } else {
                        System.out.print(">> Enter tower name: ");
                        var towerName = scanner.nextLine();
                        var tower = session.get(Tower.class, towerName);
                        if (tower == null) {
                            System.out.println(">> [ERROR] Tower " + towerName + " does not exist!");
                        } else {
                            tower.addMage(mage);
                            mage.setTower(tower);
                            session.persist(tower);
                            session.getTransaction().commit();
                            System.out.println(">> [INFO] Mage " + mageName + " has been added to tower " + towerName + "!");
                        }
                    }
                    session.close();
                } else if (command.equals("MageFromTower")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter mage name: ");
                    var mageName = scanner.nextLine();
                    var mage = session.get(Mage.class, mageName);
                    if (mage == null) {
                        System.out.println(">> [ERROR] Mage " + mageName + " does not exist!");
                    } else {
                        System.out.print(">> Enter tower name: ");
                        var towerName = scanner.nextLine();
                        var tower = session.get(Tower.class, towerName);
                        if (tower == null) {
                            System.out.println(">> [ERROR] Tower " + towerName + " does not exist!");
                        } else if (!tower.getMages().contains(mage)) {
                            System.out.println(">> [ERROR] Mage " + mageName + " does not belong to tower " + towerName + "!");
                        } else {
                            tower.removeMage(mage);
                            mage.setTower(null);
                            session.persist(tower);
                            session.getTransaction().commit();
                            System.out.println(">> [INFO] Mage " + mageName + " has been removed from tower " + towerName + "!");
                        }
                    }
                    session.close();
                } else if (command.equals("Print")) {
                    var session = sessionFactory.openSession();
                    var mages = session.createQuery("from Mage", Mage.class).list();
                    var towers = session.createQuery("from Tower", Tower.class).list();
                    System.out.println(">> [INFO] MAGES: ");
                    mages.forEach(System.out::println);
                    System.out.println(">> [INFO] TOWERS: ");
                    towers.forEach(System.out::println);
                    session.close();
                } else if (command.equals("Lvl")){
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter level: ");
                    while (!scanner.hasNextInt()) {
                        System.out.print(">> [ERROR] That's not a number! Please enter a number: ");
                        scanner.next();
                    }
                    var level = scanner.nextInt();
                    scanner.nextLine();
                    var mages = session.createQuery("from Mage", Mage.class).list();
                    List<Mage> magesHigherLevel = new ArrayList<>();
                    List<Mage> magesLowerOrEqualLevel = new ArrayList<>();
                    for (Mage mage : mages) {
                        if (mage.getLevel() > level) {
                            magesHigherLevel.add(mage);
                        } else {
                            magesLowerOrEqualLevel.add(mage);
                        }
                    }
                    System.out.println(">> [INFO] Mages with level higher than " + level + ": ");
                    for (Mage mage : magesHigherLevel) {
                        System.out.println(mage);
                    }
                    System.out.println(">> [INFO] Mages with level lower or equal to " + level + ": ");
                    for (Mage mage : magesLowerOrEqualLevel) {
                        System.out.println(mage);
                    }
                    session.close();
                } else if (command.equals("LvlInTower")) {
                    var session = sessionFactory.openSession();
                    session.beginTransaction();
                    System.out.print(">> Enter tower name: ");
                    var towerName = scanner.nextLine();
                    var tower = session.get(Tower.class, towerName);
                    if (tower == null) {
                        System.out.println(">> [ERROR] Tower " + towerName + " does not exist!");
                    } else {
                        System.out.print(">> Enter level: ");
                        while (!scanner.hasNextInt()) {
                            System.out.print(">> [ERROR] That's not a number! Please enter a number: ");
                            scanner.next();
                        }
                        var level = scanner.nextInt();
                        scanner.nextLine();
                        var mages = tower.getMages();
                        List<Mage> magesHigherLevel = new ArrayList<>();
                        List<Mage> magesLowerLevel = new ArrayList<>();
                        for (Mage mage : mages) {
                            if (mage.getLevel() > level) {
                                magesHigherLevel.add(mage);
                            } else {
                                magesLowerLevel.add(mage);
                            }
                        }
                        System.out.println(">> [INFO] Mages with level higher than " + level + ": ");
                        for (Mage mage : magesHigherLevel) {
                            System.out.println(mage);
                        }
                        System.out.println(">> [INFO] Mages with level lower or equal to " + level + ": ");
                        for (Mage mage : magesLowerLevel) {
                            System.out.println(mage);
                        }
                    }
                    session.close();
                } else if (command.equals("TowerHeight")) {
                    var session = sessionFactory.openSession();
                    System.out.print(">> Enter height: ");
                    while (!scanner.hasNextInt()) {
                        System.out.print(">> [ERROR] That's not a number! Please enter a number: ");
                        scanner.next();
                    }
                    var height = scanner.nextInt();
                    scanner.nextLine();
                    var towers = session.createQuery("from Tower", Tower.class).list();
                    List<Tower> towerLowerOrEqualHeight = new ArrayList<>();
                    List<Tower> towerHigher = new ArrayList<>();
                    for (Tower tower : towers) {
                        if (tower.getHeight() < height) {
                            towerLowerOrEqualHeight.add(tower);
                        } else {
                            towerHigher.add(tower);
                        }
                    }
                    System.out.println(">> [INFO] Towers with height lower than " + height + ": ");
                    for (Tower tower : towerLowerOrEqualHeight) {
                        System.out.println(tower);
                    }
                    System.out.println(">> [INFO] Towers with height higher or equal to " + height + ": ");
                    for (Tower tower : towerHigher) {
                        System.out.println(tower);
                    }
                    session.close();
                } else {
                    System.out.println(">> [INFO] Please enter the correct command!");
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
            StandardServiceRegistryBuilder.destroy(registry);
        }
    }
}