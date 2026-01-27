#region
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
#endregion

public class PaywallTrigger : MonoBehaviour
{
	[SerializeField] List<Sprite> buttonSprites; //sprites for A, B, X, Y buttons
	[SerializeField] HorizontalLayoutGroup horizontalLayout;
	[SerializeField] RectTransform card;

	List<GamepadButton> pressedButtons = new ();

	List<GamepadButton> requiredButtons = new ();

	bool sequenceCompleted;

	Page AssociatedPage => transform.parent.GetComponent<Page>();

	void Awake()
	{
		enabled = false;

		AssociatedPage.BecameActive += _ =>
		{
			enabled = true; // only listen for inputs if the page is active
		};
	}

	void Start()
	{
		requiredButtons.Add(GamepadButton.South); //A
		requiredButtons.Add(GamepadButton.East);  //B
		requiredButtons.Add(GamepadButton.West);  //X
		requiredButtons.Add(GamepadButton.North); //Y
		requiredButtons = requiredButtons.OrderBy(x => Guid.NewGuid()).Take(4).ToList();

		for (int i = 0; i < horizontalLayout.transform.childCount; i++)
		{
			Transform child = horizontalLayout.transform.GetChild(i);

			if (i < requiredButtons.Count)
			{
				var image = child.GetComponent<Image>();

				switch (requiredButtons[i])
				{
					case GamepadButton.South:
						image.sprite = buttonSprites[0]; //A
						break;

					case GamepadButton.East:
						image.sprite = buttonSprites[1]; //B
						break;

					case GamepadButton.West:
						image.sprite = buttonSprites[2]; //X
						break;

					case GamepadButton.North:
						image.sprite = buttonSprites[3]; //Y
						break;
				}

				child.gameObject.SetActive(true);
			}
			else child.gameObject.SetActive(false);
		}

		//display required buttons to user
		Debug.Log("Press the following buttons to remove the paywall: " + string.Join(", ", requiredButtons));
	}
	void Update()
	{
		// check for button presses
		if (Gamepad.current != null)
		{
			foreach (GamepadButton button in requiredButtons)

			{
				if (Gamepad.current != null && Gamepad.current[button].wasPressedThisFrame)
				{
					if (!pressedButtons.Contains(button))
					{
						pressedButtons.Add(button);
						Debug.Log("Button pressed: " + button);
					}
				}
			}
		}
		else
		{
			if (Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				Debug.Log("No gamepad connected. Paywall removed via Escape key.");
				if (!sequenceCompleted)
				{
					sequenceCompleted = true;
					Swipe();
				}
			}
		}

		// check if all required buttons have been pressed
		if (pressedButtons.Count == requiredButtons.Count)
		{
			Debug.Log("All required buttons pressed! Paywall removed.");

			if (!sequenceCompleted)
			{
				sequenceCompleted = true;
				Swipe();
			}
		}
	}

	void Swipe()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(card.DOAnchorPos(new (-300, 100), 1.5f).SetEase(Ease.OutBack));
		sequence.AppendInterval(0.5f);
		sequence.Append(card.DOAnchorPos(new (300, 100), 1.5f).SetEase(Ease.InBack));
		sequence.AppendInterval(1f);
		sequence.AppendCallback(() => { Destroy(gameObject); });
	}
}
