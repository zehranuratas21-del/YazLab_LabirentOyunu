using UnityEngine;
using UnityEngine.AI;

public class AjanHareketi : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


         void Update()
         {
                if (target != null && agent != null)
                {
                    agent.SetDestination(target.position);
                }

                RaycastHit hit;
                float sensorMesafesi = 3f; // Ajanýn görüþ mesafesi

                // Ajanýn önünden ileriye doðru kýrmýzý bir ýþýn çizer
                Debug.DrawRay(transform.position, transform.forward * sensorMesafesi, Color.red);

                // Eðer ýþýn bir nesneye çarparsa
                if (Physics.Raycast(transform.position, transform.forward, out hit, sensorMesafesi))
                {
                    // Çarpýlan nesnenin Tag'i "Salter" ise
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
                // ---------------------------------------------
         }
        
    
}