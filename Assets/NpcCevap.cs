using System;
using UnityEngine;

// Modelin döndürmesi beklenen JSON'un karşılığı
[Serializable]
public class NpcCevap
{
    public string cevap;   // oyuncuya gösterilen konuşma
    public string duygu;   // mutlu | kizgin | notr | korkmus | saskin
    public string eylem;   // yok | kapiyi_ac
}

// Ham metni NpcCevap'a çevirme sorumluluğu
public interface ICevapAyristirici
{
    bool Ayristir(string hamMetin, out NpcCevap sonuc);
}

// JSON ayrıştırır. Bozuk ya da eksik çıktıda false döner, oyun çökmez.
public class JsonCevapAyristirici : ICevapAyristirici
{
    static readonly string[] GecerliEylemler = { "yok", "kapiyi_ac" };

    public bool Ayristir(string hamMetin, out NpcCevap sonuc)
    {
        sonuc = null;
        if (string.IsNullOrWhiteSpace(hamMetin)) return false;

        string json = JsonKesitiBul(hamMetin);
        if (json == null) return false;

        try
        {
            sonuc = JsonUtility.FromJson<NpcCevap>(json);
        }
        catch (Exception)
        {
            sonuc = null;
            return false;
        }

        if (sonuc == null || string.IsNullOrWhiteSpace(sonuc.cevap))
        {
            sonuc = null;
            return false;
        }

        // Bilinmeyen eylem gelirse güvenli tarafta kal: hiçbir şey yapma
        if (Array.IndexOf(GecerliEylemler, sonuc.eylem) < 0) sonuc.eylem = "yok";
        if (string.IsNullOrWhiteSpace(sonuc.duygu)) sonuc.duygu = "notr";
        return true;
    }

    // Model JSON'un etrafına fazladan yazı koyarsa ilk { ile son } arasını al
    string JsonKesitiBul(string metin)
    {
        int bas = metin.IndexOf('{');
        int son = metin.LastIndexOf('}');
        if (bas < 0 || son <= bas) return null;
        return metin.Substring(bas, son - bas + 1);
    }
}
