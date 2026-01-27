using UnityEngine;

[CreateAssetMenu(fileName = "Advertisement", menuName = "Page Modifiers/Advertisement", order = 3)]
public class AdvertisementModifier : PageModifier
{
	public override void Apply(Page page)
	{
		var go = Instantiate(prefab, page.transform, false);
		go.SetActive(false);
	}
}
