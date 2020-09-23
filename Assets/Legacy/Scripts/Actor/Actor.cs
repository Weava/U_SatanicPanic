using Assets.Scripts.Misc;
using Assets.Scripts.Projectiles;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    [SerializeField]
    protected Gib gib;

    [SerializeField]
    protected float maxHealth;

    protected float currentHealth;

    protected Rigidbody rigidBody;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Instantiate(gib, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }

    public virtual void AddHealth(float amount)
    {
        currentHealth += amount;
    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        var damageSource = collider.gameObject.GetComponent<DamageSource>();
        if (damageSource != null)
        {
            AddHealth(damageSource.GetImpactDamage());
        }
    }
}