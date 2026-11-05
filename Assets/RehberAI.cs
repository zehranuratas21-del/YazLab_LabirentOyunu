using UnityEngine;
using UnityEngine.AI;

public class RehberAI : MonoBehaviour
{
    public Transform hedef;                 // çıkıştaki nokta
    public Transform oyuncu;                // oyuncu objesi
    public float hiz = 5f;                  // oyuncuyla aynı hız
    public float beklemeMesafesi = 6f;      // oyuncu bundan uzaksa rehber bekler
    public bool oyuncuHareketEdinceGit = true; // açıksa sen yürüdükçe o da yürür

    NavMeshAgent agent;
    Vector3 sonOyuncuPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = hiz;
        sonOyuncuPos = oyuncu.position;
        agent.SetDestination(hedef.position);
    }

    void Update()
    {
        float mesafe = Vector3.Distance(transform.position, oyuncu.position);
        bool oyuncuHareketli = (oyuncu.position - sonOyuncuPos).sqrMagnitude > 0.0001f;
        sonOyuncuPos = oyuncu.position;

        bool dur = mesafe > beklemeMesafesi;
        if (oyuncuHareketEdinceGit && !oyuncuHareketli) dur = true;

        agent.isStopped = dur;
    }
}