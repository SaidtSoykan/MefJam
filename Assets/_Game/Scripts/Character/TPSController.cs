using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Bu scripti attığın objede Rigidbody yoksa otomatik ekler
public class TPSController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f; // Karakterin dönme hızı
    public float jumpForce = 5f;

    [Header("Animasyon")]
    public Animator characterAnimator;

    private Rigidbody rb;
    private Transform mainCameraTransform; // Kameranın yönünü almak için
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Sahnedeki Main Camera'yı otomatik bul
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Sahnede Main Camera bulunamadı! Lütfen kamerayı 'MainCamera' olarak etiketleyin.");
        }
    }

    void Update()
    {
        // --- 1. ZIPLAMA ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // İleride buraya zıplama animasyonu da ekleyebilirsin:
            // characterAnimator.SetTrigger("Jump");
        }

        // --- 2. ANIMASYON KONTROLÜ ---
        // Yatay veya dikey giriş var mı?
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Hareket varsa animasyonu çalıştır
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);

        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isWalking", isMoving);
        }
    }

    void FixedUpdate()
    {
        // --- 3. HAREKET FİZİĞİ ---

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Kameranın yönüne göre hareket vektörü oluştur
        Vector3 camForward = mainCameraTransform.forward;
        Vector3 camRight = mainCameraTransform.right;

        // Y eksenini sıfırla (Karakter havaya bakarak yürümesin)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Son hareket yönü
        Vector3 moveDirection = (camForward * moveZ + camRight * moveX).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // A. DÖNME: Karakteri gittiği yöne döndür
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // B. İLERLEME: Karakteri hareket ettir
            Vector3 targetVelocity = moveDirection * moveSpeed;

            // Mevcut düşüş hızını (Gravity) koru
            targetVelocity.y = rb.linearVelocity.y;

            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // Eğer tuşa basılmıyorsa karakteri kaydırmadan durdur (Sadece X ve Z hızını sıfırla)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    // Yere basma kontrolleri
    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}