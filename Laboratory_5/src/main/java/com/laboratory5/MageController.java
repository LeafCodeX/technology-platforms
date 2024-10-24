package com.laboratory5;

public class MageController {
    final private MageRepository mages;

    public MageController(MageRepository mages) {
        this.mages = mages;
    }

    public String find(String name) {
        // Próba pobrania istniejącego obiektu zwraca obiekt String reprezentujący znaleziony obiekt encyjny
        try {
            Mage searchedMage = mages.find(name).orElseThrow();
            return searchedMage.toString();
        }
        // Próba pobrania nieistniejącego obiektu powoduje zwrócenie obiektu String o wartości "not found"
        catch (Exception e) {
            return ">> [RETURN] Not found!";
        }
    }

    public String delete(String name) {
        // Próba usunięcia istniejącego obiektu powoduje zwrócenie obiektu String o wartości "done"
        try {
            mages.delete(name);
            return ">> [RETURN] Done!";
        }
        // Próba usunięcia nieistniejącego obiektu powoduje zwrócenie obiektu String o wartości "not found"
        catch (IllegalArgumentException e) {
            return ">> [RETURN] Not found!";
        }
    }

    public String save(String name, int level) {
        // Próba zapisania nowego obiektu skutkuje wywołaniem metody z serwisu z poprawnym parametrem i zwróceniem obiektu String o wartości "done"
        try {
            mages.save(new Mage(name, level));
            return ">> [RETURN] Done!";
        }
        // Próba zapisania nowego obiektu, którego klucz główny znajduje się już w repozytorium powoduje zwrócenie obiektu String o wartości "bad request"
        catch (IllegalArgumentException e) {
            return ">> [RETURN] Bad request!";
        }
    }
}