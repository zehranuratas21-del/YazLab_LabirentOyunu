using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Yerel Ollama sunucusuyla konuşur (http://localhost:11434).
// Asenkron çalışır: coroutine içinde beklediği için oyun donmaz.
public class OllamaIstemci : ILlmIstemci
{
    [Serializable] class OllamaMesaj { public string role; public string content; }
    [Serializable] class OllamaYanit { public OllamaMesaj message; }

    // Modelin uyması gereken JSON şeması (yapılandırılmış çıktı)
    const string Sema =
        "{\"type\":\"object\",\"properties\":{" +
        "\"cevap\":{\"type\":\"string\"}," +
        "\"duygu\":{\"type\":\"string\",\"enum\":[\"mutlu\",\"kizgin\",\"notr\",\"korkmus\",\"saskin\"]}," +
        "\"eylem\":{\"type\":\"string\",\"enum\":[\"yok\",\"kapiyi_ac\"]}}," +
        "\"required\":[\"cevap\",\"duygu\",\"eylem\"]}";

    readonly string adres;
    readonly string model;
    readonly int zamanAsimiSaniye;

    public OllamaIstemci(string adres, string model, int zamanAsimiSaniye)
    {
        this.adres = adres;
        this.model = model;
        this.zamanAsimiSaniye = zamanAsimiSaniye;
    }

    public IEnumerator CevapIste(List<LlmMesaj> mesajlar, Action<LlmSonuc> bitince)
    {
        string govde = GovdeOlustur(mesajlar);

        using (UnityWebRequest istek = new UnityWebRequest(adres, "POST"))
        {
            istek.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(govde));
            istek.downloadHandler = new DownloadHandlerBuffer();
            istek.SetRequestHeader("Content-Type", "application/json");
            istek.timeout = zamanAsimiSaniye;

            yield return istek.SendWebRequest();

            if (istek.result == UnityWebRequest.Result.ConnectionError)
            {
                bitince(LlmSonuc.Hata("Ollama sunucusuna ulaşılamadı ya da cevap zaman aşımına uğradı. Ollama açık mı?"));
                yield break;
            }

            if (istek.result == UnityWebRequest.Result.ProtocolError)
            {
                if (istek.responseCode == 404)
                    bitince(LlmSonuc.Hata("Model bulunamadı. 'ollama pull " + model + "' ile indirdin mi?"));
                else
                    bitince(LlmSonuc.Hata("Sunucu hata verdi (kod " + istek.responseCode + ")."));
                yield break;
            }

            bitince(YaniTCoz(istek.downloadHandler.text));
        }
    }

    LlmSonuc YaniTCoz(string ham)
    {
        try
        {
            OllamaYanit yanit = JsonUtility.FromJson<OllamaYanit>(ham);
            if (yanit == null || yanit.message == null || string.IsNullOrEmpty(yanit.message.content))
                return LlmSonuc.Hata("Sunucudan boş cevap geldi.");
            return LlmSonuc.Basari(yanit.message.content);
        }
        catch (Exception e)
        {
            return LlmSonuc.Hata("Sunucu cevabı okunamadı: " + e.Message);
        }
    }

    string GovdeOlustur(List<LlmMesaj> mesajlar)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\"model\":\"").Append(Kacis(model)).Append("\",\"messages\":[");

        for (int i = 0; i < mesajlar.Count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append("{\"role\":\"").Append(Kacis(mesajlar[i].rol))
              .Append("\",\"content\":\"").Append(Kacis(mesajlar[i].icerik)).Append("\"}");
        }

        sb.Append("],\"stream\":false,\"format\":").Append(Sema)
          .Append(",\"keep_alive\":\"10m\",\"options\":{\"temperature\":0.7}}");
        return sb.ToString();
    }

    static string Kacis(string s)
    {
        if (s == null) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }
}
