using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[InitializeOnLoad]
public class FireWorkData : ScriptableObject {
	readonly static string[] labels = { "Data", "ScriptableObject" };

	public Texture2D baseTexture;	//花火パーティクルの元になるテクスチャ。　readable で　フルカラーじゃないとエラーが出るので注意;
	[Range(1, 128)]
	public int particleWidth = 64;	//パーティクル個数　横
	[Range(1, 128)]
	public int particleHeight = 64;	//パーティクル個数　縦
	[Range(0.1f, 10.0f)]
	public float particleMinSize = 0.2f, particleMaxSize = 1.0f;	//1パーティクルのサイズの幅
	[Range(0.1f, 10.0f)]
	public float exploadMinPow = 1.0f, exploadMaxPow = 2.0f;	//花火の爆発の強さ　＝　広がり具合
	[Range(0.1f, 4.0f)]
	public float slowdownTime = 0.3f;	//爆発で広がってから速度が収束するまでの時間
	public bool isRandomColor = false;	//パーティクルにランダムで色をセットするかどうか
	public Material particleMaterial;	//パーティクルで使用するマテリアル
	public Vector3 randomRotation = new Vector3(15, 15, 180);
	public GameObject lookAtTarget;
	public Vector3 lookAtPosition;
}
