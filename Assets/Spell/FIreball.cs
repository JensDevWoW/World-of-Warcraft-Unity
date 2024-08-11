using UnityEngine;

public class Fireball : SpellBase
{

    protected override void Update()
    {
        if (target != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                OnHit();
            }
        }
    }

    void OnHit()
    {
        // Handle what happens when the spell hits the target, e.g., apply damage
        Destroy(gameObject); // Destroy the fireball after it hits
    }
}
