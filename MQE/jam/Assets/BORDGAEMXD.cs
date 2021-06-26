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

	public KMSelectable[] holes;
	public Mesh[] seedsPos;

	private int[] seeds = new int[16];
	//private int limit = 10;
	private int totalNumSeeds = 98;

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
		/*/
		foreach (number in seeds)
		  number = Unity.Random(0, totalNumSeeds+1);/*/
		distribute();
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void distribute ()
	{
		UnityEngine.Random.Range(0, 98);
		seeds.Shuffle();
    }
}
