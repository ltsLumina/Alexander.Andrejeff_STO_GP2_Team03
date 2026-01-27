#region
using System;
using System.Text;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
#endregion

public class MenuAnimation : MonoBehaviour // WIP, prototype
{
	[SerializeField] float startY;
	[SerializeField] float endY;
	
	Sequence hoverEnd;

	Sequence hoverStart;
	RectTransform rectTransform;

	void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		rectTransform.anchoredPosition = Vector2.up * startY;

		StatusBarManager.Instance.Begin();
	}

	[SerializeField] Button playButton;
	
	void Update()
	{
		if (Gamepad.current.aButton.isPressed)
		{
			OnHoverStart(playButton);
			enabled = false;
		}
	}

	bool hasHovered;

	public void OnHoverStart(Button button)
	{
		if (hasHovered) return;
		hasHovered = true;

		hoverStart = DOTween.Sequence();
		hoverStart.SetLink(gameObject);
		hoverStart.OnStart(() => button.interactable = false);
		hoverStart.Append(rectTransform.DOAnchorPosY(endY, 1f).SetEase(Ease.InOutBack));
		hoverStart.OnComplete(() =>
		{
			button.interactable = true;
			
			EventSystem.current.SetSelectedGameObject(playButton.gameObject);
		});
	}

	public void Play()
	{
		SceneManagerExtended.LoadNextScene();
	}
	public void Settings()
	{
		Debug.LogWarning("Not implemented yet.");
	}
	public void Quit()
	{
		Application.Quit();
		
		#if UNITY_EDITOR
		if (EditorUtility.DisplayDialog("Quit", "Application.Quit() called." 
		                                        + "\nPress OK to exit play mode, or Cancel to stay in play mode.", "OK", "Cancel"))
		{
			EditorApplication.isPlaying = false;
		}
		#endif
	}
}
