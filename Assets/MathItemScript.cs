using UnityEngine;

public class MathItemScript : MonoBehaviour
{
    public PuzzleManager puzzleManager;

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan obje Ajan ise
        if (other.CompareTag("Agent"))
        {
            puzzleManager.CollectItem(gameObject.name, gameObject);
        }
    }
}
