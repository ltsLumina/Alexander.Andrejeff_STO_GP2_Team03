using UnityEngine;

public abstract class PageModifier : ScriptableObject
{
	[SerializeField] protected GameObject prefab;
	[SerializeField] int maxInstances = 1;
	[Tooltip("Chance (0..1) for this modifier to be applied when generating a page.")]
	[Range(0,1f)]
	[SerializeField] float modifierChance = 0.5f;

	public int MaxInstances => maxInstances;
	public float ModifierChance => modifierChance;

	/// <summary>
	/// Applies the modification to the given page.
	/// </summary>
	/// <param name="page">The parent page object to attach the modification to.</param>
	public abstract void Apply(Page page);
}