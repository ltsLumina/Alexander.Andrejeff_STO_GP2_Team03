using UnityEngine;

[CreateAssetMenu(fileName = "Ripped", menuName = "Page Modifiers/Ripped", order = 2)]
public class RippedModifier : PageModifier
{
	// Additional properties or methods specific to ripped modifier can be added here

	public override void Apply(GameObject parent)
	{
		var asset = Resources.Load<GameObject>("Page Modifiers/Rip/Rip");
		var go = Instantiate(asset.gameObject, parent.transform, false);
	}
}
