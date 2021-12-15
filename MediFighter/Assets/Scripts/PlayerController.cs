using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //Variables
    public CharacterController controller;
    public CameraController cc;
    public SpawnManager sm;
    public Animator animSword;

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
    private TextMeshProUGUI shopInfo;
    public SphereCollider footCollider;
    public HealthSystem hs;

    bool blocking;
    public GameObject shield;
    bool hasShield;

    private bool m_isAxisInUse = false;
    private bool kicking = false;

    public GameObject juice;

    public AudioSource audioSourc;
    public AudioClip Swing1;
    public AudioClip Swing2;

    private bool Swing1Playing;
    private bool Swing2Playing;


    void Start()
    {
        shopInfo = GameObject.Find("ShopInfo").GetComponent<TextMeshProUGUI>();
        juice = GameObject.Find("Juice");
        canKick = true;
        foot.SetActive(false);
        if (GameObject.Find("Spawns") != null)
        {
            sm = GameObject.Find("Spawns").GetComponent<SpawnManager>();
        }
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
        if (Input.GetAxis("Crouch") > 0)
        {
            desiredHeight = 1f;
        }
        else
        {
            desiredHeight = 2f;
        }
        controller.height = Mathf.Lerp(controller.height, desiredHeight, 0.1f);

        if (Input.GetAxis("Shield") > 0 && hasShield)
        {
            blocking = true;
            shield.SetActive(true);
        }
        else
        {
            blocking = false;
            shield.SetActive(false);
        }


        //Make sword swing only occur once
        if (Input.GetAxisRaw("Fire1") != 0 && !blocking)
        {
            if (m_isAxisInUse == false)
            {
                if (animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe"))
                {
                    animSword.SetTrigger("Swing2");
                }
                else
                {
                    animSword.SetTrigger("Swing");
                }
                m_isAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw("Fire1") == 0)
        {
            m_isAxisInUse = false;
        }

        if (animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe") && !audioSourc.isPlaying && Swing1Playing)
        {
            Swing1Playing = false;
            audioSourc.PlayOneShot(Swing1);
        }
        else if (!animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe"))
        {
            Swing1Playing = true;
        }
        if (animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe2") && !audioSourc.isPlaying && Swing2Playing)
        {
            Swing2Playing = false;
            audioSourc.PlayOneShot(Swing2);
        }
        else if (!animSword.GetCurrentAnimatorStateInfo(0).IsName("Swipe2"))
        {
            Swing2Playing = true;
        }

        //Kicking
        if (Input.GetAxisRaw("Fire3") > 0f && canKick && !blocking)
        {
            if (!kicking)
            {
                footCollider.enabled = true;
                canKick = false;
                foot.SetActive(true);
                StartCoroutine(FootDissapear());
                kicking = true;
            }
        }
        if (Input.GetAxisRaw("Fire3") == 0)
        {
            kicking = false;
        }

        //Bringing up info on buyable item you're looking at
        if (ray.transform != null)
        {
            if (ray.transform.gameObject.CompareTag("Item") && Vector3.Distance(ray.transform.position, gameObject.transform.position) < 4f)
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
                    shopInfo.text = "Defend yourself with a shield. Right Click to use.\n10 Beards\n\nBuy with [E]";
                }
                else if (buyableItem.name == "StartGame" && juice.activeSelf)
                {
                    var NextWaveNum = sm.waveNum + 1;
                    shopInfo.text = "Bring on the Dwarves!\n" + "Wave: " + NextWaveNum.ToString() + "\nPress [E]";
                }
                else if (buyableItem.name == "Beard")
                {
                    shopInfo.text = "Pick up [E]";
                }
                else
                {
                    shopInfo.text = "";
                    buyableItem = null;
                }
            }
            else
            {
                shopInfo.text = "";
                buyableItem = null;
            }
        }
        else
        {
            shopInfo.text = "";
            buyableItem = null;
        }

        //Buy buyable item you're looking at
        if (Input.GetAxis("Fire2") > 0f && buyableItem != null)
        {
            if (buyableItem.name == "ArmorKit" && hs.beards >= 20)
            {
                hs.beards -= 20;
                hs.maxHealth += 5;
                hs.playerHealth += 5;
                hs.disHealth.fillAmount = (float)hs.playerHealth / (float)hs.maxHealth;
            }
            else if (buyableItem.name == "SwordUpgrade" && hs.beards >= 25)
            {
                hs.beards -= 25;
                hs.AttackAmount++;
            }
            else if (buyableItem.name == "HealthPotion" && hs.beards >= 10)
            {
                hs.beards -= 10;
                hs.playerHealth = hs.maxHealth;
                hs.disHealth.fillAmount = (float)hs.playerHealth / (float)hs.maxHealth;
            }
            else if (buyableItem.name == "BuyShield" && hs.beards >= 10)
            {
                hs.beards -= 10;
                hs.playerHealth = hs.maxHealth;
                buyableItem.SetActive(false);
                hasShield = true;
            }
            else if (buyableItem.name == "StartGame" && juice.activeSelf)
            {
                sm.startWave = true;
            }
            else if (buyableItem.name == "Beard")
            {
                hs.beards++;
                Destroy(buyableItem);
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
}
