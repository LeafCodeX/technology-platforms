package com.laboratory5;

import java.util.ArrayList;
import java.util.Optional;
import java.util.Collection;

public class MageRepository {
    final private Collection<Mage> mages;

    public MageRepository() {
        mages = new ArrayList<>();
    }

    public Optional<Mage> find(String name) {
        Mage searchedMage = null;
        for(Mage mage : mages) {
            if(mage.getName().equals(name)) {
                searchedMage = mage;
                break;
            }
        }
        // Próba pobrania nieistniejącego obiektu zwraca pusty obiekt Optional
        if(searchedMage == null) {
            return Optional.empty();
        }
        // Próba pobrania istniejącego obiektu zwraca obiekt Optional z zawartością
        else {
            return Optional.of(searchedMage);
        }
    }

    public void delete(String name) throws IllegalArgumentException {
        Mage searchedMage = null;
        for(Mage mage : mages) {
            if(mage.getName().equals(name)) {
                searchedMage = mage;
                break;
            }
        }
        // Próba usunięcia nieistniejącego obiektu powoduje IllegalArgumentException
        if(searchedMage == null) {
            throw new IllegalArgumentException();
        }
        // Jeśli maga znaleziono, usuwamy go z kolekcji
        else {
            mages.remove(searchedMage);
        }
    }

    public void save(Mage newMage) throws IllegalArgumentException {
        // Próba zapisania obiektu, którego klucz główny już znajduje się w repozytorium powoduje IllegalArgumentException
        for(Mage mage : mages) {
            if(mage.getName().equals(newMage.getName())) {
                throw new IllegalArgumentException();
            }
        }
        // Jeśli maga nie ma w kolekcji, dodajemy go
        mages.add(newMage);
    }
}