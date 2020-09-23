using UnityEngine;

public class ViewModelMotion : MonoBehaviour
{
    [SerializeField]
    protected float recoveryRate;

    [SerializeField]
    protected float maxPunch; //Z

    [SerializeField]
    protected FPSController.FPSController player;

    //The motion always wants to settle back to root position on each axis.
    protected Vector3 motion;

    protected Vector3 rootPostition;

    // Start is called before the first frame update
    private void Start()
    {
        motion = new Vector3(0, 0, 0);
        rootPostition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePunch();
        UpdatePosition();
    }

    public void Punch(float amount)
    {
        motion = new Vector3(motion.x, motion.y, Mathf.Clamp(amount + motion.z, -maxPunch, maxPunch));
    }

    //Weapon punch
    protected void UpdatePunch()
    {
        var newPunch = Mathf.Lerp(motion.z, 0, Time.deltaTime * recoveryRate);

        motion = new Vector3(motion.x, motion.y, newPunch);
    }

    protected void UpdatePosition()
    {
        transform.localPosition = new Vector3(rootPostition.x + motion.x,
            rootPostition.y + motion.y,
            rootPostition.z + motion.z);
    }
}