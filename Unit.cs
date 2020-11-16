using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	/*
		 TYPE TO INT!

		NORMAL ---> 1
		FIRE -----> 2
		WATER ----> 3
		GRASS ----> 4
		ELECTRIC -> 5
		ICE ------> 6
		FIGHTING -> 7
		POISON ---> 8
		GROUND ---> 9
		FLYING ---> 10
		PSYCHIC --> 11
		BUG ------> 12
		ROCK -----> 13
		GHOST ----> 14
		DRAGON ---> 15
		DARK -----> 16
		STEEL ----> 17
	*/



	public string unitName;
	public int unitLevel;
	public int unitType;
	public int damage;
	public int maxHP;
	public int currentHP;
	public int attack;
	public int defense;
	public int speed;

	public string move1;
	public int move1Power;
	public int move1Accuracy;
	public int move1Type;
	public int move1Priority;

	public string move2;
	public int move2Power;
	public int move2Accuracy;
	public int move2Type;
	public int move2Priority;

	public string move3;
	public int move3Power;
	public int move3Accuracy;
	public int move3Type;
	public int move3Priority;

	public string move4;
	public int move4Power;
	public int move4Accuracy;
	public int move4Type;
	public int move4Priority;


	public bool TakeDamage(int dmg) {

		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount) {

		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}
