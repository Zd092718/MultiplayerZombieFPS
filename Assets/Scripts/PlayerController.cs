using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Control
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private Vector2 moveComposite;

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private bool invertCamera;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float gamepadSensitivity;
    private float lookSensitivity;
    private float camCurXRot;
    [SerializeField] private Vector2 mouseDelta;
    #endregion
    #region Properties
    [SerializeField] private Animator anim;
    [SerializeField] private Transform shotPoint;
    private Vector3 velocity;
    private PlayerInput playerInput;
    private Rigidbody rb;
    private CapsuleCollider capsule;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();

    }

    private void LateUpdate()
    {
        CameraLook();
    }

    private void Move()
    {
        Vector3 dir = transform.forward * moveComposite.y + transform.right * moveComposite.x;

        dir *= moveSpeed;

        dir.y = rb.velocity.y;

        rb.velocity = dir;
    }
    private bool IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        return isGrounded;
    }
    private void CameraLook()
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            lookSensitivity = gamepadSensitivity;
        }
        else
        {
            lookSensitivity = mouseSensitivity;
        }

        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);

        //Invert camera if option is selected
        if (invertCamera)
        {
            cameraContainer.localEulerAngles = new Vector3(camCurXRot, 0, 0);

        }
        else
        {
            cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        }
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);

        Debug.Log(playerInput.currentControlScheme);
    }




    // private void ProcessZombieHit()
    // {
    //     RaycastHit hit;
    //     if (Physics.Raycast(shotPoint.position, shotPoint.forward, out hit, 50))
    //     {
    //         GameObject hitZombie = hit.collider.gameObject;
    //         if (hitZombie.CompareTag("Zombie"))
    //         {
    //             ZombieController zc = hitZombie.GetComponent<ZombieController>();
    //             if (Random.Range(0, 10) < 5)
    //             {
    //                 GameObject ragdollPrefab = zc.Ragdoll;
    //                 GameObject newRagdoll = Instantiate(ragdollPrefab, hitZombie.transform.position, hitZombie.transform.rotation);
    //                 newRagdoll.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shotPoint.forward * 10000);
    //                 Destroy(hitZombie);
    //             }
    //             else
    //             {
    //                 zc.KillZombie();
    //             }
    //         }
    //     }
    // }

    #region Control Input Functions
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            moveComposite = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            moveComposite = Vector2.zero;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (IsGrounded())
            {

                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !anim.GetBool("fire"))
        {
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {

        }
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            anim.SetTrigger("melee");
        }
    }
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo"))
        {
            Destroy(other.gameObject);
            Debug.Log("Picked up ammo");
        }
        if (other.CompareTag("Medkit"))
        {
            Destroy(other.gameObject);
            Debug.Log("Picked up medkit");
        }
    }
}
