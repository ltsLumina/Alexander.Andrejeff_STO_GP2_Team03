using UnityEngine;

[CreateAssetMenu(fileName = "Ripped", menuName = "Page Modifiers/Ripped", order = 2)]
public class RippedModifier : PageModifier
{
	public override void Apply(Page page)
	{
		var go = Instantiate(prefab, page.transform, false);
	}
}
