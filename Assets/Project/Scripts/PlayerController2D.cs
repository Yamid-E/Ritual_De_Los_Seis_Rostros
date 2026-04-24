using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;

    [Header("Configuración de Cámara")]
    public Transform cameraPivot;
    public float mouseSensitivity = 3f;
    public float rotationSmooth = 10f;
    public float minPitch = -15f;
    public float maxPitch = 35f;

    private Rigidbody rb;
    private Animator anim;
    private Vector2 input;

    private float yaw;
    private float pitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // Inicializamos la rotación
        yaw = 0f;
        pitch = 0f;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Bloqueamos y ocultamos el cursor para controlar la cámara con el mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Capturar los inputs
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // 2. Manejar la rotación y las animaciones frame a frame
        HandleCameraRotation();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        // 3. Aplicar el movimiento físico en el FixedUpdate
        HandleMovement();
    }

    private void HandleCameraRotation()
    {
        if (cameraPivot == null) return;

        // Rotación horizontal (afecta al jugador completo)
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Rotación vertical (afecta solo al pivote de la cámara)
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion targetPitch = Quaternion.Euler(pitch, 0f, 0f);
        cameraPivot.localRotation = Quaternion.Lerp(
            cameraPivot.localRotation, 
            targetPitch, 
            rotationSmooth * Time.deltaTime
        );
    }

    private void HandleMovement()
    {
        // Calculamos la dirección basándonos en hacia dónde está mirando el jugador
        Quaternion moveRotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 forward = moveRotation * Vector3.forward;
        Vector3 right = moveRotation * Vector3.right;

        Vector3 moveDir = (forward * input.y) + (right * input.x);

        // .normalized evita que el restaurador corra más rápido al moverse en diagonal
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }

    private void HandleAnimations()
    {
        if (anim == null) return;

        if (input != Vector2.zero)
        {
            // Actualizamos los parámetros de dirección en el Animator
            anim.SetFloat("DirX", input.x);
            anim.SetFloat("DirY", input.y);
            anim.SetBool("Caminando", true);

            // LA MEMORIA: Guardamos la última dirección antes de soltar la tecla
            anim.SetFloat("LastDirX", input.x);
            anim.SetFloat("LastDirY", input.y);
        }
        else
        {
            anim.SetBool("Caminando", false);
        }
    }
}