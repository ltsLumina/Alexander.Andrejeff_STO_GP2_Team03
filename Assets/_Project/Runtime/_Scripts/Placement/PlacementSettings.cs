using UnityEngine;

[CreateAssetMenu(fileName = "New Placement Settings", menuName = "Placement/Placement Settings")]
public class PlacementSettings : ScriptableObject
{
    [Header("References")]
    [field: SerializeField] public LayerMask PlacementLayerMask { get; private set; }
    [field: SerializeField] public Material PreviewMaterial { get; private set; }

    [Header("Settings")]
    [field: SerializeField] public float PieceProjectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float HorizontalRotateStep { get; private set; } = 90f;
    [field: SerializeField] public float VerticalRotateStep { get; private set; } = 90f;
    
    [Header("Visuals")]
    [field: SerializeField] public float WobbleStrength { get; private set; } = 5f;
    [field: SerializeField] public float WobbleSpeed { get; private set; } = 5f;
}