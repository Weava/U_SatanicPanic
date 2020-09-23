using UnityEngine;

public class Mouse_Look : MonoBehaviour
{
    public float mouseSens;
    public bool invertMouse;

    private Transform player;

    [SerializeField]
    private GameObject viewModel;

    private float mouseXDelta = 0.0f;
    private float mouseYDelta = 0.0f;

    private float mouseLookClamp = 90.0f;

    // Use this for initialization
    private void Start()
    {
        player = this.transform.parent;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (invertMouse)
            mouseXDelta -= Input.GetAxis("Mouse Y") * mouseSens;
        else
            mouseXDelta += Input.GetAxis("Mouse Y") * mouseSens;

        mouseYDelta = Input.GetAxis("Mouse X") * mouseSens;

        player.Rotate(0, mouseYDelta, 0);

        mouseXDelta = Mathf.Clamp(mouseXDelta, -mouseLookClamp, mouseLookClamp);

        this.transform.localEulerAngles = new Vector3(mouseXDelta, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}