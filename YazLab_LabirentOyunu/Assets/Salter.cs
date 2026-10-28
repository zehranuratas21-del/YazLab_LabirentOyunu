using UnityEngine;

public class Salter : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject bariyer;          
    public Transform ajanTransform;    
    public float tetiklenmeMesafesi = 1.5f; 

    private bool salterAktif = false;

    void Update()
    {
        if (!salterAktif && ajanTransform != null)
        {
            // mesafeyi hasaplama
            float mesafe = Vector3.Distance(transform.position, ajanTransform.position);

            
            if (mesafe <= tetiklenmeMesafesi)
            {
                SalteriAktiflestir();
            }
        }
    }

    public void SalteriAktiflestir()
    {
        salterAktif = true;
        Debug.Log("Þaltere basýldý! Bariyer açýlýyor...");

        if (bariyer != null)
        {
            bariyer.SetActive(false); // Engeli gizle
        }
    }
}
