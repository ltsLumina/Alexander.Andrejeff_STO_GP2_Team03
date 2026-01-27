#region
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

public class PageUpdater : EditorWindow
{
	enum PageType
	{
		Level1,
		Level2,
		Level3,
		Bloat
	}

	[MenuItem("Tools/Upload Page")]
	static void Create()
	{
		var window = GetWindow<PageUpdater>();
		window.titleContent = new GUIContent("Page Updater");
		window.maxSize = new Vector2(300, 50);
		window.minSize = window.maxSize;
		window.Show();
	}

	static PageType pageType = PageType.Level1;
	
	void OnGUI()
	{
		using (new EditorGUILayout.VerticalScope())
		{
			pageType = (PageType)EditorGUILayout.EnumPopup("Select Page Type:", pageType);
			
			if (GUILayout.Button("Upload Page"))
			{
				UploadPage();
			}
		}
	}
	
	static void UploadPage()
	{
		string path = EditorUtility.OpenFilePanelWithFilters("Select Page Image", "", new[] { "Image files", "png,jpg,jpeg", "All files", "*" });

		if (!string.IsNullOrEmpty(path))
		{
			byte[] fileData = System.IO.File.ReadAllBytes(path);
			Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false); // disables mipmaps
			tex.LoadImage(fileData);
			pageImage = tex;
		}

		if (pageImage != null)
		{
			var savedPath = EditorUtility.SaveFilePanel("Save Page Image", $"Assets/_Project/Resources/Instruction Pages/{ObjectNames.NicifyVariableName(pageType.ToString())}", "page_X.png", "png");

			if (!string.IsNullOrEmpty(savedPath) && pageImage is Texture2D texture2D)
			{
				byte[] pngData = texture2D.EncodeToPNG();
				System.IO.File.WriteAllBytes(savedPath, pngData);

				var relativePath = "Assets" + savedPath[Application.dataPath.Length..];
				var imageFile = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
				EditorGUIUtility.PingObject(imageFile);
			}
			
			if (!string.IsNullOrEmpty(savedPath))
			{
				AssetDatabase.Refresh();
				
				var textureImporter = AssetImporter.GetAtPath("Assets" + savedPath[Application.dataPath.Length..]) as TextureImporter;

				if (textureImporter != null)
				{
					textureImporter.textureType = TextureImporterType.Sprite;
					textureImporter.spriteImportMode = SpriteImportMode.Single;
					textureImporter.SaveAndReimport();
					
					AssetDatabase.Refresh();
				}
			}

			if (!string.IsNullOrEmpty(savedPath))
			{
				EditorUtility.DisplayDialog("Success", "Page image uploaded successfully!", "OK");
			}
			else if (!EditorUtility.DisplayDialog("Error", "Failed to save the page image.", "OK", "Why"))
			{
				Application.OpenURL("https://www.reddit.com/media?url=https%3A%2F%2Fi.redd.it%2Fi8ltwa7ywxjc1.gif");
			}
		}
	}

	static Object pageImage;
}
