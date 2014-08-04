using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof(FireWorkParticle) )]
public class FireWorkEditor : Editor {
	private bool isFoldOut = false;
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		FireWorkParticle fwp = target as FireWorkParticle;

		isFoldOut = EditorGUILayout.Foldout(isFoldOut, "Extend Editor");
		if (isFoldOut) {
			EditorGUILayout.MinMaxSlider(new GUIContent("ParticleSize"), ref fwp.particleMinSize, ref fwp.particleMaxSize, 0.01f, 5.0f);
			EditorGUILayout.MinMaxSlider(new GUIContent("ExplodePower"), ref fwp.exploadMinPow, ref fwp.exploadMaxPow, 0.1f, 10.0f);
			GUIStyle style = new GUIStyle();
			EditorGUILayout.BoundsField(new GUIContent(fwp.tex),new Bounds(Vector3.zero,new Vector3(32,32,32)));
			GUILayout.Box(fwp.tex);
			EditorUtility.SetDirty(target);
		}

		var path = UnityEditor.AssetDatabase.GetAssetPath(fwp.tex);
		var importer = AssetImporter.GetAtPath(path) as TextureImporter;
		var isReadable = importer.isReadable;

		if (isReadable == false) {
			importer.isReadable = true;
			importer.textureFormat = TextureImporterFormat.ARGB32;
			importer.filterMode = FilterMode.Point;
			EditorUtility.SetDirty(importer);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}


	}
}
