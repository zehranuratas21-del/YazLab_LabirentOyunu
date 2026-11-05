using UnityEngine;

// "kapiyi_ac" eylemi gelince kapıyı açar ve oyunu bitirir.
public class KapiAcmaEylemi : NpcEylemUygulayici
{
    public GameObject kapi;             // açılacak kapı objesi
    public GameObject oyunSonuPaneli;   // "Tebrikler, oyun bitti" paneli

    bool tamamlandi = false;

    public override void Uygula(string eylem)
    {
        if (tamamlandi || eylem != "kapiyi_ac") return;
        tamamlandi = true;

        if (kapi != null) kapi.SetActive(false);
        if (oyunSonuPaneli != null) oyunSonuPaneli.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Kapı açıldı, oyun tamamlandı.");
    }
}
