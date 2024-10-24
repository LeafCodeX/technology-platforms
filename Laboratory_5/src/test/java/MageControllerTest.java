import com.laboratory5.Mage;
import com.laboratory5.MageController;
import com.laboratory5.MageRepository;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mockito;
import java.util.Optional;
import static org.mockito.Mockito.*;

public class MageControllerTest {
    private MageRepository mages;
    private MageController mageController;

    @BeforeEach
    public void init() {
        mages = Mockito.mock(MageRepository.class);
        mageController = new MageController(mages);
    }

    @Test
    public void deleteExist() {
        Assertions.assertEquals(">> [RETURN] Done!", mageController.delete("Mage1"));
    }

    @Test
    public void deleteNotExist() {
        doThrow(IllegalArgumentException.class)
                .when(mages)
                .delete(anyString());
        Assertions.assertEquals(">> [RETURN] Not found!", mageController.delete("Mage1"));
    }

    @Test
    public void findNotExist() {
        when(mages.find(anyString())).thenReturn(Optional.empty());
        Assertions.assertEquals(">> [RETURN] Not found!", mageController.find(anyString()));
    }

    @Test
    public void findExist() {
        String name = "Mage1";
        int level = 50;
        Mage expectedMage = new Mage(name, level);
        when(mages.find(name)).thenReturn(Optional.of(expectedMage));
        String actualMage = mageController.find(name);
        Assertions.assertEquals(expectedMage.toString(), actualMage);
        //Assertions.assertEquals(">> [RETURN] Done!", actualMage);
    }

    @Test
    public void addNotExist() {
        Assertions.assertDoesNotThrow(() -> mageController.save("Mage1", 50));
        Assertions.assertEquals(">> [RETURN] Done!", mageController.save("Mage1", 50));
    }

    @Test
    public void addExist() {
        doThrow(IllegalArgumentException.class)
                .when(mages)
                .save(any(Mage.class));
        Assertions.assertEquals(">> [RETURN] Bad request!", mageController.save("Mage1", 100));
    }
}
