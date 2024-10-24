package com.laboratory2;

import java.util.LinkedList;
import java.util.Queue;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class PrimesList {
    private final Queue<Long> primes = new LinkedList<>(); 
    private final Lock lock = new ReentrantLock();

    public synchronized void addPrime(long prime) {
        lock.lock();
        try {
            primes.add(prime);
        } finally {
            lock.unlock();
        }
        notify();
    }

    public synchronized long getPrime() throws InterruptedException {
        while (primes.isEmpty()) {
            wait(500);
        }
        long prime = -1;
        lock.lock();
        try {
            prime = primes.poll();
        } finally {
            lock.unlock();
        }

        return prime;
    }
}