using UnityEngine;

public enum PartID : int
{
    Leg,
    FourByFour,
}

public class PartIdentifier : MonoBehaviour
{
    [field: SerializeField] public string PartID { get; private set; }
}