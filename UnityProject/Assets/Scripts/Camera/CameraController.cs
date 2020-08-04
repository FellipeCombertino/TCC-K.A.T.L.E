using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{


    public float Y_ANGLE_MIN_Kim = 0f;
    public float Y_ANGLE_MIN_Barto = 0f;
    private const float Y_ANGLE_MAX = 50.0f;

    public Transform cameraFocus;
    float maxDistance,minDistance;
    public float cameraCollisionOffset;
    public float distanceKim = 5;
    public float distanceBarto = 3.5f;
    public bool lockCamera;

    private float currentX;
    private float currentY;
    public float sensitivityX = 2.0f;
    public float sensitivityY = 2.0f;

    public LayerMask ignoreCamCollsion;
    static public CameraController Instance { get; set; }
    private void Start()
    {
        Instance = this;
        maxDistance = BagController.Instance.canControll?distanceBarto:distanceKim;
        ignoreCamCollsion = ~ignoreCamCollsion;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

 
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("Level1");



        }
        if (!lockCamera)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY += Input.GetAxis("Mouse Y") * sensitivityY;

            RaycastHit hit;
            if (Physics.Linecast(cameraFocus.position, transform.parent.position, out hit, ignoreCamCollsion))
            {
                Debug.DrawLine(cameraFocus.position, hit.point, Color.red);
                transform.localPosition = new Vector3(0, 0 + cameraCollisionOffset, Vector3.Distance(cameraFocus.position, transform.parent.position) - Vector3.Distance(cameraFocus.position, hit.point));
            }
            else
            {
                Debug.DrawLine(cameraFocus.position, transform.parent.position);
                transform.localPosition = new Vector3(0, 0 + cameraCollisionOffset, 0);

            }
            currentY = Mathf.Clamp(currentY, BagController.Instance.canControll?Y_ANGLE_MIN_Barto:Y_ANGLE_MIN_Kim, Y_ANGLE_MAX);

            /*


            // ignoreCamCollision = layer do player/mochila. O linecast é feito partindo do player e terminando no pai da câmera. 
            RaycastHit hit;
            if (Physics.Linecast(cameraFocus.transform.position, transform.parent.position, out hit, ignoreCamCollsion))
            {
                distance = minDistance; // se tem algo entre eles, a câmera irá dar zoom no player
            }
            else
            {
                distance = maxDistance; // se não tiver nada entre eles, a câmera voltará para a distância normal
            }

            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

            // o pai da camera acompanha a posição da camera e fica fixo sempre na distância máxima p/ que o linecast entre o player e o pai da câmera possa detectar colisões
            transform.parent.position = cameraFocus.position + rotation * new Vector3(0, 0, -maxDistance);

            transform.position = cameraFocus.position + rotation * dir;
            transform.LookAt(cameraFocus.position);


            */
        }
    }

    private void FixedUpdate()
    {
        if (!lockCamera)
        {
            Vector3 dir = new Vector3(0, 0, BagController.Instance.canControll ? -distanceBarto : -distanceKim);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.parent.position = cameraFocus.position + rotation * dir;
            transform.parent.LookAt(cameraFocus.position);
        }
    }

    private void LateUpdate()
    {
        if (!lockCamera)
        {
            Vector3 dir = new Vector3(0, 0, BagController.Instance.canControll ? -distanceBarto : -distanceKim);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.parent.position = cameraFocus.position + rotation * dir;
            transform.parent.LookAt(cameraFocus.position);
        }
    }

}