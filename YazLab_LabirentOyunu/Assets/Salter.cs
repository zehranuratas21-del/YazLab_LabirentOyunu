using UnityEngine;

public class Salter : MonoBehaviour
{
    // Þaltere basýldýðýnda açýlacak olan bariyeri buraya sürükleyeceðiz
    public GameObject bariyer;

    // Þaltere basýlýp basýlmadýðýný takip etmek için
    private bool salterAktif = false;

    // Ajan þaltere yaklaþýp etkileþime girdiðinde bu fonksiyon çalýþacak
    public void SalteriAktiflestir()
    {
        if (!salterAktif)
        {
            salterAktif = true;
            Debug.Log("Þaltere basýldý! Bariyer açýlýyor...");

            // Bariyeri sahneden kaldýrýyoruz (kapatýyoruz)
            if (bariyer != null)
            {
                bariyer.SetActive(false);
            }
        }
    }
}
