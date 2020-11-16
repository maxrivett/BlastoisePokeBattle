using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public GameObject charizardPrefab;
	public GameObject pikachuPrefab;
	public GameObject venusaurPrefab;

	public GameObject returnButton;
	public GameObject combatButtons;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;
	public Text attack1Text;
	public Text attack2Text;
	public Text attack3Text;

	public Text playerHPText;
	public Text enemyHPText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public AudioSource music;
	public AudioSource winEffect;
	public AudioSource hitmarker;

	string moveName;
	int movePower;
	int moveAcc;
	int moveType;
	float STAB;

	int userHealCount;
	int enemyHealCount;

	int opponentChoice = Selection.choice;

    // Start is called before the first frame update
    void Start() {
		music.Play();
		state = BattleState.START;
		StartCoroutine(SetupBattle());
		playerUnit.currentHP = playerUnit.maxHP;
		enemyUnit.currentHP = enemyUnit.maxHP;
		playerHUD.SetHP(playerUnit.currentHP);
		enemyHUD.SetHP(enemyUnit.currentHP);
		userHealCount = 3;
		enemyHealCount = 3;
    }

	IEnumerator SetupBattle() {
		Debug.Log ("" + opponentChoice);
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		
		if (opponentChoice == 1) {
			GameObject enemyGO = Instantiate(charizardPrefab, enemyBattleStation);	
			enemyUnit = enemyGO.GetComponent<Unit>();
		} else if (opponentChoice == 2) {
			GameObject enemyGO = Instantiate(pikachuPrefab, enemyBattleStation);
			enemyUnit = enemyGO.GetComponent<Unit>();
		} else if (opponentChoice == 3) {
			GameObject enemyGO = Instantiate(venusaurPrefab, enemyBattleStation);
			enemyUnit = enemyGO.GetComponent<Unit>();
		} else { //default to pikachu in the case that the program is run from Game scene not selection scene
			GameObject enemyGO = Instantiate(pikachuPrefab, enemyBattleStation);
			enemyUnit = enemyGO.GetComponent<Unit>();
		}


		dialogueText.text = "A wild " + enemyUnit.unitName + " appeared!";
		attack1Text.text = playerUnit.move1 + "";
		attack2Text.text = playerUnit.move2 + "";
		attack3Text.text = playerUnit.move3 + "";

		playerHPText.text = playerUnit.currentHP + "/" + playerUnit.maxHP;
		enemyHPText.text = enemyUnit.currentHP + "/" + enemyUnit.maxHP;

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		combatButtons.SetActive(false);

		yield return new WaitForSeconds(2f);

		if (playerUnit.speed > enemyUnit.speed) {
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		} else {
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());	
		}
	}

	IEnumerator PlayerAttack() {
		combatButtons.SetActive(false);

		bool isDead = false;
		var rand = Random.Range(1, 101);
		if (moveAcc >= rand) {
			hitmarker.Play();
			// bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
			isDead = enemyUnit.TakeDamage(calcDamageOnEnemy());

			enemyHUD.SetHP(enemyUnit.currentHP);
			if (enemyUnit.currentHP < 0) {
				enemyUnit.currentHP = 0;
			}
			dialogueText.text = playerUnit.unitName + " used " + moveName + "!";
			if (userMoveEffectiveness() == 20) { //if move is super effective
				dialogueText.text += "\nIt's super effective!";
			}
			if (userMoveEffectiveness() == 5) { //if move is not very effective
				dialogueText.text += "\nIt's not very effective...";
			}
			playerHPText.text = playerUnit.currentHP + "/" + playerUnit.maxHP;
			enemyHPText.text = enemyUnit.currentHP + "/" + enemyUnit.maxHP;
			yield return new WaitForSeconds(2f);
		} else {
			dialogueText.text = playerUnit.unitName + "'s attack missed!";
			yield return new WaitForSeconds(2f);
		}
		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn() {
		combatButtons.SetActive(false);

		if (enemyUnit.currentHP < enemyUnit.maxHP / 2 && enemyHealCount > 0) { //enemy heals if below half health
			enemyHealCount--;
			int a = enemyUnit.currentHP;
			enemyUnit.Heal(enemyUnit.maxHP / 2); //test
			enemyHUD.SetHP(enemyUnit.currentHP);
			int b = enemyUnit.currentHP - a; 
			dialogueText.text = enemyUnit.unitName + " healed " + b + " HP!";
			playerHPText.text = playerUnit.currentHP + "/" + playerUnit.maxHP;
			enemyHPText.text = enemyUnit.currentHP + "/" + enemyUnit.maxHP;
			yield return new WaitForSeconds(2f);
			state = BattleState.PLAYERTURN;
			PlayerTurn();

		} else { // attacks
			hitmarker.Play();
			dialogueText.text = enemyUnit.unitName + " attacks!";
			// yield return new WaitForSeconds(1f);
			bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
			playerHUD.SetHP(playerUnit.currentHP);
			if (playerUnit.currentHP < 0) {
				playerUnit.currentHP = 0;
			}
			playerHPText.text = playerUnit.currentHP + "/" + playerUnit.maxHP;
			enemyHPText.text = enemyUnit.currentHP + "/" + enemyUnit.maxHP;
			yield return new WaitForSeconds(1f);

			if(isDead) {
				state = BattleState.LOST;
				EndBattle();
			} else {
				state = BattleState.PLAYERTURN;
				PlayerTurn();
			}
		}

	}

	void EndBattle() {

		if(state == BattleState.WON) {
			combatButtons.SetActive(false);
			dialogueText.text = enemyUnit.unitName + " fainted! \nYou won the battle!";
			music.Pause();
			winEffect.Play();
			returnButton.SetActive(true);
			// yield return new WaitForSeconds(1.5f);
			// dialogueText.text = "You won the battle!";

		} else if (state == BattleState.LOST) {
			music.Pause();
			dialogueText.text = "You were defeated.";
			returnButton.SetActive(true);
		}
	}

	void PlayerTurn() {
		dialogueText.text = "Select a move:";
		combatButtons.SetActive(true);
	}

	IEnumerator PlayerHeal() {
		userHealCount--;
		combatButtons.SetActive(false);
		int initHP = playerUnit.currentHP; // hp before the user heals
		playerUnit.Heal(playerUnit.maxHP/2);
		int newHP = playerUnit.currentHP; // hp after the user heals
		int diffHP = newHP - initHP; //how much is healed
		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = playerUnit.unitName + " restored " + diffHP + " HP!";
		playerHPText.text = playerUnit.currentHP + "/" + playerUnit.maxHP;
		enemyHPText.text = enemyUnit.currentHP + "/" + enemyUnit.maxHP;

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttack1Button() {

		if (state != BattleState.PLAYERTURN)
			return;

		moveName = attack1Text.text;
		movePower = playerUnit.move1Power;
		moveAcc = playerUnit.move1Accuracy;
		moveType = playerUnit.move1Type;
		if (playerUnit.move1Type == playerUnit.unitType) {
			STAB = 1.5f;
		} else {
			STAB = 1f;
		}
		StartCoroutine(PlayerAttack());
	}

	public void OnAttack2Button() {

		if (state != BattleState.PLAYERTURN)
			return;

		moveName = attack2Text.text;
		movePower = playerUnit.move2Power;
		moveAcc = playerUnit.move2Accuracy;
		moveType = playerUnit.move2Type;
		if (playerUnit.move2Type == playerUnit.unitType) {
			STAB = 1.5f;
		} else {
			STAB = 1f;
		}
		StartCoroutine(PlayerAttack());
	}

		public void OnAttack3Button() {

		if (state != BattleState.PLAYERTURN)
			return;

		moveName = attack3Text.text;
		movePower = playerUnit.move3Power;
		moveAcc = playerUnit.move3Accuracy;
		moveType = playerUnit.move3Type;
		if (playerUnit.move3Type == playerUnit.unitType) {
			STAB = 1.5f;
		} else {
			STAB = 1f;
		}
		StartCoroutine(PlayerAttack());
	}

	public void OnHealButton() {
		if (state != BattleState.PLAYERTURN) {
			return;
		} 
		if (userHealCount == 0) { 
			dialogueText.text = "No heal potions left!";
			return;
		}

		StartCoroutine(PlayerHeal());
	}

	public int calcDamageOnEnemy() {

		/* Damage calculations
		((2A/5+2)*B*C)/D)/50)+2)*X)*Y/10)*Z)/255

		A = attacker's Level
		B = attacker's Attack or Special
		C = attack Power
		D = defender's Defense or Special
		X = same-Type attack bonus (1 or 1.5)
		Y = Type modifiers (40, 20, 10, 5, 2.5, or 0)  NEED TO DO THIS
		Z = a random number between 217 and 255
		*/

		var number = Random.Range(217, 256);
		int dmg = (int)((((((((2*playerUnit.unitLevel)/5 + 2)* playerUnit.attack * movePower)/enemyUnit.defense)/50 + 2)* STAB)*userMoveEffectiveness())/10 * number) / 255;
		return dmg;
	}

	public int userMoveEffectiveness() {
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

		20 for super effective
		10 for normal
		5 for not very effective
	*/

		if (moveType == 3) {
			if (enemyUnit.unitType == 2) {
				return 20;
			}
			if (enemyUnit.unitType == 4) {
				return 5;
			}
		} else if (moveType == 6) {
			if (enemyUnit.unitType == 2) {
				return 5;
			}
			if (enemyUnit.unitType == 4) {
				return 20;
			}
		} 
		return 10;

	}

	public void returnToMenu() {

		SceneManager.LoadScene(0);

	}

}
