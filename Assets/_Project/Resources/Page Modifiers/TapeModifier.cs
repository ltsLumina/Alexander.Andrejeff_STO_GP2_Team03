using UnityEngine;

[CreateAssetMenu(fileName = "Tape", menuName = "Page Modifiers/Tape", order = 3)]
public class TapeModifier : PageModifier
{
	public override void Apply(GameObject parent)
	{
		var asset = Resources.Load<GameObject>("Page Modifiers/Tape/Tape");
		var go = Instantiate(asset.gameObject, parent.transform, false);
		var rectTransform = go.GetComponent<RectTransform>();

		bool horizontal = Random.value > 0.5f;
		if (horizontal)
		{
			//rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y, rectTransform.sizeDelta.x);
			rectTransform.rotation = Quaternion.Euler(0f, 0f, 90f);
		}
		
		var pos = rectTransform.anchoredPosition;
		if (!horizontal) pos.x += Random.Range(-200f, 200f);
		else pos.y += Random.Range(-300f, 300f);
		rectTransform.anchoredPosition = pos;
	}
}
