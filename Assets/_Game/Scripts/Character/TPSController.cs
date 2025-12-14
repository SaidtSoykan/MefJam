using UnityEngine;

public class TPSController : MonoBehaviour
{
    // ... Eski değişkenlerin burada duruyor ...
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Mouse/Kamera Ayarları")]
    public float mouseSensitivity = 2f;
    public Transform cameraPivot;
    public float minXLook = -60f;
    public float maxXLook = 60f;

    // YENİ: Animator Tanımlaması
    [Header("Animasyon")]
    public Animator characterAnimator;

    private Rigidbody rb;
    private float currentXRotation = 0f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() // Animasyon kontrolleri genelde Update'te yapılır
    {
        // ... Mouse kodların burada ...
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentXRotation -= mouseY;
        currentXRotation = Mathf.Clamp(currentXRotation, minXLook, maxXLook);
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

        // ... Zıplama kodun burada ...
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // YENİ: Animasyon Kontrolü
        // Eğer W,A,S,D tuşlarına basılıyorsa hareket var demektir.
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Hareket vektörünün uzunluğu 0'dan büyükse hareket ediyoruzdur.
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isWalking", isMoving);
        }
    }

    void FixedUpdate()
    {
        // ... Fizik hareket kodların burada ...
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.right * moveX) + (transform.forward * moveZ);

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    // ... Collision kodların burada ...
    void OnCollisionStay(Collision collision) { isGrounded = true; }
    void OnCollisionExit(Collision collision) { isGrounded = false; }
}