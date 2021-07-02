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
	public Mesh[] seedsPos;

	private int[] seeds = Enumerable.Repeat(7, 16).ToArray();
	private int[] initialState = new int[16];

	private int[] mostSeeds = new int[7];

	private bool turn = true;
	private int intedBool;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake ()
	{
		moduleId = moduleIdCounter++;
	}

	// Use this for initialization
	void Start ()
	{
		distribute();
		Array.Copy(getTheMost(), mostSeeds, mostSeeds.Length);
		Debug.Log("Seeds: " +
					mostSeeds.Max() +
				  "in position: " +
					Array.IndexOf(mostSeeds, mostSeeds.Max()) +
				  ".");
	}

	//Update is called once per frame
	void Update ()
	{

	}

	void distribute ()
	{
		seeds[7] = 0;
		seeds[15] = 0;
		int turns = UnityEngine.Random.Range(8, 15 + 1);
		for (int j = 0; j <= turns; j++)
		{
			int rng = UnityEngine.Random.Range(getBoundsFromPlayer(turn, false), getBoundsFromPlayer(turn, true));
			playAturn(rng, turn);
			turn = !turn;
		}
		Array.Copy(seeds, initialState, seeds.Length);
		for (int k = 0; k < seeds.Length; k++)
			holes[k].text = seeds[k].ToString();
	}

	int int2bool (bool originalBool)
	{
		return originalBool ? 1 : 0;
	}

	int getBoundsFromPlayer (bool player, bool upperBound)
	{ //P0 is the human; P1 is the machine; P = player
		if (upperBound) return int2bool(player) * 8 + 7; // = player's store/main hole
		else return int2bool(player) * 8;
	}

	int[] getTheMost ()
	{
		int[] result = new int[7];

		for (int holeNumber = 0; holeNumber < 7; holeNumber++)
		{
			playAturn(holeNumber, false);
			getTotalPts(holeNumber, result);
			reset();
		}
		return result;
	}

	void playAturn (int hn, bool turn)
	{
		int[] bounds = { getBoundsFromPlayer(turn, false), getBoundsFromPlayer(turn, true), getBoundsFromPlayer(!turn, true) };
			// = {lower bound from current player, upper bound from current player, upper bound from the opponent} --- I don't need the lower bound from the opponent

		while (!(hn == bounds[1] || seeds[hn] == 1 || seeds[hn] == 0))
		{// When last seed lands on player's store or in an empty hole, the last condition is for the if part
			int hold = seeds[hn];
			seeds[hn] = 0;
			for (; hold > 0; hold--)
			{ //Distrubuting seeds
				hn++;
				if (hn == bounds[2]) hn = bounds[0]; //Skipping opponent's store/HoleNumber
				else if (hn >= 16) hn = 0;
				seeds[hn]++;
			}
			if (seeds[hn] == 1 && hn >= bounds[0] && hn < bounds[1])
			{// Accounting for when last seed lands on own empty hole
				seeds[bounds[1]] += seeds[hn] + seeds[14 - hn];
				seeds[hn] = 0;
				seeds[14 - hn] = 0;
			}
		}
	}

	void getTotalPts (int hn, int[] result)
	{
		foreach (int hole in seeds)
			result[hn] += hole;
	}

	void reset ()
	{
		Array.Copy(initialState, seeds, 7);
	}
}
