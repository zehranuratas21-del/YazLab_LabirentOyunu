using UnityEngine;
using UnityEngine.AI;

public class AjanHareketi : MonoBehaviour
{
    public Transform[] waypoints; // Sýrasýyla gidilecek hedefler (1. kutu, 2. kutu, son hedef)
    private int currentWaypointIndex = 0;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Ýlk hedefe yönlendir
        SetNextDestination();
    }

    void Update()
    {
        // Eðer hedefler listesi boþ deðilse ve ajan hedefe çok yaklaþtýysa bir sonraki hedefe geç
        if (waypoints.Length > 0 && currentWaypointIndex < waypoints.Length)
        {
            if (waypoints[currentWaypointIndex] != null && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentWaypointIndex++;
                SetNextDestination();
            }
        }

        // Raycast ve Þalter Algýlama Mantýðý (Aynen korundu)
        RaycastHit hit;
        float sensorMesafesi = 3f; // Ajanýn görüþ mesafesi

        // Ajanýn önünden ileriye doðru kýrmýzý bir ýþýn çizer
        Debug.DrawRay(transform.position, transform.forward * sensorMesafesi, Color.red);

        // Eðer ýþýn bir nesneye çarparsa
        if (Physics.Raycast(transform.position, transform.forward, out hit, sensorMesafesi))
        {
            // Çarpan nesnenin Tag'i "Salter" ise
            if (hit.collider.CompareTag("Salter"))
            {
                Debug.Log("Þalter algýlandý! Bariyer açýlýyor...");

                // Þalter scriptindeki fonksiyonu çaðýrýyoruz
                Salter salterScript = hit.collider.GetComponent<Salter>();
                if (salterScript != null)
                {
                    salterScript.SalteriAktiflestir();
                }
            }
        }
    }

    void SetNextDestination()
    {
        if (currentWaypointIndex < waypoints.Length && waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}