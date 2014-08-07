using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class FireWorkParticle: MonoBehaviour {
	public Texture2D baseTexture;	//花火パーティクルの元になるテクスチャ。　readable で　フルカラーじゃないとエラーが出るので注意;
	[Range(1,128)]public int particleWidth = 64;	//パーティクル個数　横
	[Range(1,128)]public int particleHeight = 64;	//パーティクル個数　縦
	[Range(0.1f, 10.0f)]public float particleMinSize = 0.2f, particleMaxSize = 1.0f;	//1パーティクルのサイズの幅
	[Range(0.1f, 10.0f)]public float exploadMinPow = 1.0f, exploadMaxPow = 2.0f;	//花火の爆発の強さ　＝　広がり具合
	[Range(0.1f, 4.0f)]public float slowdownTime = 0.3f;	//爆発で広がってから速度が収束するまでの時間
	public bool isRandomColor = false;	//パーティクルにランダムで色をセットするかどうか
	public Material particleMaterial;	//パーティクルで使用するマテリアル
	public Vector3 randomRotation = new Vector3(15, 15, 180);
	public GameObject lookAtTarget;
	public Vector3 lookAtPosition;
	[HideInInspector]
	public bool isPreview;

	private float _slowDownTime;
	private ParticleSystem ps;
	private ParticleSystem.Particle[] particles;

	public void CreateParticle() {
		if (baseTexture == null) {
			GameObject.Destroy(gameObject);
			return;
		}

		_slowDownTime = slowdownTime;

		if (lookAtTarget) {
			lookAtPosition = lookAtTarget.transform.position;
		}
		transform.LookAt(lookAtPosition);

		//実際の花火のようにどの方向で開くかちょっとランダムを入れる
		transform.Rotate(Random.Range(-randomRotation.x, randomRotation.x), Random.Range(-randomRotation.y, randomRotation.y), Random.Range(-randomRotation.z, randomRotation.z));

		//すでにParticleSystemがComponentにある場合はそれを使う。ない場合は追加
		ps = GetComponent<ParticleSystem>();
		if (!ps) ps = gameObject.AddComponent<ParticleSystem>();
		ps.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		int particleSize = particleWidth * particleHeight;
		ps.loop = false;
		ps.gravityModifier = 1.0f;
		ps.playOnAwake = false;
		ps.maxParticles = particleSize;
		if (particleMaterial) ps.renderer.material = particleMaterial;
		ps.startLifetime = 5.0f;

		ps.Emit(particleSize);
		particles = new ParticleSystem.Particle[particleSize];
		ps.GetParticles(particles);

		//画素ピックアップ間隔
		float cx = baseTexture.width / (float)particleWidth;
		float cy = baseTexture.height / (float)particleHeight;

		Color[] pixels = baseTexture.GetPixels(0, 0, baseTexture.width, baseTexture.height);	//画素データ取得

		float explodePower = Random.Range(exploadMinPow, exploadMaxPow);

		int particleCount = 0;
		for (int y = 0; y < particleHeight; y++) {
			for (int x = 0; x < particleWidth; x++) {
				Color col = pixels[(int)(x * cx) + (int)(y * cy) * baseTexture.width];
				if (col.a < 0.1f) continue;	//透明すぎなので作らない
				if (col.r + col.g + col.b < 0.2f) continue;	//黒過ぎなので作らない

				if (isRandomColor) {	//ランダム指定がされている場合
					Color baseColor = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
					col *= baseColor;
				}

				Vector3 targetPos = new Vector3(x - particleWidth * 0.5f, y - particleHeight * 0.5f, 0);	//目的地を設定
				ParticleSystem.Particle p = particles[particleCount];
				p.startLifetime = 10.0f;
				p.position = Vector3.zero;
				p.rotation = Random.Range(0.0f, 360.0f);
				Vector3 targetVec = targetPos - p.position;
				p.size = Random.Range(particleMinSize, particleMaxSize);	//パーティクルのサイズ設定
				p.color = col;
				p.velocity = targetVec.normalized * targetVec.magnitude * explodePower + transform.forward * Random.Range(-1.0f, 1.0f) + Vector3.up;
				particles[particleCount] = p;
				particleCount++;
			}
		}
		ps.SetParticles(particles, particleCount);
	}

	// Use this for initialization
	void Start () {
		CreateParticle();
	}
	
	// Update is called once per frame
	void Update () {
		_slowDownTime -= Time.deltaTime;

		if (_slowDownTime < 0) {
			int num = ps.GetParticles(particles);
			if (num <= 0 && isPreview == false) {
				GameObject.Destroy(gameObject);//全てのパーティクルの寿命が尽きたら自分も消える
				return;
			}
			for(int i = 0;i < num;i++){
				particles[i].velocity *= Mathf.Pow(0.90f,Time.deltaTime * 60.0f);
				particles[i].size *= Mathf.Pow( 0.999f, Time.deltaTime * 60.0f);
				particles[i].color *= new Color(1.0f, 1.0f, 1.0f, Mathf.Pow(0.97f, Time.deltaTime * 60.0f));
				if (particles[i].color.a <= 0.05f) particles[i].lifetime = 0.0f;
			}
			ps.SetParticles(particles, num);
		}
	}

#if UNITY_EDITOR
	void OnApplicationQuit() {
		SaveParticleData();
	}

	public void SaveParticleData() {
		var data = ScriptableObject.CreateInstance<FireWorkData>();
		data.baseTexture = baseTexture;
		data.particleWidth = particleWidth;
		data.particleHeight = particleHeight;
		data.particleMinSize = particleMinSize;
		data.exploadMinPow = exploadMinPow;
		data.slowdownTime = slowdownTime;
		data.isRandomColor = isRandomColor;
		data.particleMaterial = particleMaterial;
		data.randomRotation = randomRotation;
		data.lookAtTarget = lookAtTarget;
		data.lookAtPosition = lookAtPosition;

		string dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetOrScenePath(this));
		string path = dirPath + "/" + typeof(FireWorkData) + ".asset";
		AssetDatabase.CreateAsset(data, path);
		AssetDatabase.Refresh();
	}

	public void LoadParticleData() {
		string dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetOrScenePath(this));
		string path = dirPath + "/" + typeof(FireWorkData) + ".asset";
		var data = AssetDatabase.LoadAssetAtPath(path, typeof(FireWorkData)) as FireWorkData;

		baseTexture = data.baseTexture;
		particleWidth = data.particleWidth;
		particleHeight = data.particleHeight;
		particleMinSize = data.particleMinSize;
		exploadMinPow = data.exploadMinPow;
		slowdownTime = data.slowdownTime;
		isRandomColor = data.isRandomColor;
		particleMaterial = data.particleMaterial;
		randomRotation = data.randomRotation;
		lookAtTarget = data.lookAtTarget;
		lookAtPosition = data.lookAtPosition;
	}
#endif
}
