using UnityEngine;

public class Salter : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject bariyer;          // Kaldýrýlacak engel duvarý
    public Transform ajanTransform;     // Sahnedeki Agent objesini buraya sürükleyeceðiz
    public float tetiklenmeMesafesi = 1.5f; // Ajan þaltere ne kadar yaklaþýnca açýlsýn?

    private bool salterAktif = false;

    void Update()
    {
        if (!salterAktif && ajanTransform != null)
        {
            // Ajan ile þalter arasýndaki mesafeyi hesaplýyoruz
            float mesafe = Vector3.Distance(transform.position, ajanTransform.position);

            // Eðer ajan belirlenen mesafeden daha yakýnsa þalter tetiklenir
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
