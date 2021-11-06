using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public CameraController cc;

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

    // Start is called before the first frame update
    void Start()
    {
        desiredHeight = 2;
    }

    // Update is called once per frame
    void Update()
    {
        isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isOnGround && velocity.y < 0)
        {
            velocity.y = -2f;
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
            SceneManager.LoadScene("Main");
        }

        controller.height = Mathf.Lerp(controller.height, desiredHeight, 0.1f);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            desiredHeight = 1f;
        }
        else
        {
            desiredHeight = 2f;
        }


    }
}
