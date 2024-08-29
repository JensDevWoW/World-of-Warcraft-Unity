using UnityEngine;

public abstract class CreatureScript : MonoBehaviour
{
    public Unit unit;
    public virtual void Update()
    {
        // Default
    }

    public virtual void Died()
    {
        // Default
    }

    public virtual void Reset()
    {
        // Default
    }

    public virtual void OnSpawn()
    {
        // Default
    }

}
