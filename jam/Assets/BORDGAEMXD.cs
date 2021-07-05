using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using UnityEngine.EventSystems;
using NUnit.Framework.Internal.Commands;

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
	private bool visualLogging = true;
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

	// Use this for initialization
	void Start ()
	{
		distribute();
		logArray(seeds, "Initial state: ");
		Array.Copy(getTheMost(), mostSeeds, mostSeeds.Length);
		logArray(mostSeeds, "Total points after playing each hole: ");
		Debug.Log("Seeds: " +
					mostSeeds.Max() +
				  " in hole: " +
					(Array.IndexOf(mostSeeds, mostSeeds.Max()) + 1) +
				  ".");
	}

	//Update is called once per frame
	void Update ()
	{

	}

	void logArray (int[] arr, string extraMsg = "")
	{
		string str = "";
		for (int i = 0; i < arr.Length; i++)
			str += arr[i] + " ";

		Debug.Log(extraMsg + str);
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

		colorSeeds();
		positionSeeds();
	}

    void positionSeeds ()
    {
		float smallRadius = Math.Abs(pivot[0].transform.position.x - pivot[16].transform.position.x);
		float storeSmallRadius = Math.Abs(pivot[15].transform.position.z - pivot[18].transform.position.z);
		float storeLargeRadius = Math.Abs(pivot[15].transform.position.x - pivot[17].transform.position.x);
		if (visualLogging) Debug.Log(pivot[0].name + " and " + pivot[pivot.Length - 3].name + " give a distance of: " + smallRadius);
		float seedDiameter = ReferenceSeed.bounds.size.x*2;
		if (visualLogging) Debug.Log("The diameter of a seed is: " + seedDiameter);
		for (int k = 0, s = 0; k < pivot.Length-3; k++)
		{
			for (int l = 0; l < seeds[k]; l++, s++)
			{
				if (k == 7 || k == 15)
                {
					Vector3 newPos = (new Vector3(UnityEngine.Random.Range(
												(pivot[k].transform.position.x - (storeLargeRadius - seedDiameter)), //0.003f
												(pivot[k].transform.position.x + (storeLargeRadius - seedDiameter))),
											 0.05f,
											 UnityEngine.Random.Range(
												 (pivot[k].transform.position.z - (storeSmallRadius - seedDiameter)),
												 (pivot[k].transform.position.z + (storeSmallRadius - seedDiameter)))));
					while (!collide)
                    {
						Vector3 newY = new Vector3(seedsPlacement[s].transform.position.x, (seedsPlacement[s].transform.position.y - 0.001f), seedsPlacement[s].transform.position.z);
						seedsPlacement[s].transform.position = newY;
					}
					collide = false;
				}
                else
                {
					Vector3 newPos = (new Vector3(UnityEngine.Random.Range(
												(pivot[k].transform.position.x - (smallRadius - seedDiameter)), //0.003f
												(pivot[k].transform.position.x + (smallRadius - seedDiameter))),
											 0.05f,
											 UnityEngine.Random.Range(
												 (pivot[k].transform.position.z - (smallRadius - seedDiameter)),
												 (pivot[k].transform.position.z + (smallRadius - seedDiameter)))));
					seedsPlacement[s].transform.position = newPos;
					while (!collide)
					{
						Vector3 newY = new Vector3(seedsPlacement[s].transform.position.x, (seedsPlacement[s].transform.position.y - 0.001f), seedsPlacement[s].transform.position.z);
						seedsPlacement[s].transform.position = newY;
					}
					collide = false;
				}
				
				if (visualLogging) Debug.Log("In hole " + (k + 1) + " there is " + pivot[k].name + "; placing seed number " + (s + 1));
			}

		}
	}

	void colorSeeds()
    {
		Debug.Log("Done. All are white.");
    }

	void OnCollisionEnter(Collision collision)
    {
		collide = true;
    }
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
				if (codeLogging) Debug.Log("----------------- PLAYING HOLE " + (holeNumber + 1) + ":");
				playAturn(holeNumber, false);
				if (codeLogging) Debug.Log("Total number of seeds: " + Enumerable.Sum(seeds) + " for hole: " + (holeNumber + 1)); //debug
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
			if (codeLogging) { Debug.Log("Ended in hole: " + (hn + 1)); logArray(seeds); }
			if (seeds[hn] == 1 && hn >= bounds[0] && hn < bounds[3])
			{// Accounting for when last seed lands on own empty hole
				seeds[bounds[1]] += seeds[hn] + seeds[14 - hn];
				seeds[hn] = 0;
				seeds[14 - hn] = 0;
				if (codeLogging) { Debug.Log("Capturing hole " + (14 - hn + 1)); logArray(seeds); }
				break;
			}
		}
	}

	void getTotalPts (int hn, int[] result)
	{
		for (int i = 0; i < 7; i++)
			result[hn] += seeds[i];
		result[hn] += seeds[seeds.Length - 1];
		if (codeLogging) Debug.Log("Points gained if the player played this hole: " + result[hn] + " with value: " + initialState[hn]);
	}

	void reset ()
	{
		Array.Copy(initialState, seeds, initialState.Length);
	}

	void holeHandler (KMSelectable hole, int[] ms)
	{
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		if (moduleSolved == false)
		{
			if (hole.name[hole.name.Length - 1] - '0' == Array.IndexOf(ms, ms.Max()) + 1) //Enter most seeds hole here
			{                         //That is mostSeeds.Max()) + 1 --- in line 50
				moduleSolved = true;
				Debug.Log("You selected the correct hole. Module solved.");
				Module.HandlePass();
			}
			else
			{
				Debug.Log("You selected the wrong hole. Strike.");
				Module.HandleStrike();
			}
		}
		else
			Debug.Log(funnyPhrases[UnityEngine.Random.Range(0, funnyPhrases.Length)]);
	}
}