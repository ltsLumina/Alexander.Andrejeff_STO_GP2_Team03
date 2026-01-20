#region
using System.Collections.Generic;
using JetBrains.Annotations;
using Lumina.Essentials.Modules;
using UnityEngine;
using VInspector;
#endregion

public class Page : MonoBehaviour
{
	[SerializeField] Texture2D texture;
	[SerializeField] PageModifier modifier;
	[SerializeField] float modifierChance = 0.5f;

#if UNITY_EDITOR
	[Button, UsedImplicitly]
	void Stain() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Stained"), true, true);

	[Button] [UsedImplicitly]
	void Rip() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Ripped"), true, true);

	[Button] [UsedImplicitly]
	void Tape() => SetModifier(Resources.Load<PageModifier>("Page Modifiers/Taped"), true, true);
	
	/// <summary>
	/// Sets the page modifier for debugging purposes.
	/// </summary>
	/// <param name="newModifier"></param>
	/// <param name="overwrite"> Overwrite existing modifier if any. (Destroys all children!!) </param>
	/// <param name="applyImmediately"> Applies the modifier immediately, otherwise you must call Apply() manually. </param>
	void SetModifier(PageModifier newModifier, bool overwrite = true, bool applyImmediately = false)
	{
		if (overwrite) transform.DestroyAllChildren();
		modifier = newModifier;
		if (applyImmediately) modifier?.Apply(gameObject);
	}
#endif

	public PageModifier Modifier => modifier;

	public void SetRandomModifier()
	{
		PageModifier[] modifiers = Resources.LoadAll<PageModifier>("Page Modifiers");

		if (Random.value < modifierChance)
		{
			modifier = null;
			return;
		}

		PageModifier randomModifier = modifiers.Length > 0 ? modifiers[Random.Range(0, modifiers.Length)] : null;

		modifier = randomModifier;
	}
}

public static class PageExtensions
{
	public static Vector2 GetPageBounds(this Page page, bool includeMargin = true)
	{
		const float marginWidth = 100f;
		const float marginHeight = 100f;

		var rectTransform = page.GetComponent<RectTransform>();
		var width = rectTransform.rect.width;
		var height = rectTransform.rect.height;

		if (includeMargin) return new Vector2(width / 2f - marginWidth, height / 2f - marginHeight);
		return new Vector2(width / 2f, height / 2f);
	}
}
