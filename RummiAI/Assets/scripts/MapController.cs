using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameController mapGameController;  // Ka¿da mapa ma swój GameController

    private void Awake()
    {
        if (mapGameController != null)
        {
            // Ustawienie statycznej instancji GameController, ale pamiêtaj¹c o tym, ¿eby nie nadpisaæ istniej¹cej instancji
            if (GameController.Instance == null)
            {
                GameController.SetInstance(mapGameController);  // Przypisanie tylko wtedy, gdy Instance jest null
            }
            else
            {
                Debug.LogWarning("Instancja GameController ju¿ zosta³a ustawiona.");
            }
        }
        else
        {
            Debug.LogError("Brak przypisanego GameController do MapController!");
        }
    }
}
