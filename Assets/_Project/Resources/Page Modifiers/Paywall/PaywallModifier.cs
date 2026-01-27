using UnityEngine;

[CreateAssetMenu(fileName = "Paywall", menuName = "Page Modifiers/Paywall", order = 4)]
public class PaywallModifier : PageModifier
{
	public override void Apply(Page page)
	{
		var go = Instantiate(prefab, page.transform, false);
	}
}
