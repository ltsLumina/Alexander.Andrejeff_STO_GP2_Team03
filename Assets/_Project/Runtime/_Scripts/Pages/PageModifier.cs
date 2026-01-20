using UnityEngine;

public abstract class PageModifier : ScriptableObject
{
	public abstract void Apply(GameObject parent);
}