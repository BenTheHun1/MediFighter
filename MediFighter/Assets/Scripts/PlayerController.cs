using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Variables
    public CharacterController controller;
    public CameraController cc;
    public Animator animSword;
    public CapsuleCollider hitBox;

    public Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float speed = 4f;
    private float gravity = -9.81f * 6;
    private float jumpHeight = 1f;

    public Vector3 velocity;
    public bool isOnGround;

    public RaycastHit ray;

    float desiredHeight;
    bool canKick;
    public GameObject foot;

    public GameObject buyableItem;
    private Text shopInfo;
    public HealthSystem hs;

    bool blocking;
    public GameObject shield;
    bool hasShield;

    void Start()
    {
        shopInfo = GameObject.Find("ShopInfo").GetComponent<Text>();
        canKick = true;
        foot.SetActive(false);
    }

    void Update()
    {
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //Checks if you are on a Ground layer object

        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -2f; //Stops y velocity from infinitely decreasing
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Quick Reload of Scene
        }

        //Crouching System
        if (Input.GetKey(KeyCode.LeftControl))
        {
            desiredHeight = 1f;
        }
        else
        {
            desiredHeight = 2f;
        }
        controller.height = Mathf.Lerp(controller.height, desiredHeight, 0.1f);

        if (Input.GetKey(KeyCode.Mouse1) && hasShield)
        {
            blocking = true;
            shield.SetActive(true);
        }
        else
        {
            blocking = false;
            shield.SetActive(false);
        }

        //Sword Animatons
        if (Input.GetKeyDown(KeyCode.Mouse0) && !blocking)
        {
            hitBox.enabled = true;
            if (animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe"))
            {
                animSword.SetTrigger("Swing2");
            }
            else
            {
                animSword.SetTrigger("Swing");
            }
            StartCoroutine(Slaying());
        }

        //Kicking
        if (Input.GetKeyDown(KeyCode.F) && canKick && !blocking)
        {
            canKick = false;
            foot.SetActive(true);
            StartCoroutine(FootDissapear());
        }

        //Bringing up info on buyable item you're looking at
        if (ray.transform != null)
        {
            if (ray.transform.gameObject.CompareTag("Item"))
            {
                buyableItem = ray.transform.gameObject;
                if (buyableItem.name == "ArmorKit")
                {
                    shopInfo.text = "Upgrade your Armor, increasing your max health.\n20 Beards\n\nBuy with [E]"; // \n = New line
                }
                else if (buyableItem.name == "SwordUpgrade")
                {
                    shopInfo.text = "Upgrade your Sword, increasing your attack power.\n25 Beards\n\nBuy with [E]";
                }
                else if (buyableItem.name == "HealthPotion")
                {
                    shopInfo.text = "Heal yourself back to full health.\n10 Beards\n\nBuy with [E]";
                }
                else if (buyableItem.name == "BuyShield")
                {
                    shopInfo.text = "Defend yourself with a shield.\n10 Beards\n\nBuy with [E]";
                }
            }
            else
            {
                buyableItem = null;
                shopInfo.text = "";
            }
        }

        //Buy buyable item you're looking at
        if (Input.GetKeyDown(KeyCode.E) && buyableItem != null)
        {
            if (buyableItem.name == "ArmorKit" && hs.beards >= 20)
            {
                hs.beards -= 20;
                hs.maxHealth += 5;
                hs.playerHealth += 5;
            }
            else if (buyableItem.name == "SwordUpgrade" && hs.beards >= 25)
            {
                hs.beards -= 25;
                //atk++;
            }
            else if (buyableItem.name == "HealthPotion" && hs.beards >= 10)
            {
                hs.beards -= 10;
                hs.playerHealth = hs.maxHealth;
            }
            else if (buyableItem.name == "BuyShield" && hs.beards >= 10)
            {
                hs.beards -= 10;
                hs.playerHealth = hs.maxHealth;
                buyableItem.SetActive(false);
                hasShield = true;
            }
        }
    }

    //Hide foot after done playing anim
    IEnumerator FootDissapear()
    {
        yield return new WaitForSeconds(0.5f); //Change time based on anim speed 1.5 speed = 0.5 seconds
        cc.inControl = true;
        foot.SetActive(false);
        canKick = true;
    }

    IEnumerator Slaying()
    {
        yield return new WaitForSeconds(1f); //Change time based on anim speed 1.5 speed = 0.5 seconds
        hitBox.enabled = false;
    }

}
