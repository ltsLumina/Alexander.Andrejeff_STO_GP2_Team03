#region
using System;
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
		currentClicks++;

		switch (currentClicks)
		{
			case < 3: {
				Color color = image.color;
				color.a -= 0.15f;
				image.color = color;
				break;
			}

			case >= 3:
				image.CrossFadeAlpha(0, 0.25f, false);
				break;
		}
	}
}
