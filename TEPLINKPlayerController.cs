using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TEPLINKPlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public TEPLINKGameManager manager;
    public float moveSpeed = 4.5f;
    public float mouseSensitivity = 2f;
    public float gravity = -20f;
    private CharacterController controller;
    private float pitch;
    private float verticalVelocity;
    private bool lookEnabled = true;
    private bool mobileForward, mobileBack, mobileLeft, mobileRight;

    private void Awake() { controller = GetComponent<CharacterController>(); }

    private void Update()
    {
        if (TEPLINKUI.IsMenuOpen) return;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        if (mobileLeft) x = -1;
        if (mobileRight) x = 1;
        if (mobileForward) z = 1;
        if (mobileBack) z = -1;

        Vector3 move = (transform.right * x + transform.forward * z);
        if (move.magnitude > 1) move.Normalize();

        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (lookEnabled && Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            transform.Rotate(Vector3.up * mouseX);
            pitch = Mathf.Clamp(pitch - mouseY, -80, 80);
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            TEPLINKUI.IsMenuOpen = true;
            manager.ui?.ShowPauseHint();
        }
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            TEPLINKUI.IsMenuOpen = false;
            manager.ui?.HidePauseHint();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null) return;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 3.2f))
        {
            var computer = hit.collider.GetComponentInParent<TEPLINKComputer>();
            if (computer != null) { manager.ui?.OpenCyberPanel(computer); return; }
            var food = hit.collider.GetComponentInParent<TEPLINKFoodSpot>();
            if (food != null) { manager.BuyFood(); return; }
            var drink = hit.collider.GetComponentInParent<TEPLINKDrinkSpot>();
            if (drink != null) { manager.BuyDrink(); return; }
            var bed = hit.collider.GetComponentInParent<TEPLINKBed>();
            if (bed != null) { manager.energy = Mathf.Min(100, manager.energy + 30); manager.ui?.ShowNotice("ارتحت شوية. الطاقة زادت."); manager.ui?.Refresh(); }
        }
    }

    public void SetMobileMove(string direction, bool pressed)
    {
        switch (direction)
        {
            case "forward": mobileForward = pressed; break;
            case "back": mobileBack = pressed; break;
            case "left": mobileLeft = pressed; break;
            case "right": mobileRight = pressed; break;
        }
    }

    public void MobileInteract() => TryInteract();
}
