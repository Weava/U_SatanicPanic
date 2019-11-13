using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float lifeTime;

    float currentLife = 0;

    // Update is called once per frame
    void Update()
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
