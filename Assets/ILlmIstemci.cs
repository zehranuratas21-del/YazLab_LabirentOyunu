using System;
using System.Collections;
using System.Collections.Generic;

// Sohbetteki tek bir mesaj (system / user / assistant)
public class LlmMesaj
{
    public string rol;
    public string icerik;

    public LlmMesaj(string rol, string icerik)
    {
        this.rol = rol;
        this.icerik = icerik;
    }
}

// LLM isteğinin sonucu: ya ham metin ya da hata mesajı
public class LlmSonuc
{
    public bool basarili;
    public string metin;
    public string hataMesaji;

    public static LlmSonuc Basari(string metin)
    {
        return new LlmSonuc { basarili = true, metin = metin };
    }

    public static LlmSonuc Hata(string mesaj)
    {
        return new LlmSonuc { basarili = false, hataMesaji = mesaj };
    }
}

// Dil modeline erişimi soyutlar. Yarın Ollama yerine başka bir yerel
// sunucu kullanılırsa sadece yeni bir sınıf yazmak yeterli.
public interface ILlmIstemci
{
    IEnumerator CevapIste(List<LlmMesaj> mesajlar, Action<LlmSonuc> bitince);
}
