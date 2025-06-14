using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour{

    private GameLoop.Ware wareInHand;


    private LayerMask layerMask;
    private RaycastHit raycastHit;
    private Transform lastHitTransform;
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Vector2 playerMoveInput;
    private Vector2 playerLookInput;
    private float verticalRotation = 0f;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 15f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private float acceleration = 5f; //smooth movement start
    private Vector3 currentVelocity;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Transform handEmpty;
    [SerializeField] private Transform handWmelon;
    [SerializeField] private Transform handMelon;
    [SerializeField] private Transform handBeer;
    [SerializeField] private Transform handStrawberry;
    [SerializeField] private Transform handMoney;


    public void Awake() {
        layerMask = LayerMask.GetMask("Clickable");
        characterController = GetComponent<CharacterController>();
        playerInput = new PlayerInput();

    }

    private void OnEnable() {
        playerInput.Default.Enable();
        playerInput.Default.Movement.performed += OnMove;
        playerInput.Default.Movement.canceled += OnMove;
        playerInput.Default.Look.performed += OnLook;
        playerInput.Default.InteractMain.performed += OnInteractMain;
        playerInput.Default.InteractSecondary.performed += OnInteractSecondary;

    }

    private void OnDisable() {
        playerInput.Default.Movement.performed -= OnMove;
        playerInput.Default.Movement.canceled -= OnMove;
        playerInput.Default.Look.performed -= OnLook;
        playerInput.Default.InteractMain.performed -= OnInteractMain;
        playerInput.Default.InteractSecondary.performed -= OnInteractSecondary;
        playerInput.Default.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx) {
        playerMoveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx) {
        playerLookInput = ctx.ReadValue<Vector2>();
    }

    public void Update() {
        HandleMovement();
        HandleCameraRotation();
        LookingScan();
    }

    public GameLoop.Ware WareInHand(){ return wareInHand; }

    public void UpdateHandheld(GameLoop.Ware newWare)
    {
        wareInHand = newWare;
        handEmpty.gameObject.SetActive(false);
        handWmelon.gameObject.SetActive(false);
        handMelon.gameObject.SetActive(false);
        handBeer.gameObject.SetActive(false);
        handStrawberry.gameObject.SetActive(false);
        handMoney.gameObject.SetActive(false);



        switch (wareInHand) {
            case GameLoop.Ware.Empty:
                handEmpty.gameObject.SetActive(true);
                break;
            case GameLoop.Ware.Watermelon:
                handWmelon.gameObject.SetActive(true);
                break;
            case GameLoop.Ware.Melon:
                handMelon.gameObject.SetActive(true);
                break;
            case GameLoop.Ware.Beer:
                handBeer.gameObject.SetActive(true);
                break;
            case GameLoop.Ware.Strawberry:
                handStrawberry.gameObject.SetActive(true);
                break;
            case GameLoop.Ware.BuyerMoney:
                handMoney.gameObject.SetActive(true);
                break;
            case GameLoop.Ware.Change:
                handMoney.gameObject.SetActive(true);
                break;
        }
    }

    private void HandleMovement()
    {
        Vector3 targetDirection = (transform.forward * playerMoveInput.y + transform.right * playerMoveInput.x).normalized;
        Vector3 targetSpeed = targetDirection * moveSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetSpeed, acceleration * Time.deltaTime);
        characterController.Move(currentVelocity * Time.deltaTime);
    }

    private void HandleCameraRotation() {
        transform.Rotate(Vector3.up, playerLookInput.x * mouseSensitivity * Time.deltaTime);

        verticalRotation -= playerLookInput.y * mouseSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void LookingScan()
    {
        lastHitTransform?.GetComponent<ClickableObject>()?.OnPlayerStopLooking(this);
        if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out raycastHit, Mathf.Infinity, layerMask))
        {
            raycastHit.transform.GetComponent<ClickableObject>().OnPlayerLook(this);
            if (lastHitTransform != raycastHit.transform)
            {
                lastHitTransform?.GetComponent<ClickableObject>()?.OnPlayerStopLooking(this);
                lastHitTransform = raycastHit.transform;
            }
        }

        /*if (lastHitTransform != null)
        {
            lastHitTransform.GetComponent<ClickableObject>().OnPlayerStopLooking(this);
            lastHitTransform = null;
        } */
    }

    private void OnInteractMain(InputAction.CallbackContext ctx)
    {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out raycastHit, Mathf.Infinity, layerMask))
        {
            raycastHit.transform.GetComponent<ClickableObject>().OnPlayerInteractMain(this);
            Debug.DrawLine(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward) * 10000f, Color.black, 1000f);
        }

    }

    private void OnInteractSecondary(InputAction.CallbackContext ctx) {
        if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out raycastHit, Mathf.Infinity, layerMask)) {
            raycastHit.transform.GetComponent<ClickableObject>().OnPlayerInteractSecondary(this);
        }
    }

}
