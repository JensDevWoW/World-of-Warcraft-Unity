using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    private Transform unitTransform;

    void Awake()
    {
        unitTransform = GetComponent<Transform>();
    }

    // Method to get the current position of the unit
    public Vector3 GetPosition()
    {
        return unitTransform.position;
    }

    // Method to set the position of the unit
    public void SetPosition(Vector3 newPosition)
    {
        unitTransform.position = newPosition;
    }

    // Method to get the distance from another unit
    public float GetDistanceFrom(Unit otherUnit)
    {
        return Vector3.Distance(unitTransform.position, otherUnit.transform.position);
    }

    // Method to move the unit towards a target position
    public void MoveTowards(Vector3 targetPosition, float speed)
    {
        unitTransform.position = Vector3.MoveTowards(unitTransform.position, targetPosition, speed * Time.deltaTime);
    }

    // You can add more location-based methods here as needed
}
