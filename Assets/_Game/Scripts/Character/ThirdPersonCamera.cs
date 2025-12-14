using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Hedef Ayarlarý")]
    public Transform target; // Karakterin Kafa/Omuz hizasýndaki Pivot noktasý
    public Vector3 offset = new Vector3(0, 0, -3); // Kameranýn karakterden ideal uzaklýðý

    [Header("Hýz ve Hassasiyet")]
    public float mouseSensitivity = 2f;
    public float rotationSmoothTime = 0.12f;
    public Vector2 pitchMinMax = new Vector2(-40, 85); // Aþaðý/Yukarý bakma limiti

    [Header("Duvar Çarpýþmasý (Collision)")]
    public LayerMask collisionLayers; // Hangi objeler kamerayý engeller? (Duvarlar, Zemin)
    public float wallOffset = 0.2f; // Duvara ne kadar yaklaþsýn (Ýçine girmemesi için pay)
    public float collisionSmoothTime = 0.1f; // Duvarý görünce ne kadar yumuþak yaklaþsýn

    // Private Deðiþkenler
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;
    private float yaw; // Sað-Sol açýsý
    private float pitch; // Yukarý-Aþaðý açýsý
    private float finalDistance;
    private float currentDistanceVelocity;

    void Start()
    {
        // Baþlangýçta hedef ile aradaki mesafeyi hesapla
        finalDistance = offset.magnitude;

        // Mouse imlecini gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Mouse Girdilerini Al
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        // 2. Yumuþak Dönüþ Hesapla
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        transform.eulerAngles = currentRotation;

        // 3. Ýdeal Pozisyonu Hesapla (Duvar olmasaydý nerede olacaktý?)
        // Kameranýn baktýðý yönün tersine (arkaya) doðru offset kadar git
        Vector3 desiredPosition = target.position - transform.forward * offset.magnitude;

        // 4. Çarpýþma Kontrolü (Duvar var mý?)
        Vector3 adjustedPosition = desiredPosition;
        float targetDist = offset.magnitude;

        // Hedef'ten (Karakter) Kameraya doðru bir ýþýn (Ray) yolla
        RaycastHit hit;
        // Karakterden kameraya doðru, aradaki mesafe kadar tara
        if (Physics.Raycast(target.position, -transform.forward, out hit, offset.magnitude, collisionLayers))
        {
            // Eðer bir þeye çarparsa, mesafeyi çarpýlan yerin biraz önüne çek
            targetDist = hit.distance - wallOffset;
        }

        // 5. Kamerayý Pozisyonla
        // Aniden zoom yapmamasý için SmoothDamp kullanýyoruz
        finalDistance = Mathf.SmoothDamp(finalDistance, targetDist, ref currentDistanceVelocity, collisionSmoothTime);

        // Kamerayý, baktýðý yönün tersinde 'finalDistance' kadar uzaða koy
        transform.position = target.position - transform.forward * finalDistance;
    }
}