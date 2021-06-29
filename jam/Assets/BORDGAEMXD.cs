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

	private int[] seeds = new int[16];
	private int[] initialState = new int[16];
	//private int limit = 10;
	private int totalNumSeeds = 98;
	private int[] mostSeeds = new int[7];

	private bool myTurn = true;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake() {
		moduleId = moduleIdCounter++;
	}

	// Use this for initialization
	void Start() {
		distribute(totalNumSeeds);
		Array.Copy(getTheMost(), mostSeeds, 7);
		Debug.Log("Seeds: " +
					mostSeeds.Max() +
			      "in position: " +
				    Array.IndexOf(mostSeeds, mostSeeds.Max()) +
				  ".");
	}

	//Update is called once per frame
	void Update()
	{

	}

	void distribute(int tns)
	{
		/*/for (int i = 0; i < seeds.Length; i++)
		{
			seeds[i] = UnityEngine.Random.Range(0, tns + 1);
			tns -= seeds[i];
		}
		seeds.Shuffle();/*/
		/*/for (int lmao, lmao < seeds.Length; lmao++)//lmao
		  initialState[lmao] = seeds[lmao];/*/
		for (int i = 0; i < seeds.Length; i++) { seeds[i] = 7; }
		seeds[7] = 0;
		seeds[15] = 0;
		int turns = UnityEngine.Random.Range(8, 15);
		for (int j = 0; j <= turns; j++)
		{
			if (myTurn == true)
			{
				int rng = UnityEngine.Random.Range(0, 7);
				playAturn(rng);
				myTurn = false;
			}
			else
			{
				int rng = UnityEngine.Random.Range(8, 15);
				playAturn(rng);
				myTurn = true;
			}
		}
		Array.Copy(seeds, initialState, 7);
		for (int k = 0; k < seeds.Length; k++)
        {
			holes[k].text = seeds[k].ToString();
        }

	}

	int[] getTheMost() {
		int[] result = new int[7];

		for (int holeNumber = 0; holeNumber < 7; holeNumber++)
		{
			playAturn(holeNumber);
			getTotalPts(holeNumber, result);
			reset();
		}
		return result;
	}

	void playAturn(int hn) {
		if (myTurn = true)
        {
			while (!(hn == 7 || seeds[hn] == 1 || seeds[hn] == 0))
			{// When last seed lands on player's store or in an empty hole, the last condition is for the if part
				int hold = seeds[hn];
				seeds[hn] = 0;
				for (; hold > 0; hn++, hold--)
				{//Distrubuting seeds xdxdxd what. <- the first argument is useless bc we already have declared the variable hold
					if (hn == 15) hn = 0; //Skipping opponent's store/HoleNumber
					seeds[hn]++;
				}
				if (seeds[hn] == 1 && hn >= 0 && hn <= 6)
				{// Accounting for when last seed lands on own empty hole
					seeds[7] += seeds[hn] + seeds[14 - hn];
					seeds[hn] = 0;
					seeds[14 - hn] = 0;
				}
			}
		}
        else
        {
			while (!(hn == 15 || seeds[hn] == 1 || seeds[hn] == 0))
			{// When last seed lands on player's store or in an empty hole, the last condition is for the if part
				int hold = seeds[hn];
				seeds[hn] = 0;
				for (; hold > 0; hn++, hold--)
				{//Distrubuting seeds xdxdxd what. <- the first argument is useless bc we already have declared the variable hold
					if (hn == 7) hn = 8; //Skipping opponent's store/HoleNumber
					seeds[hn]++;
				}
				if (seeds[hn] == 1 && hn >= 8 && hn <= 14)
				{// Accounting for when last seed lands on own empty hole
					seeds[15] += seeds[hn] + seeds[14 - hn];
					seeds[hn] = 0;
					seeds[14 - hn] = 0;
				}
				hold = seeds[hn];// Holds the current number of seeds in that current hole
				seeds[hn] = 0;
			}
		}
		
	}

	void getTotalPts(int hn, int[] result){
		for (int h = 0; h < 7; h++)
			result[hn] += seeds[h];
	}

	void reset()
	{
		/*/
		for (int j; j < seeds.Length; j++) //Resets the board
		  seeds[j] = initialState[j];/*/
		Array.Copy(initialState, seeds, 7);
	}
}
