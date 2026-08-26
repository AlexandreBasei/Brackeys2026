using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : Singleton<PlayerController>
{
    public bool CanMove { get; private set; } = true;
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 5f;
    public bool dying = false;

    [Header("Look Parameters")]
    [SerializeField, Range(0f, 10f)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(0f, 10f)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(0f, 180f)] private float upperLookLimit = 80f;
    [SerializeField, Range(0f, 180f)] private float lowerLookLimit = 80f;

    [Header("Footsteps Parameters")]
    [SerializeField] private bool enableFootsteps = true;
    [SerializeField] private float baseStepSpeed = 0.5f;
    
    [Header("Screamer Parameters")]
    [SerializeField] private GameObject redScreamer;
    [SerializeField] private GameObject neutralScreamer;
    private float footstepTimer = 0f;
    private float GetCurrentOffset => baseStepSpeed;

    private Camera playerCamera;
    private GameObject gun;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0f;

    void Awake()
    {
        base.Awake();
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        gun = GetComponentInChildren<GunController>().gameObject;
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Start()
    {
        TutoManager.Instance.ShowTuto();
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if(enableFootsteps)
            {
                HandleFootsteps();
            }

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(walkSpeed * Input.GetAxisRaw("Vertical"), walkSpeed * Input.GetAxis("Horizontal"));
        
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
        moveDirection = moveDirection.normalized * Mathf.Clamp(moveDirection.magnitude, 0, walkSpeed);
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleFootsteps()
    {
        if (!characterController.isGrounded) return;

        if (currentInput == Vector2.zero)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            AudioManager.Instance.PlayStep();
            footstepTimer = GetCurrentOffset;
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y += Physics.gravity.y * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        BystanderScript enemy = other.gameObject.GetComponentInParent<BystanderScript>();
        if (enemy != null && enemy.isPossessed)
        {
            print(GameManager.Instance.agressive);
            if (GameManager.Instance.agressive)
            {
                StartCoroutine(DeathSequence(enemy));
            }
        }
    }

    private IEnumerator DeathSequence(BystanderScript enemy)
    {
        if(dying)
                yield return null;

        dying = true;
        GameManager.Instance.Cleanup();
        GameManager.Instance.dayCount = 0;
        TutoManager.Instance.currentDay = 0;

        if (!enemy.isSmart)
        {
            redScreamer.SetActive(true);
        }
        else
        {
            neutralScreamer.SetActive(true);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.MusicEndGame);

        yield return new WaitForSeconds(7f);

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicMainMenu);
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
