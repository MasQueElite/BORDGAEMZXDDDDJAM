using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class BORDGAEMXD : MonoBehaviour
{

	public KMBombInfo Bomb;
	public KMBombModule Module;
	public KMAudio Audio;

	public TextMesh[] holes;
	public KMSelectable[] yourHoles;
	public GameObject[] seedsPlacement;
	public GameObject[] pivot;
	public Renderer ReferenceSeed;

	private int[] seeds = Enumerable.Repeat(7, 16).ToArray();
	private int[] initialState = new int[16];

	private int[] mostSeeds = new int[7];

	private bool turn;
	private bool codeLogging;
	private bool visualLogging;
	private bool collide;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved = false;
	string[] funnyPhrases = {"What are you doing. Stop.",
							 "No, you can't eat those \"seeds\"",
							 "Please, go solve the rest of the bomb; this module is already solved",
							 "What do you want from me? TwT",
							 "If you REALLY are bored, better check THIS out: https://www.youtube.com/watch?v=o-YBDTqX_ZU"};

	void Awake ()
	{
		moduleId = moduleIdCounter++;
		foreach (KMSelectable hole in yourHoles)
		{
			KMSelectable selectedHole = hole;
			hole.OnInteract += delegate () { holeHandler(selectedHole, mostSeeds); return false; };
		}
	}

	void Start ()
	{
		distribute();
		logArray(seeds, "Initial state: ");
		Array.Copy(getTheMost(), mostSeeds, mostSeeds.Length);
		logArray(mostSeeds, "Total points after playing each hole: ");
		/*/Debug.LogFormat("The maximum seeds in a hole is: " +
					mostSeeds.Max() +
				  ", which is in the hole: " +
					(Array.IndexOf(mostSeeds, mostSeeds.Max()) + 1) +
				  ".");/*/
		Debug.LogFormat("[Congkak {0}] The maximum seeds in a hole is: {1}, which is in the hole: {2}.", moduleId, mostSeeds.Max(), ((Array.IndexOf(mostSeeds, mostSeeds.Max())) + 1));
	}

	void logArray (int[] arr, string extraMsg = "")
	{
		string str = "";
		for (int i = 0; i < arr.Length; i++)
			str += arr[i] + " ";

		Debug.LogFormat("[Congkak {0}] {1} {2}", moduleId, extraMsg, str);
	}

	void distribute ()
	{
		seeds[7] = 0;
		seeds[15] = 0;
		int rng;
		int turns = UnityEngine.Random.Range(8, 15 + 1); //range: [x, y[
		turn = turns % 2 == 0 ? false : true;
		for (int j = 0; j <= turns; j++)
		{
			do rng = UnityEngine.Random.Range(getBoundsFromPlayer(turn, false), getBoundsFromPlayer(!turn, true));
			while (seeds[rng] <= 0);

			playAturn(rng, turn);
			turn = !turn;
		}
		Array.Copy(seeds, initialState, seeds.Length);

		//colorSeeds();
		positionSeeds();
	}

    void positionSeeds ()
    {
		float smallRadius = Math.Abs(pivot[0].transform.position.x - pivot[16].transform.position.x);
		float smallHeight = Math.Abs(pivot[0].transform.position.y - pivot[16].transform.position.y);
		//float storeSmallRadius = Math.Abs(pivot[15].transform.position.z - pivot[18].transform.position.z);
		//float storeLargeRadius = Math.Abs(pivot[15].transform.position.x - pivot[17].transform.position.x);
		if (visualLogging) Debug.Log("[Congkak {0}] {1} and {2} give a distance of: {3}", moduleId, pivot[0].name, pivot[16].name, smallRadius);
		float seedDiameter = ReferenceSeed.bounds.size.x*2;
		if (visualLogging) Debug.Log("[Congkak {0}] The diameter of a seed is: {1}", moduleId, seedDiameter);
		for (int k = 0, s = 0; k < 16; k++)
		{
			for (int l = 0; l < seeds[k]; l++, s++)
			{/*/
				if (k == 7 || k == 15)
                {
					Vector3 newPos = (new Vector3(UnityEngine.Random.Range(
												(pivot[k].transform.position.x - (storeLargeRadius - seedDiameter)), //0.003f
												(pivot[k].transform.position.x + (storeLargeRadius - seedDiameter))),
											 0.03f,
											 UnityEngine.Random.Range(
												 (pivot[k].transform.position.z - (storeSmallRadius - seedDiameter)),
												 (pivot[k].transform.position.z + (storeSmallRadius - seedDiameter)))));
					seedsPlacement[s].transform.position = newPos;
					while (!collide)
                    {
						Vector3 newY = new Vector3(seedsPlacement[s].transform.position.x, (seedsPlacement[s].transform.position.y - 0.001f), seedsPlacement[s].transform.position.z);
						seedsPlacement[s].transform.position = newY;
					}
					collide = false;/*/
				//}
                //else
                //{
					Vector3 newPos = (new Vector3(UnityEngine.Random.Range(
												(pivot[k].transform.position.x - (smallRadius - seedDiameter)),
												(pivot[k].transform.position.x + (smallRadius - seedDiameter))),
											 (pivot[0].transform.position.y + seedDiameter/2),
											 UnityEngine.Random.Range(
												 (pivot[k].transform.position.z - (smallRadius - seedDiameter)),
												 (pivot[k].transform.position.z + (smallRadius - seedDiameter)))));
					seedsPlacement[s].transform.position = newPos;
				/*/
						newPos = (new Vector3(seedsPlacement[s].transform.position.x,
											 (pivot[0].transform.position.y + seedDiameter/4) +
												smallHeight*pivot[0].transform.position.x/seedsPlacement[s].transform.position.x,
											 seedsPlacement[s].transform.position.z));
				seedsPlacement[s].transform.position = newPos;
				seedsPlacement[s].transform.position = pivot[0].transform.position;
				seedsPlacement[s].transform.position = new Vector3(UnityEngine.Random.Range(
												(pivot[0].transform.position.x - (smallRadius - seedDiameter / 4)),
												(pivot[0].transform.position.x + (smallRadius - seedDiameter / 4))),
																	0,
																	seedsPlacement[s].transform.position.z);
				newPos = (new Vector3(seedsPlacement[s].transform.position.x,
				((pivot[0].transform.position.y + seedDiameter / 4) +
				Math.Abs(seedsPlacement[s].transform.position.x - pivot[0].transform.position.x)),
				seedsPlacement[s].transform.position.z));
				seedsPlacement[s].transform.position = newPos;

				Debug.Log("Base height: " + (pivot[0].transform.position.y + seedDiameter / 4) +
						  "; subtract seed Xpos " + seedsPlacement[s].transform.position.x +
						  " and pivot Xpos " + pivot[0].transform.position.x + " gives: " +
						  Math.Abs(seedsPlacement[s].transform.position.x - pivot[0].transform.position.x));/*/

				/*/while (!collide)
				{
					Vector3 newY = new Vector3(seedsPlacement[s].transform.position.x, (seedsPlacement[s].transform.position.y - 0.001f), seedsPlacement[s].transform.position.z);
					seedsPlacement[s].transform.position = newY;
				}
				collide = false;/*/
				//}

				//if (visualLogging) Debug.Log("In hole " + (k + 1) + " there is " + pivot[k].name + "; placing seed number " + (s + 1));
			}
		}
	}
	/*/
	void colorSeeds()
    {
		Color32[] colors = new Color32[10] { new Color32(255, 0, 0, 255), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255), new Color32(255, 0, 0, 255), new Color32(255, 255, 0, 255), new Color32(255, 0, 255, 255), new Color32(0, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255), new Color32(128, 128, 128, 255) };
		for (int i = 0; i < 98; i++)
        {
			int n = UnityEngine.Random.Range(0, 10);
			seedsPlacement[i].GetComponent<MeshRenderer>().material.Color32 = colors[n];
        }
		Debug.Log("Done.");
    }

	void OnCollisionEnter(Collision collision)
    {
		collide = true;
    }/*/

    int int2bool (bool originalBool)
	{
		return originalBool ? 1 : 0;
	}

	int getBoundsFromPlayer (bool player, bool upperBound)
	{ //P0 is the human; P1 is the machine; P = player
		if (upperBound) return int2bool(!player) * 8 + 7; // = player's store/main hole
		else return int2bool(player) * 8;
	}

	int[] getTheMost ()
	{
		int[] result = new int[7];

		for (int holeNumber = 0; holeNumber < 7; holeNumber++)
		{
			if (seeds[holeNumber] > 0)
			{
				if (codeLogging) Debug.Log("[Congkak {0}] ----------------- PLAYING HOLE {1}:", moduleId, (holeNumber + 1));
				playAturn(holeNumber, false);
				if (codeLogging) Debug.Log("[Congkak {0}] Total number of seeds: {1} for hole: {2}", moduleId, Enumerable.Sum(seeds), (holeNumber + 1)); //debug
																															  //this should ALWAYS be 98
				getTotalPts(holeNumber, result);
				reset();
			}
		}
		return result;
	}

	void playAturn (int hn, bool turn)
	{
		int[] bounds = { getBoundsFromPlayer(turn, false), getBoundsFromPlayer(turn, true), getBoundsFromPlayer(!turn, false), getBoundsFromPlayer(!turn, true) };
		// = {lower bound from current player, upper bound from current player, lower bound from the opponent, upper bound from the opponent}

		while (!(hn == bounds[1] || seeds[hn] == 1))
		{// When last seed lands on player's store or in an empty hole
			int hold = seeds[hn];
			seeds[hn] = 0;
			for (; hold > 0; hold--)
			{ //Distrubuting seeds
				hn++;
				if (hn == bounds[3]) hn = bounds[2]; //Skipping opponent's store/HoleNumber
				else if (hn >= 16) hn = 0;
				seeds[hn]++;
			}
			if (codeLogging) { Debug.Log("[Congkak {0}] Ended in hole: {1}", moduleId, (hn + 1)); logArray(seeds); }
			if (seeds[hn] == 1 && hn >= bounds[0] && hn < bounds[3])
			{// Accounting for when last seed lands on own empty hole
				seeds[bounds[1]] += seeds[hn] + seeds[14 - hn];
				seeds[hn] = 0;
				seeds[14 - hn] = 0;
				if (codeLogging) { Debug.Log("[Congkak {0}] Capturing hole {1}", moduleId, (14 - hn + 1)); logArray(seeds); }
				break;
			}
		}
	}

	void getTotalPts (int hn, int[] result)
	{
		for (int i = 0; i < 7; i++)
			result[hn] += seeds[i];
		result[hn] += seeds[seeds.Length - 1];
		if (codeLogging) Debug.Log("[Congkak {0}] Points gained if the player played this hole: {1} with value {2}", moduleId, result[hn], initialState[hn]);
	}

	void reset ()
	{
		Array.Copy(initialState, seeds, initialState.Length);
	}

	void holeHandler (KMSelectable hole, int[] ms)
	{
		int maxSeeds = ms.Max();
		if (moduleSolved == false)
		{
			if (ms[hole.name[hole.name.Length - 1] - '0' - 1] == ms.Max())
			{
				moduleSolved = true;
				Debug.LogFormat("[Congkak {0}] You selected the correct hole. Module solved.", moduleId);
				Module.HandlePass();
			}
			else
			{
				Debug.LogFormat("[Congkak {0}] You selected the wrong hole. Strike.", moduleId);
				Module.HandleStrike();
			}
		}
		else
			Debug.LogFormat(funnyPhrases[UnityEngine.Random.Range(0, funnyPhrases.Length)]);
	}
}