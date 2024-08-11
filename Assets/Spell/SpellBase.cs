using UnityEngine;

public class SpellBase : MonoBehaviour
{
    protected Transform caster;
    protected Transform target;
    protected float speed;

    public virtual void Initialize(Transform caster, Transform target, float Speed)
    {
        this.caster = caster;
        this.target = target;
        this.speed = Speed;
        this.enabled = false; // Start disabled, will be enabled when the spell is cast
    }

    protected virtual void Update()
    {
        // Implement spell movement and logic here, e.g., move towards the target
        // Override this method in derived classes for different behaviors
    }
}
