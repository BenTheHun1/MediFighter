using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAICharacterJoints : MonoBehaviour
{
	public Vector3 lastPos;
	public GameObject rootJoint;
	public Rigidbody rootRigid;
	public CapsuleCollider rootCapCollide;
	public BoxCollider rootBoxCollide;
	public Rigidbody[] rigids;
	public CapsuleCollider[] capColliders;
	public BoxCollider[] boxColliders;
	public Animator animEnemy;
	public Renderer rend;
	public int Health;
	public float movementSpeed;
	public bool isDamaged;
	public bool isRagdoll;
	public bool isKicked;
	public bool isAttacking;
	public bool isWalking;
	public bool GetUp;
	private bool invincible;
	private Quaternion qTo;
	private GameObject player;
	private GameObject spawnManager;
	private float lookSpeed = 2.0f;
	private float stoppingradius = 1.7f;
	private Color32 color;

	void Start()
	{
		Health = 3;
		invincible = false;
		rootRigid = GetComponent<Rigidbody>();
		rootCapCollide = GetComponent<CapsuleCollider>();
		rootBoxCollide = GetComponent<BoxCollider>();
		rigids = GetComponentsInChildren<Rigidbody>();
		capColliders = GetComponentsInChildren<CapsuleCollider>();
		boxColliders = GetComponentsInChildren<BoxCollider>();
		animEnemy = transform.root.GetComponent<Animator>();
		player = GameObject.Find("Player");
		spawnManager = GameObject.Find("Spawns");
	}

	void Update()
	{
		Vector3 lookDirection = (player.transform.position - transform.position).normalized;
		lookDirection.y = 0;
		qTo = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 90, 0);
		rootJoint.transform.SetParent(transform, true);
		if (Vector3.Distance(player.transform.position, transform.position) > stoppingradius && !isDamaged && !isRagdoll)
		{
			isWalking = true;
			isAttacking = false;
			animEnemy.SetTrigger("Walking");
			transform.rotation = Quaternion.Slerp(transform.rotation, qTo, Time.deltaTime * lookSpeed);
			if (animEnemy.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
			{
				transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movementSpeed * Time.deltaTime);
			}
		}
		else
		{
			if (!isDamaged && !isRagdoll)
			{
				isAttacking = true;
				isWalking = false;
				animEnemy.SetTrigger("Attacking");
			}
		}

		if (gameObject.transform.position.y < -25)
		{
			spawnManager.GetComponent<SpawnManager>().enemyAmount.Remove(gameObject);
			Destroy(gameObject);
			Debug.Log("AAAAAAAAAAAAAAAAAAAA");
		}

		if (GetUp)
        {
			Quaternion q = Quaternion.FromToRotation(rootJoint.transform.up, Vector3.up) * rootJoint.transform.rotation;
			rootJoint.transform.rotation = Quaternion.Slerp(rootJoint.transform.rotation, q, Time.deltaTime * lookSpeed);
			rootJoint.transform.localPosition = Vector3.Slerp(rootJoint.transform.localPosition, new Vector3(rootJoint.transform.localPosition.x, rootJoint.transform.localPosition.y + 0.3f, rootJoint.transform.localPosition.z), Time.deltaTime * 2f);
		}
	}

	void OnCollisionEnter(Collision collision)
	{

		if (collision.gameObject.CompareTag("Sword") && !isDamaged && !isRagdoll && !invincible)
		{
			isDamaged = true;
			Ragdoll();
		}

		if (collision.gameObject.CompareTag("Boot") && !isDamaged && !isRagdoll && !invincible)
		{
			isKicked = true;
			isDamaged = true;
			Ragdoll();
		}

	}

	/*void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<EnemyAICharacterJoints>().isKicked == true && other.gameObject.GetComponent<EnemyAICharacterJoints>().isRagdoll == true)
		{
			isDamaged = true;
			isKicked = true;
			Ragdoll();
		}
	}*/
	// rip domino effect

	void ResetColliders()
    {
		rootRigid = gameObject.AddComponent<Rigidbody>();
		rootRigid.constraints = RigidbodyConstraints.FreezeRotation;
		rootCapCollide = gameObject.AddComponent<CapsuleCollider>();
		rootCapCollide.center = new Vector3(0, 3, 0);
		rootCapCollide.direction = 1;
		rootCapCollide.radius = 0.4f;
		rootCapCollide.height = 6.6f;
		rootBoxCollide = gameObject.AddComponent<BoxCollider>();
		rootBoxCollide.isTrigger = true;
		rootBoxCollide.center = new Vector3(0, 3, 0);
		rootBoxCollide.size = new Vector3(4, 7, 5);
	}
	void WakeUp()
    {
		isRagdoll = false;
		foreach (Rigidbody rb in rigids)
		{
			if (rb != null)
			{
				rb.gameObject.transform.rotation = Quaternion.identity;
				rb.isKinematic = true;
			}
		}
		foreach (CapsuleCollider cc in capColliders)
		{
			if (cc != null)
			{
				cc.enabled = false;
			}
		}
		foreach (BoxCollider bc in boxColliders)
		{
			if (bc != null)
			{
				bc.enabled = false;
			}
		}
		animEnemy.enabled = true;
		lastPos = rootJoint.transform.position;
		transform.position = lastPos;
		rootJoint.transform.position = lastPos;
	}

	void Ragdoll()
	{
		Health -= 1;
		animEnemy.ResetTrigger("Walking");
		animEnemy.ResetTrigger("Attacking");
		isRagdoll = true;
		isAttacking = false;
		isWalking = false;
		animEnemy.enabled = false;
		rootRigid.constraints = RigidbodyConstraints.None;
		Destroy(rootRigid);
		Destroy(rootCapCollide);
		Destroy(rootBoxCollide);
		foreach (Rigidbody rb in rigids)
		{
			if (rb != null)
			{
				rb.isKinematic = false;
			}
		}
		foreach (CapsuleCollider cc in capColliders)
		{
			if (cc != null)
			{
				cc.enabled = true;
			}
		}
		foreach (BoxCollider bc in boxColliders)
		{
			if (bc != null)
			{
				bc.enabled = true;
			}
		}
		if (Health > 0)
		{
			color = new Color32(108, 108, 108, 0);
			rend.material.color = color;
		}
		else
        {
			if (Health <= 0)
			{
				color = new Color32(108, 0, 0, 0);
				rend.material.color = color;
			}
		}
		StartCoroutine(Damage());
	}
	IEnumerator Damage()
	{
		if (isKicked)
		{
			yield return new WaitForSeconds(0.5f);
			isKicked = false;
		}
		yield return new WaitForSeconds(3f);
		if (Health > 0)
		{
			GetUp = true;
			rootJoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
			color = new Color32(255, 255, 255, 0);
			rend.material.color = color;
			StartCoroutine(WakingUp());
		}
		else
        {
			if (Health <= 0)
            {
				StartCoroutine(FinalDeath());
            }
        }
	}

	IEnumerator WakingUp()
    {
		yield return new WaitForSeconds(3f);
		GetUp = false;
		rootJoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		WakeUp();
		ResetColliders();
		isDamaged = false;
		StartCoroutine(InvincibilityFrame());
	}
	IEnumerator FinalDeath()
	{
		yield return new WaitForSeconds(3f);
		rootJoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		GetUp = true;
		yield return new WaitForSeconds(2f);
		GetUp = false;
		rootJoint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		yield return new WaitForSeconds(2f);
		spawnManager.GetComponent<SpawnManager>().enemyAmount.Remove(gameObject);
		Destroy(gameObject);
	}

	IEnumerator InvincibilityFrame()
	{
		color = new Color32(255, 255, 255, 0);
		rend.material.color = color;
		invincible = true;
		yield return new WaitForSeconds(2f);
		color = new Color32(255, 255, 255, 0);
		rend.material.color = color;
		invincible = false;
	}
}
