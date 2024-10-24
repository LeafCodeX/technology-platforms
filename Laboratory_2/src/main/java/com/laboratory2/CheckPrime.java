package com.laboratory2;

import java.util.Random;

public class CheckPrime implements Runnable {
    private final PrimesList checkPrime;
    private final ResultPrimes results;
    private final String name;

    public CheckPrime(PrimesList checkPrime, ResultPrimes results, String name) {
        this.checkPrime = checkPrime;
        this.name = name;
        this.results = results;
    }

    @Override
    public void run() {
        while (true) {
            try {
                long prime = checkPrime.getPrime();
                Random random = new Random();
                try {
                    Thread.sleep(random.nextInt(501) + 500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
                printDivisorsAndCheckPrime(prime);
            } catch (InterruptedException e) {
                System.out.println("Shutting down checker.");
                break;
            }
        }
    }
    private void printDivisorsAndCheckPrime(long number) {
        if (number <= 1) {
            System.out.println("1: [1]");
            return;
        }

        System.out.print(number + ": [");
        for (long i = 1; i <= number; i++) {
            if (number % i == 0) {
                System.out.print(i);
                if (i != number) {
                    System.out.print(", ");
                }
            }
        }
        System.out.println("]");
    }

    private boolean checkPrime(long number) {
        if (number <= 1) {
            return false;
        }

        for (long i = 2; i <= Math.sqrt(number); i++) {
            if (number % i == 0) {
                return false;
            }
        }
        return true;
    }
}
