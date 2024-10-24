package com.laboratory2;

import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class ResultPrimes {
    private final List<Long> results = new LinkedList<>();
    private final Lock lock = new ReentrantLock();

    public void addResult(long result) {
        lock.lock();
        try {
            results.add(result);
        } finally {
            lock.unlock();
        }
    }

    public List<Long> getResults() {
        lock.lock();
        try {
            return new LinkedList<>(results);
        } finally {
            lock.unlock();
        }
    }
}