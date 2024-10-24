import com.laboratory5.Mage;
import com.laboratory5.MageRepository;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertThrows;

public class MageRepositoryTest {
    @Test
    @DisplayName("Tests removing non existing object")
    public void removeNotExist() {
        MageRepository mages = new MageRepository();
        assertThrows(IllegalArgumentException.class, () -> {
            mages.delete("Mage1");
        });
    }

    @Test
    @DisplayName("Tests finding non existing object")
    public void findNotExist() {
        MageRepository mages = new MageRepository();
        assert mages.find("Mage1").isEmpty() : true;
    }

    @Test
    @DisplayName("Tests finding existing object")
    public void findExist() {
        MageRepository mages = new MageRepository();
        mages.save(new Mage("Mage1", 50));
        assert mages.find("Mage1").isPresent() : true;
    }

    @Test
    @DisplayName("Tests adding already existing object")
    public void addExist() {
        MageRepository mages = new MageRepository();
        mages.save(new Mage("Mage1", 50));
        assertThrows(IllegalArgumentException.class, () -> {
            mages.save(new Mage("Mage1", 50));
        });
    }
}
