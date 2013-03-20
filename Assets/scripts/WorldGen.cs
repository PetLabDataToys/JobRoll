using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGen : MonoBehaviour {
	
	public Transform[] woodPrefabs;
	
	public List<Transform> pool = new List<Transform>();
	public float[] cachedHeights = new float[0];
	
	public Light sunlight;
	
	const float guiOffset = 192f;
	const float guiWidth = 256f;
	const float guiHeight = 32f;
	
	public static int poolGridSize = 50;
	
	public int varGridSizeX = 80;
	public int varGridSizeY = 80;
	
	public float varHeightScale = 10f;
	public float varFlatnessA = 20f;
	public float varFlatnessDistro = 0.5f;
	public float varFlatnessB = 1f;
	
	public float varCliffSize = 20f;
	public float varCliffDistro = 0.5f;
	
	public bool animating = false;
	
	public static bool showSliders = false;
	
	public GUISkin guiSkin;
	
	void Start () {
		InstantiateGrid();
		AnimateToPerlinGrid();
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown(KeyCode.R) )
//			AnimateToPerlinGrid();
		
	}
	
	void InstantiateGrid () { // call this only ONCE
		// CLEAN-UP
		foreach (Transform block in pool) {
			Destroy(block.gameObject);	
		}
		pool.Clear();
		
		// INSTANTIATE	TODO: to optimize, instantiate pre-made clusters?
		Transform thisTransform = transform; // cache transform
		for (int y=0; y<poolGridSize; y++) {
			for (int x=0; x<poolGridSize; x++) {
				float height = cachedHeights.Length > (x + (y * poolGridSize) ) ? cachedHeights[x + (y * poolGridSize)] : 0f;
				Transform newWood = Instantiate(woodPrefabs[ Mathf.Clamp(Mathf.FloorToInt(Mathf.PerlinNoise(x/2f, y/2f) * woodPrefabs.Length), 0, 3) ], 
									new Vector3 (x, height, y), 
									Quaternion.Euler(new Vector3(0f, 90f * Random.Range(0, 4), 0f) ) ) as Transform;
				newWood.parent = thisTransform;
				pool.Add(newWood);
			}
		}
	}
	
	void AnimateToPerlinGrid () {
		AnimateToPerlinGrid( Random.Range(0f, 100f) );
	}
	
	public void AnimateToPerlinGrid (float seed) {
//		InstantiateGrid();
		
		if (animating)
			return;
		
		animating = true;
		
		// flatness, pre-perlin noise pass	
		const float weightRes = 15f;
		float[] flatnessWeights = new float[varGridSizeX * varGridSizeY];
		for (int y=0; y<varGridSizeY; y++) {
			for (int x=0; x<varGridSizeX; x++) {
				float perlin = Mathf.PerlinNoise(x/weightRes + seed, y/weightRes + seed);
				int currentIndex = x + (y * varGridSizeX);
				flatnessWeights[currentIndex] = perlin;
			}
		}
		
		// initial perlin noise pass
		float[] woodHeights = new float[varGridSizeX * varGridSizeY];
		for (int y=0; y<varGridSizeY; y++) {
			for (int x=0; x<varGridSizeX; x++) {
				int currentIndex = x + (y * varGridSizeX);
				float flatness = flatnessWeights[currentIndex] < varFlatnessDistro ? varFlatnessA : varFlatnessB;
				float perlin = Mathf.PerlinNoise(x/flatness + seed, y/flatness + seed);
				woodHeights[currentIndex] = perlin * varHeightScale;
			}
		}
		
		// cliff pass
		for (int y=0; y<varGridSizeY; y++) {
			for (int x=0; x<varGridSizeX; x++) {
				int currentIndex = x + (y * varGridSizeX);
				float perlin = Mathf.PerlinNoise(x/varCliffSize + seed, y/varCliffSize + seed);
				if (perlin < varCliffDistro)
				woodHeights[currentIndex] = Mathf.Clamp(woodHeights[currentIndex] * (1.62f + Random.Range(0.1f, 0.5f) ), 0f, varHeightScale);
			}
		}
		
		StartCoroutine( AnimateGrid(woodHeights, 2f) );
	}
	
	IEnumerator AnimateGrid (float[] newWoodHeights, float duration) {
		yield return 0;
		const int animateBatchSize = 3000; // increase for smoother (but more expensive) performance
		float t=0; // initialize timer
		
		// pull all old wood scales
		List<float> oldWoodHeights = new List<float>();
		foreach (Transform wood in pool) {
			oldWoodHeights.Add(wood.localPosition.y);
//			wood.collider.enabled = false;
		}
		
		// calculate offset to center varGrid inside Pool
		int offset = Mathf.FloorToInt( (poolGridSize - varGridSizeX) / 2f);
		
		// build a dictionary to remap pool indices to (smaller) grid indices
		Dictionary<int, int> gridToPool = new Dictionary<int, int>();
		for (int y=0; y<varGridSizeY; y++) {
			for (int x=0; x<varGridSizeX; x++) {
				gridToPool.Add(offset + x + ( (offset + y) * poolGridSize), x + y * varGridSizeX);
			}
		}
		
		yield return new WaitForSeconds(0.25f);
		
		float startTime = Time.time;
		
		while (t < 1f) {
			float frameLength = Time.time - startTime;
			startTime = Time.time;	
			t += frameLength / duration;
			
			int counter = 0;
			for (int i=0; i<pool.Count; i++) {
				float newHeight = gridToPool.ContainsKey(i) ? newWoodHeights[gridToPool[i]] : -15f;
				if (oldWoodHeights[i] == newHeight) // don't lerp if we're already there
					continue;
					
				pool[i].localPosition = new Vector3 ( pool[i].localPosition.x, Mathf.Lerp(oldWoodHeights[i], newHeight, t), pool[i].localPosition.z );
				counter++;
				if (counter > animateBatchSize) {
					counter = 0;
					yield return 0;
				}
			}
			yield return 0;
		}
		
		// turn on collision again
//		foreach (Transform wood in woods) {
//			wood.collider.enabled = true;
//		}
		
		animating = false;
		
		cachedHeights = newWoodHeights;
		
		// batch
//		StaticBatchingUtility.Combine(this.gameObject);
	}
	
	
	void OnGUI () {
		
		GUI.skin = guiSkin;

		
		if (GUI.Button(new Rect(0f, 0f, guiWidth * 0.6f, guiHeight), showSliders ? "HIDE SETTINGS" : "SHOW SETTINGS") )
			showSliders = !showSliders;
		
		GUI.enabled = animating ? false : true;
		if (GUI.Button(new Rect(guiWidth * 0.6f, 0f, guiWidth, guiHeight), animating ? "Working, please wait..." : "Re-generate Terrain!") )
			AnimateToPerlinGrid();
		
		int counter = 1;
		if (showSliders) {	
			GUI.Box(new Rect(0f, guiHeight, guiWidth * 2f, guiHeight * 7f), "");
			varGridSizeX = Mathf.RoundToInt(GUISlider(counter, "World Size (" + varGridSizeX.ToString() + ")", varGridSizeX, 15f, poolGridSize) );
			varGridSizeY = varGridSizeX;
			counter++;
			
			varHeightScale = GUISlider(counter, "Bump Height (" + varHeightScale.ToString() + ")", varHeightScale, 1f, 25f);
			counter++;
			
			varFlatnessA = GUISlider(counter, "Flatness A (" + varFlatnessA.ToString("##.##") + ")", varFlatnessA, 5f, 25f);
			counter++;
			
			varFlatnessB = GUISlider(counter, "Flatness B (" + varFlatnessB.ToString("##.##") + ")", varFlatnessB, 5f, 25f);
			counter++;			
			
			varFlatnessDistro = GUISlider(counter, "Flat Distro (A" + varFlatnessDistro.ToString("#.##") + " B" + (1f-varFlatnessDistro).ToString("#.##") + ")", varFlatnessDistro, 0f, 1f);
			counter++;				
			
			varCliffSize = GUISlider(counter, "Cliff Width (" + varCliffSize.ToString() + ")", varCliffSize, 5f, 25f);
			counter++;
			
			varCliffDistro = GUISlider(counter, "Cliff Distro (" + varCliffDistro.ToString("#.##") + ")", varCliffDistro, 0f, 1f);
			counter++;	
		}
		
		GUI.enabled = true;
		if (GUI.Button(new Rect(Screen.width - 64f, 0f, 64f, 32f), "EXIT") )
			Application.Quit();
	}
	
	float GUISlider (int counter, string label, float variable, float leftLimit, float rightLimit) {
		GUI.Label(new Rect(0f, counter * guiHeight, 192f, guiHeight), label );
		return GUI.HorizontalSlider(new Rect(guiOffset, counter * guiHeight, guiWidth, guiHeight), variable, leftLimit, rightLimit);
	}
}
