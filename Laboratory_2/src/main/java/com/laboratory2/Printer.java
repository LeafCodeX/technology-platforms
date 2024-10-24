package com.laboratory2;

import java.util.List;

public class Printer implements Runnable {
    private final ResultPrimes results;

    public Printer(ResultPrimes results) {
        this.results = results;
    }

    @Override
    public void run() {
        List<Long> list;
        List<Long> lastList = results.getResults();
        try {
            while (true) {
                Thread.sleep(5500);
                list = results.getResults();
                if (list.equals(lastList)) {
                    continue;
                }
                for (int i = 0; i < list.size(); i++) {
                    if (i == 0 && i == list.size() - 1) {
                        System.out.println("[RESULT] List: [P" + i + " = " + list.get(i) + "]");
                    } else if (i == 0) {
                        System.out.print("[RESULT] List: [P" + i + " = " + list.get(i) + "], ");
                    } else if (i == lastList.size()) {
                        System.out.println("[P" + i + " = " + list.get(i) + "].");
                    } else {
                        System.out.print("[P" + i + " = " + list.get(i) + "], ");
                    }
                }
                lastList = list;
            }
        } catch (InterruptedException e) {
            System.out.println("Shutting down printer.");
        }
    }
}