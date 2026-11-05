using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Sadece ekranı yönetir: panel, NPC yazısı, "Düşünüyor...", yazı kutusu.
// Bu script panelin kendisine DEĞİL, Canvas'a takılmalı.
public class NpcDiyalogArayuzu : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text npcMetni;
    public TMP_Text durumMetni;
    public TMP_InputField girisAlani;

    public event Action<string> MesajGonderildi;

    Coroutine noktaAnimasyonu;

    void Awake()
    {
        girisAlani.onSubmit.AddListener(Gonderildi);
        panel.SetActive(false);
    }

    public void Ac(string acilisMetni)
    {
        panel.SetActive(true);
        npcMetni.text = acilisMetni;
        durumMetni.text = "";
        GirisiAc(true);
    }

    public void Kapat()
    {
        DurumuTemizle();
        panel.SetActive(false);
    }

    public void DusunuyorGoster()
    {
        GirisiAc(false);
        DurumuTemizle();
        noktaAnimasyonu = StartCoroutine(DusunuyorAnimasyonu());
    }

    public void CevapGoster(string metin)
    {
        DurumuTemizle();
        npcMetni.text = metin;
    }

    public void HataGoster(string metin)
    {
        DurumuTemizle();
        durumMetni.text = metin;
    }

    public void GirisiAc(bool aktif)
    {
        girisAlani.interactable = aktif;
        if (aktif) girisAlani.ActivateInputField();
    }

    void Gonderildi(string metin)
    {
        if (string.IsNullOrWhiteSpace(metin))
        {
            girisAlani.ActivateInputField();
            return;
        }
        girisAlani.text = "";
        if (MesajGonderildi != null) MesajGonderildi(metin.Trim());
    }

    void DurumuTemizle()
    {
        if (noktaAnimasyonu != null)
        {
            StopCoroutine(noktaAnimasyonu);
            noktaAnimasyonu = null;
        }
        durumMetni.text = "";
    }

    IEnumerator DusunuyorAnimasyonu()
    {
        int n = 0;
        while (true)
        {
            durumMetni.text = "Düşünüyor" + new string('.', n % 4);
            n++;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
