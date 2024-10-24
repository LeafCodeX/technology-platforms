package com.laboratory2;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

public class AddPrime implements Runnable {
    private final PrimesList primesList;

    public AddPrime(PrimesList primesList) {
        this.primesList = primesList;
    }

    @Override
    public void run() {
        try {
            processFile("test-1-watki.txt");
            processFile("test-2-watki.txt");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void processFile(String filename) throws IOException {
        try (BufferedReader br = new BufferedReader(new FileReader(filename))) {
            String line;
            while ((line = br.readLine()) != null) {
                long number;
                try {
                    number = Long.parseLong(line.trim());
                } catch (NumberFormatException e) {
                    System.err.println("Error parsing number in file " + filename + ": " + e.getMessage());
                    continue;
                }

                primesList.addPrime(number);
                //System.out.println("[NUMBER_ADDED]: " + number);

                try {
                    Thread.sleep(1000); // Simulate some processing time before producing the next prime
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }
    }

    /*@Override
    public void run() {
        try (BufferedReader br = new BufferedReader(new FileReader("test-2-watki.txt"))) {
            String line;
            while ((line = br.readLine()) != null) {
                long number = Long.parseLong(line.trim());
                primesList.addPrime(number);
                //System.out.println("[NUMBER_ADDED]: " + number);

                try {
                    Thread.sleep(1000); // Simulate some processing time before producing the next prime
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        } catch (NumberFormatException e) {
            System.err.println("Error parsing number: " + e.getMessage());
        }
    }*/
}

/*public class AddPrime implements Runnable {
    private final PrimesList primesList;

    public AddPrime(PrimesList primesList) {
        this.primesList = primesList;
    }

    @Override
    public void run() {
        for (int i = 10; i < 21; i++) {
            primesList.addPrime(i);
            System.out.println("[NUMBER_ADDED]: " + i);

            try {
                Thread.sleep(1000); // Simulate some processing time before producing the next prime
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}*/
