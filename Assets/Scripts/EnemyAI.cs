using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Player'ý güvenli bir þekilde bulalým
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    void Update()
    {
        // --- DÜZELTME BURADA ---

        // 1. Hedefim var mý? (Player ölmüþ olabilir)
        // 2. Agent bileþeni hala aktif mi? (Ölünce kapanýyor çünkü)
        // 3. NavMesh üzerinde miyim? (Bazen havada doðarsa hata verir)

        if (target != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }

        // Eðer agent kapalýysa (ölüysem) hiçbir þey yapma, kod burada dursun.
    }
}