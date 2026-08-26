using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerController : Singleton<PlayerController>
{
    public bool CanMove = true;
    public bool CanLook = false;
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
    [SerializeField] private GameObject frenzyScreamer;
    [SerializeField] private GameObject pacificEnd;
    [SerializeField] private GameObject killerEnd;
    [SerializeField] private TextMeshProUGUI gameOverText;
    public TextMeshProUGUI timerText;
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

            if (CanLook)
                HandleMouseLook();

            if (enableFootsteps)
            {
                HandleFootsteps();
            }

            ApplyFinalMovements();
        }

        bool shouldShowTimer = !GameManager.Instance.isPaused && GameManager.Instance.dayCount != 4;

        timerText.enabled = shouldShowTimer;

        if (shouldShowTimer)
        {
            TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.timeLeft);
            timerText.text = time.ToString(@"mm\:ss\:ff");
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
            if (GameManager.Instance.agressive)
            {
                StartCoroutine(DeathSequence(enemy));
            }
        }
    }

    private IEnumerator DeathSequence(BystanderScript enemy)
    {
        if (dying)
            yield return null;

        dying = true;
        timerText.enabled = false;

        if (!enemy.isSmart && !GameManager.Instance.triggeredFrenzy)
        {
            redScreamer.SetActive(true);
        }
        else if (!enemy.isSmart && GameManager.Instance.triggeredFrenzy)
        {
            frenzyScreamer.SetActive(true);
            showFrenzyText();
        }
        else
        {
            neutralScreamer.SetActive(true);
        }

        GameManager.Instance.ResetGame();
        TutoManager.Instance.currentDay = 0;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.MusicScreamer);

        yield return new WaitForSeconds(5f);

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicMainMenu);
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void showFrenzyText()
    {
        gameOverText.text = "No one trusts you anymore";
        fadeInGameOverText();
    }

    public void showPacificEnd()
    {
        pacificEnd.SetActive(true);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicPacificEnd);
        gameOverText.text = "You trusted everyone";
        fadeInGameOverText();
    }

    public void showKillerEnd()
    {
        killerEnd.SetActive(true);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicKillerEnd);
        gameOverText.text = "You trusted no one";
        fadeInGameOverText();
    }

    private void fadeInGameOverText()
    {
        LeanTween.value(
            gameOverText.gameObject,
            gameOverText.color.a,
            1f,
            1f
        )
        .setEase(LeanTweenType.easeInOutQuad)
        .setOnUpdate((float alpha) =>
        {
            Color color = gameOverText.color;
            color.a = alpha;
            gameOverText.color = color;
        });
    }
}
