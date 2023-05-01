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
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    [SerializeField] private bool invertCamera;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float gamepadSensitivity;
    private float lookSensitivity;
    private float camCurXRot;
    private Quaternion playerCameraOriginalRotation;
    private float shakeTime;
    private float shakeDuration;
    [SerializeField] private Vector2 mouseDelta;
    #endregion
    #region Properties
    [Header("Properties")]
    [SerializeField] private Animator anim;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform shotPoint;
    private Vector3 velocity;
    private PlayerInput playerInput;
    private Rigidbody rb;
    private CapsuleCollider capsule;

    #endregion
    #region Weapon  
    [Header("Weapon")]
    [SerializeField] float range = 100f;
    [SerializeField] float weaponDamage = 5f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitParticles;
    [SerializeField] AudioClip gunShot;
    
    AudioSource source;

    #endregion
    #region PlayerInventory
    [Header("Inventory")]
    [SerializeField] float health = 100;

    public float Health { get => health; set => health = value; }
    #endregion

    #region UI
    [SerializeField] private CanvasGroup hurtPanel;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
        Cursor.lockState = CursorLockMode.Locked;
        source = GetComponent<AudioSource>();
    }
    private void Update()
    {
        Move();
        if(hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }
        if (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if (playerCamera.transform.localRotation != playerCameraOriginalRotation)
        {
            playerCamera.transform.localRotation = playerCameraOriginalRotation;
        }
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    public void Move()
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

    void Shoot()
    {
        muzzleFlash.Play();
        source.PlayOneShot(gunShot);
        anim.SetBool("isShooting", true);
        RaycastHit hit;

        if (Physics.Raycast(shotPoint.position, transform.forward, out hit, range))
        {
            EnemyManager enemy = hit.transform.GetComponent<EnemyManager>();
            if (enemy != null)
            {
                enemy.TakeDamage(weaponDamage);
                GameObject instParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                instParticles.transform.parent = hit.transform;

                Destroy(instParticles, 2f);
            }
        }
    }

    public void PlayerHit(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameManager.EndGame();
        }
        else
        {
            shakeTime = 0;
            shakeDuration = 0.2f;
            hurtPanel.alpha = 1;
        }
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }

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
        if (anim.GetBool("isShooting"))
        {
            anim.SetBool("isShooting", false);
        }
        if (context.phase == InputActionPhase.Performed)
        {
            Shoot();
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {

        }
    }

    #endregion

}
