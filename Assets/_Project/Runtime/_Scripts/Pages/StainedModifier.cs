using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Stained", menuName = "Page Modifiers/Stained", order = 1)]
public class StainedModifier : PageModifier
{
	[SerializeField] Texture2D stainTexture;
	[SerializeField] float scale = 2f;

	public override void Apply(GameObject parent)
	{
		var asset = Resources.Load<GameObject>("Page Modifiers/Stain/Stain");
		var go = Instantiate(asset.gameObject, parent.transform, false);

		// randomize position on the page
		var rectTransform = parent.GetComponent<RectTransform>();

		// leave some margin at top/bottom and left/right (in pixels)
		float verticalMargin = 100f;
		float horizontalMargin = 100f;

		float X = rectTransform.rect.width / 2f - horizontalMargin;
		float Y = rectTransform.rect.height / 2f - verticalMargin;
		X = Mathf.Max(0f, X);
		Y = Mathf.Max(0f, Y);

		((RectTransform) go.transform).anchoredPosition = new Vector3(Random.Range(-X, X), Random.Range(-Y, Y), 0f);
		((RectTransform) go.transform).rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		
		var image = go.GetComponent<Image>();
		image.sprite = Sprite.Create(stainTexture, new Rect(0, 0, stainTexture.width, stainTexture.height), new Vector2(0.5f, 0.5f));
		go.transform.localScale *= scale;
	}
}
