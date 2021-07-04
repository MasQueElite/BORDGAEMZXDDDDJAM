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

	private int[] seeds = Enumerable.Repeat(7, 16).ToArray();
	private int[] initialState = new int[16];

	private int[] mostSeeds = new int[7];

	private bool turn;
	private bool logging;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved = false;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		foreach (KMSelectable hole in yourHoles)
		{
			KMSelectable selectedHole = hole;
			hole.OnInteract += delegate () { holeHandler(hole); return false; };
		}
	}

	// Use this for initialization
	void Start()
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
	void Update()
	{

	}

	void logArray(int[] arr, string extraMsg = "")
	{
		string str = "";
		for (int i = 0; i < arr.Length; i++)
			str += arr[i] + " ";

		Debug.Log(extraMsg + str);
	}

	void distribute()
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
		for (int k = 0; k < pivot.Length; k++)
		{
			for (int l = 0; l < seeds[k]; l++)
			{
				Vector3 newPos = (new Vector3(UnityEngine.Random.Range((pivot[k].transform.position.x - 0.003f), (pivot[k].transform.position.x + 0.003f)), 0.09f, UnityEngine.Random.Range((pivot[k].transform.position.z - 0.003f), (pivot[k].transform.position.z + 0.003f))));
				seedsPlacement[l].transform.position = newPos;
			}

		}
	}

	int int2bool(bool originalBool)
	{
		return originalBool ? 1 : 0;
	}

	int getBoundsFromPlayer(bool player, bool upperBound)
	{ //P0 is the human; P1 is the machine; P = player
		if (upperBound) return int2bool(!player) * 8 + 7; // = player's store/main hole
		else return int2bool(player) * 8;
	}

	int[] getTheMost()
	{
		int[] result = new int[7];

		for (int holeNumber = 0; holeNumber < 7; holeNumber++)
		{
			if (seeds[holeNumber] > 0)
			{
				if (logging) Debug.Log("----------------- PLAYING HOLE " + (holeNumber + 1) + ":");
				playAturn(holeNumber, false);
				if (logging) Debug.Log("Total number of seeds: " + Enumerable.Sum(seeds) + " for hole: " + (holeNumber + 1)); //debug
																															  //this should ALWAYS be 98
				getTotalPts(holeNumber, result);
				reset();
			}
		}
		return result;
	}

	void playAturn(int hn, bool turn)
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
			if (logging) { Debug.Log("Ended in hole: " + (hn + 1)); logArray(seeds); }
			if (seeds[hn] == 1 && hn >= bounds[0] && hn < bounds[3])
			{// Accounting for when last seed lands on own empty hole
				seeds[bounds[1]] += seeds[hn] + seeds[14 - hn];
				seeds[hn] = 0;
				seeds[14 - hn] = 0;
				if (logging) { Debug.Log("Capturing hole " + (14 - hn + 1)); logArray(seeds); }
				break;
			}
		}
	}

	void getTotalPts(int hn, int[] result)
	{
		for (int i = 0; i < 7; i++)
			result[hn] += seeds[i];
		result[hn] += seeds[seeds.Length - 1];
		if (logging) Debug.Log("Points gained if the player played this hole: " + result[hn] + " with value: " + initialState[hn]);
	}

	void reset()
	{
		Array.Copy(initialState, seeds, initialState.Length);
	}

	void holeHandler(KMSelectable hole)
	{
		if (moduleSolved == false)
		{
			if (hole == yourHoles[mostSeeds.Max() + 1]) //Enter most seeds hole here
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
	}
}