using UnityEngine;

public class Billboard_Sprite : MonoBehaviour
{
    [SerializeField]
    protected bool RotateWithParent;

    protected Camera camera;

    // Start is called before the first frame update
    private void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (camera != null)
            transform.LookAt(new Vector3(camera.transform.position.x,
                RotateWithParent ? transform.position.y : camera.transform.position.y,
                camera.transform.position.z));
    }
}