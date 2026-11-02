using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PatrolGuard : MonoBehaviour
{
    [Header("Devriye Noktalarý")]
    public Transform pointA;
    public Transform pointB;

    [Header("Devriye Ayarlarý")]
    public float speed = 2.5f;
    public float stunDuration = 2f;

    private NavMeshAgent guardAgent;
    private Transform currentTarget;

    void Start()
    {
        guardAgent = GetComponent<NavMeshAgent>();
        guardAgent.speed = speed;

        currentTarget = pointA;

        if (currentTarget != null)
        {
            guardAgent.SetDestination(currentTarget.position);
        }
    }

    void Update()
    {
        if (currentTarget == null || guardAgent.pathPending) return;

        if (guardAgent.remainingDistance <= 0.5f)
        {
            if (currentTarget == pointA)
            {
                currentTarget = pointB;
            }
            else
            {
                currentTarget = pointA;
            }

            guardAgent.SetDestination(currentTarget.position);
        }
    }

    // oyunca çarpýnca çalýþýr
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.name == "Agent")
        {
            // Oyuncuyu durdur
            NavMeshAgent playerNav = collision.gameObject.GetComponent<NavMeshAgent>();
            if (playerNav != null)
            {
                StartCoroutine(PlayerStunRoutine(playerNav));
            }
        }
    }

    // Oyuncuyu donduran fonksiyon
    IEnumerator PlayerStunRoutine(NavMeshAgent playerNav)
    {
        Debug.Log("Bekçiye yakalandýn! Karakterin 2 saniye kilitlendi.");

       
        playerNav.isStopped = true;

        // 2 saniye bekle
        yield return new WaitForSeconds(stunDuration);

        // 2 saniye sonra hareketi aç
        playerNav.isStopped = false;
    }
}