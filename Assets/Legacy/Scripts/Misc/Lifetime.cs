using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float lifeTime;

    private float currentLife = 0;

    // Update is called once per frame
    private void Update()
    {
        if (currentLife < lifeTime)
        {
            currentLife += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}