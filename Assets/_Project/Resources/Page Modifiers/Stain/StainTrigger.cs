#region
using UnityEngine;
using UnityEngine.UI;
#endregion

public class StainTrigger : MonoBehaviour
{
	int currentClicks;

	Image image;
	
	void Awake()
	{
		image = GetComponent<Image>();
	}

	public void Click()
	{
		if (currentClicks >= 3) Destroy(gameObject);
	}

	public void PointerEnter()
	{
		currentClicks++;
		
		var rectTransform = image.rectTransform;
		Vector2 bounds = new Vector2(960, 1080);

		// random position within bounds
		rectTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(-bounds.x, bounds.x), UnityEngine.Random.Range(-bounds.y, bounds.y));
	}
}
