/*
  _        _            _            _         _             _ _           
 | |      (_)          | |          | |       | |           | (_)          
 | |_ _ __ ___   ____ _| | ___ _ __ | |_   ___| |_ _   _  __| |_  ___  ___ 
 | __| '__| \ \ / / _` | |/ _ \ '_ \| __| / __| __| | | |/ _` | |/ _ \/ __|
 | |_| |  | |\ V / (_| | |  __/ | | | |_  \__ \ |_| |_| | (_| | | (_) \__ \
  \__|_|  |_| \_/ \__,_|_|\___|_| |_|\__| |___/\__|\__,_|\__,_|_|\___/|___/
                                                                           
                                FPS MOVEMENT 
                                Version: 1.0
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tivalent.Common
{
    public class FirstPersonController : MonoBehaviour
    {

        [Header("Movement:")]
        [HideInInspector] public float Speed;
        public float walkSpeed;
        public float sprintSpeed;
        public float Drag;
        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        bool canJump;
        public bool jumping;

        public Camera cam;

        [Header("Ground Check:")]
        public float Height;
        public Transform orientation;
        public LayerMask Ground;
        public bool isGrounded;

        public Vector3 dir;
        public Rigidbody rb;
        float horizontal;
        float vertical;

        // CAMERA

        public GameObject head;

        float xRot;
        float yRot;

        public float sensX;
        public float sensY;

        public bool running;


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            canJump = true;
            cam = Camera.main;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }


        void Update()
        {
            SpeedLimit();
            Inputs();

            isGrounded = Physics.Raycast(transform.position, Vector3.down, Height * 0.5f + 0.2f, Ground);

            if (isGrounded)
            {
                rb.drag = Drag;
            }
            else
            {
                rb.drag = 0;
            }

            float mouseX = Input.GetAxis("Mouse X") * sensX;
            float mouseY = Input.GetAxis("Mouse Y") * sensY;

            yRot += mouseX;
            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            cam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
            orientation.rotation = Quaternion.Euler(0, yRot, 0);

            cam.transform.position = head.transform.position;

        }

        private void FixedUpdate()
        {
            Move();
        }

        void Move()
        {
            // movement direction
            dir = orientation.forward * vertical + orientation.right * horizontal;

            if (isGrounded)
            {
                jumping = false;
                rb.AddForce(dir.normalized * Speed * 10f, ForceMode.Force);
            }

            else if (!isGrounded)
            {
                rb.AddForce(dir.normalized * Speed * 10f * airMultiplier, ForceMode.Force);
            }
        }



        void SpeedLimit()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > Speed)
            {
                Vector3 limitedVel = flatVel.normalized * Speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        void Jump()
        {
            StartCoroutine(secureJump());
            jumping = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        void Inputs()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Jump") && canJump && isGrounded)
            {
                canJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }


            if (Input.GetKey(KeyCode.LeftShift))
            {
                Speed = sprintSpeed;
                running = true;
            }
            else
            {
                running = false;
                Speed = walkSpeed;
            }
        }


        private void ResetJump()
        {
            canJump = true;
        }

        IEnumerator secureJump()
        {
            yield return new WaitForSeconds(0.01f);
            jumping = true;
        }

    }
}