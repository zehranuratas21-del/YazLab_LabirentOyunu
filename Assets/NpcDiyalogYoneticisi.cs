using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Diyaloğun akışını yönetir: oyuncu yaklaşınca başlatır, mesajı LLM'e yollar,
// cevabı ayrıştırır, ekrana basar ve eylemi tetikler.
public class NpcDiyalogYoneticisi : MonoBehaviour
{
    [Header("Bağlantılar")]
    public NpcDiyalogArayuzu arayuz;
    public NpcEylemUygulayici eylemUygulayici;

    [Header("Ollama")]
    public string ollamaAdresi = "http://localhost:11434/api/chat";
    public string modelAdi = "llama3.1";
    public int zamanAsimiSaniye = 90;

    [Header("Ayarlar")]
    public string oyuncuTag = "Agent";
    public float konusmaMesafesi = 3f;
    public int maksGecmis = 12;
    public float eylemGecikmesi = 3f;   // son cevap okunsun diye bekleme

    [Header("Karakter")]
    [TextArea] public string acilisSozu =
        "Dur yolcu. Bu kapı herkese açılmaz. Kimsin ve neden geçmek istiyorsun?";

    [TextArea(8, 20)] public string sistemPromptu =
        "Sen Bekçi adında bir NPC'sin. Bir labirentin çıkışındaki kilitli kapıyı korursun. " +
        "Karakterin: yaşlı, ciddi, hafif alaycı ama adil. Amacın, kapıyı sadece geçmeyi hak eden birine açmaktır. " +
        "Kurallar: " +
        "1) Her zaman Türkçe konuş ve cevabın en fazla 2 kısa cümle olsun. " +
        "2) Oyuncu nazikse, samimiyse ve geçmek için mantıklı bir sebep söylüyorsa onu ikna olmuş say: duygun mutlu olsun, vedalaş ve eylem 'kapiyi_ac' olsun. " +
        "3) Oyuncu ilk mesajında hemen ikna olma, önce bir soru sor ya da biraz test et. " +
        "4) Oyuncu hakaret ederse ya da tehdit ederse kızgın ol (veya tehdit ciddiyse korkmuş ol), eylem 'yok' olsun. " +
        "5) Oyuncu alakasız ya da saçma konuşursa şaşkın ol, eylem 'yok' olsun. " +
        "6) Önceki konuşmayı hatırla. Seni kızdırdıysa bir süre soğuk kal, içtenlikle özür dilerse yumuşa. " +
        "7) Karakterden çıkma. " +
        "Cevabını SADECE şu JSON biçiminde ver: " +
        "{\"cevap\": \"konuşma metni\", \"duygu\": \"mutlu|kizgin|notr|korkmus|saskin\", \"eylem\": \"yok|kapiyi_ac\"}";

    ILlmIstemci istemci;
    ICevapAyristirici ayristirici;
    readonly List<LlmMesaj> gecmis = new List<LlmMesaj>();

    Transform oyuncu;
    NavMeshAgent oyuncuAgent;
    bool konusuyor = false;
    bool bekliyor = false;
    bool bitti = false;

    void Start()
    {
        istemci = new OllamaIstemci(ollamaAdresi, modelAdi, zamanAsimiSaniye);
        ayristirici = new JsonCevapAyristirici();

        GameObject o = GameObject.FindGameObjectWithTag(oyuncuTag);
        if (o != null)
        {
            oyuncu = o.transform;
            oyuncuAgent = o.GetComponent<NavMeshAgent>();
        }
        else
        {
            Debug.LogError("NpcDiyalogYoneticisi: '" + oyuncuTag + "' tagli obje bulunamadı!");
        }

        if (arayuz != null) arayuz.MesajGonderildi += OyuncuMesajiAlindi;
    }

    void OnDestroy()
    {
        if (arayuz != null) arayuz.MesajGonderildi -= OyuncuMesajiAlindi;
    }

    void Update()
    {
        if (bitti || konusuyor || oyuncu == null) return;

        if (Vector3.Distance(transform.position, oyuncu.position) <= konusmaMesafesi)
            DiyalogBaslat();
    }

    void DiyalogBaslat()
    {
        konusuyor = true;
        arayuz.Ac(acilisSozu);

        if (oyuncuAgent != null) oyuncuAgent.isStopped = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OyuncuMesajiAlindi(string metin)
    {
        if (!konusuyor || bekliyor || bitti) return;
        StartCoroutine(MesajIsle(metin));
    }

    IEnumerator MesajIsle(string metin)
    {
        bekliyor = true;
        arayuz.DusunuyorGoster();

        gecmis.Add(new LlmMesaj("user", metin));
        GecmisiKisalt();

        List<LlmMesaj> liste = new List<LlmMesaj>();
        liste.Add(new LlmMesaj("system", sistemPromptu));
        liste.AddRange(gecmis);

        LlmSonuc sonuc = null;
        yield return istemci.CevapIste(liste, s => sonuc = s);

        NpcCevap cevap = null;
        bool eylemVar = false;

        if (sonuc == null || !sonuc.basarili)
        {
            // Sunucu kapalı, model yok ya da zaman aşımı: oyun devam eder
            gecmis.RemoveAt(gecmis.Count - 1);
            arayuz.HataGoster(sonuc != null ? sonuc.hataMesaji : "Bilinmeyen bir hata oluştu.");
        }
        else if (ayristirici.Ayristir(sonuc.metin, out cevap))
        {
            gecmis.Add(new LlmMesaj("assistant", sonuc.metin));
            arayuz.CevapGoster(cevap.cevap);
            eylemVar = cevap.eylem != "yok";
        }
        else
        {
            // Bozuk JSON: oyuncudan tekrar denemesi istenir
            gecmis.RemoveAt(gecmis.Count - 1);
            arayuz.HataGoster("Bekçi'nin sözü anlaşılamadı. Tekrar dene.");
        }

        if (eylemVar && eylemUygulayici != null)
        {
            bitti = true;
            yield return new WaitForSeconds(eylemGecikmesi);
            arayuz.Kapat();
            if (oyuncuAgent != null) oyuncuAgent.isStopped = false;
            eylemUygulayici.Uygula(cevap.eylem);
            yield break;
        }

        bekliyor = false;
        arayuz.GirisiAc(true);
    }

    // Geçmiş çok uzarsa en eski soru-cevap çiftini at
    void GecmisiKisalt()
    {
        while (gecmis.Count > maksGecmis && gecmis.Count >= 2)
            gecmis.RemoveRange(0, 2);
    }
}
