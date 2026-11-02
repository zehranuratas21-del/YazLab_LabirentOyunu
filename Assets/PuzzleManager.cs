using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject mathPuzzleDoor; 
    private int totalScore = 0;
    private int targetScore = 5; // hedefimiz

    public void CollectItem(string itemName, GameObject itemObject)
    {
        if (itemName.Contains("2"))
        {
            totalScore += 2;
            Debug.Log("Item_Value2 toplandý! Puan: 2");
        }
        else if (itemName.Contains("3"))
        {
            totalScore += 3;
            Debug.Log("Item_Value3 toplandý! Puan: 3");
        }

        Debug.Log("Güncel Toplam Puan: " + totalScore);

        // objeyi kaldýrdý
        itemObject.SetActive(false);

       //kapý açýlan nokta
        if (totalScore >= targetScore && mathPuzzleDoor != null)
        {
            mathPuzzleDoor.SetActive(false);
            Debug.Log("Tebrikler! Matematik bulmacasý çözüldü, kapý açýldý.");
        }
    }
}
