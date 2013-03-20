using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CityType { Recruiting, Fast, Large }; 	// Large = largest concentration of foreign-born population
[System.Serializable]								// Fast = fastest-growing foreign-born population
public class City {									// Recruiting = actively-recruiting foreign-born
	public string name = "CITY_NAME";
	public CityType type = CityType.Fast; // TODO: determines which terrain paradigm to use
	public CityData[] data;
	
	[HideInInspector]
	public float seed = -1f; // randomize this at beginning of game, but don't change during the game
}

public enum Year { _2000, _2005, _2010 }
[System.Serializable]
public class CityData {
	public Year year = Year._2000;
	public int totalPop;
	public float immShare;
	public float immShareEmploy; // isn't this generally a correlation of immShare?
	
	[HideInInspector]
	public float employRatio; //  (immShareEmploy / immShare)
}

public class CityDataDB : MonoBehaviour {
	public City[] cities;
	WorldGen worldGen;
	
	City currentCity;
	Year currentYear = Year._2000;
	CityData currentData { get { return currentCity.data[(int)currentYear]; } }
	
	int avgCityPopulation = 0;
	float bestEmployRatio = 0f;
	
	const float flatBbase = 10f;
	
	public GUISkin skin;
	
	public GUIText console;
	int consoleCounter = 0;

	// Use this for initialization
	void Start () {
		worldGen = GetComponent<WorldGen>();
		
		foreach (City city in cities) {
			city.seed = Random.Range(0f, 100f);
			foreach (CityData dat in city.data) {
				dat.employRatio = dat.immShareEmploy / dat.immShare;
				avgCityPopulation += dat.totalPop;
				
				if (dat.employRatio > bestEmployRatio)
					bestEmployRatio = dat.employRatio;
			}
		}
		
//		currentCity = cities[Random.Range(0, cities.Length)];
		avgCityPopulation /= (cities.Length * cities[0].data.Length);
	}
	
	void Update () {
		if (WorldGen.showSliders) {
			console.enabled = true;
		} else {
			console.enabled = false;
		}
	}
	
	void ChangeWorldGenSettings () {
		CityData dat = currentData;
		ConsoleLog(currentCity.name + "(" + currentYear.ToString() + "): START TERRAIN MORPH...");
		
		// THIS WORKS pretty well as a scale mechanism, but we don't want small levels?
		float popVsAll = dat.totalPop * 1f / avgCityPopulation * 1f;
		ConsoleLog(currentCity.name + "(" + currentYear.ToString() + "): Population vs All: " + popVsAll);
 		worldGen.varGridSizeX = Mathf.Clamp(Mathf.RoundToInt(popVsAll * (WorldGen.poolGridSize * 0.75f)), 10, WorldGen.poolGridSize);
		
//		float employRatio = 
//		worldGen.varGridSizeX = Mathf.Clamp(Mathf.RoundToInt(
		
		worldGen.varGridSizeY = worldGen.varGridSizeX;
		
		float employVsAll = dat.employRatio / bestEmployRatio; // float 0-1
		ConsoleLog(currentCity.name + "(" + currentYear.ToString() + "): Employ vs All: " + employVsAll );
		worldGen.varFlatnessA = 20f;
//		worldGen.varFlatnessB = 1f + flatBbase * employVsAll; // IS THIS GOOD MATH??? AHHH
		worldGen.varFlatnessB = 5f + (10f * ( (dat.employRatio - 1f) / 0.4f) );
		worldGen.varFlatnessDistro = 0.2f + (dat.employRatio - 1f);
//		worldGen.varFlatnessDistro = employVsAll * employVsAll;
		
		worldGen.AnimateToPerlinGrid(currentCity.seed);
	}
	
	void ConsoleLog(string line) {
		consoleCounter++;
		if (consoleCounter > 6) {
			consoleCounter = 0;
			console.text = "";
		}
		
		Debug.Log(line);
		console.text += "\n" + line;
	}
	
	void OnGUI () {
		GUI.skin = skin;
		
		const float guiWidth = 192f;
		const float guiHeight = 32f;
		
		if (worldGen.animating)
			GUI.enabled = false;
		else
			GUI.enabled = true;
		
		Year lastYear = currentYear;
		int currentRow = 0;
		int counter = 2;
		foreach (City cit in cities) {
			if (cit == currentCity) {
				currentRow = counter;	
				float yearOffset = Screen.width - guiWidth;
				GUI.Box(new Rect(yearOffset, guiHeight * (currentRow), guiWidth, guiHeight * 5), currentCity.name);
				GUI.Label(new Rect(yearOffset, guiHeight * (currentRow+1), guiWidth, guiHeight), "YEAR: " + currentYear.ToString() );
				currentYear = (Year)Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(yearOffset, guiHeight * (currentRow+2), guiWidth, guiHeight), (int)currentYear, 0f, 2f ) );
				GUI.Label(new Rect(yearOffset, guiHeight * (currentRow+3), guiWidth, guiHeight * 4), "POP: " + currentData.totalPop.ToString() + "\n% POP IMM: " + (currentData.immShare * 100f).ToString() + "%\nJOB/POP SHARE " + currentData.employRatio.ToString("#.##") );
				if (lastYear != currentYear)
					ChangeWorldGenSettings();
				counter += 5;
			} else  {
				if (GUI.Button(new Rect(Screen.width - guiWidth, guiHeight * counter, guiWidth, guiHeight), cit.name) ) {
					currentCity = cit;
					ChangeWorldGenSettings();
				}
				counter++;
			}
		}
		


	}
	
	// if the player adjusted worldGen settings, this finds the city closest to those settings
	void GetClosestCity () {
		
	}
}
