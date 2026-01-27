using UnityEngine;

[CreateAssetMenu(fileName = "Stained", menuName = "Page Modifiers/Stained", order = 1)]
public class StainedModifier : PageModifier
{
	public override void Apply(Page page)
	{
		var go = Instantiate(prefab, page.transform, false);

		// randomize position on the page
		var rectTransform = page.GetComponent<RectTransform>();

		// leave some margin at top/bottom and left/right (in pixels)
		float verticalMargin = 150f;
		float horizontalMargin = 150f;

		float X = rectTransform.rect.width / 2f - horizontalMargin;
		float Y = rectTransform.rect.height / 2f - verticalMargin;
		X = Mathf.Max(0f, X);
		Y = Mathf.Max(0f, Y);

		((RectTransform) go.transform).anchoredPosition = new Vector3(Random.Range(-X, X), Random.Range(-Y, Y), 0f);
		//((RectTransform) go.transform).rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
	}
}
