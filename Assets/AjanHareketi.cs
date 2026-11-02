using UnityEngine;
using UnityEngine.AI;

public class AjanHareketi : MonoBehaviour
{
    public Transform[] waypoints; //gidilicek kutular
    private int currentWaypointIndex = 0;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Ýlk hedef
        SetNextDestination();
    }

    void Update()
    {
        // diðer kutulara devam et
        if (waypoints.Length > 0 && currentWaypointIndex < waypoints.Length)
        {
            if (waypoints[currentWaypointIndex] != null && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentWaypointIndex++;
                SetNextDestination();
            }
        }

        // þalter
        RaycastHit hit;
        float sensorMesafesi = 3f;

        // kýrmýzý çizgi
        Debug.DrawRay(transform.position, transform.forward * sensorMesafesi, Color.red);

        
        if (Physics.Raycast(transform.position, transform.forward, out hit, sensorMesafesi))
        {
            // çarpan nesnenin Tagi salter
            if (hit.collider.CompareTag("Salter"))
            {
                Debug.Log("Þalter algýlandý! Bariyer açýlýyor...");

                //þalter fonksiyonu çaðýrma
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