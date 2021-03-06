using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandlerBOOM : MonoBehaviour
{
    //public EnemyAICharacterJoints enemyAI;
    public BoomEnemyAI enemyAI;
    private PlayerController playerController;
    private bool isKICKED;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = transform.root.GetComponent<BoomEnemyAI>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword") && !enemyAI.invincible)
        {
            if (playerController.animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe") || playerController.animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe"))
            {
                enemyAI.Slashed();
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Boot") && !enemyAI.isRagdoll && !isKICKED)
            {
                enemyAI.isKicked = true;
                enemyAI.Ragdoll();
            }
        }

        if (other.gameObject.CompareTag("Boot") && enemyAI.isRagdoll && enemyAI.Health <= 0)
        {
            enemyAI.skipDeathStruggle = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        if (other.transform.root != transform.root && other.gameObject.CompareTag("EnemyRoot") && enemyAI.isRagdoll && enemyAI.isKicked == true)
        {
            if (other.transform.root.TryGetComponent(out EnemyAICharacterJoints AIenemy))
            {
                AIenemy.isKicked = true;
                AIenemy.Ragdoll();
            }
        }
    }
}
