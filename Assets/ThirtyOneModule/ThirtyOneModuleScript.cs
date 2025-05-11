using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class ThirtyOneModuleScript : MonoBehaviour {
	public KMBombModule Module;
   public KMBombInfo Bomb;
   public KMAudio Audio;
   
   public KMSelectable hitButton;
   public KMSelectable standButton;

   public GameObject correctScreen;
   public GameObject wrongScreen;
   public GameObject cardScreen;
   public SolveManager solveManager;

   public CardRenderer transferCard;
   public CardRenderer newCard;
   public CardRenderer oldCard;
   
   public Sprite[] ranks;
   public Sprite[] suits;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;
   private List<List<int>> map = new List<List<int>> {
      new List<int> {10,13,5,7,11,7,1,12,2,2},
      new List<int> {13,13,2,12,6,6,7,13,9,8},
      new List<int> {2,7,5,12,9,1,13,3,1,5},
      new List<int> {7,7,6,11,12,1,6,3,11,3},
      new List<int> {4,10,3,2,6,4,4,1,11,8},
      new List<int> {11,11,13,1,8,12,8,6,13,10},
      new List<int> {3,4,11,9,2,4,12,11,1,2},
      new List<int> {7,4,9,13,11,10,10,8,5,10},
      new List<int> {2,1,3,8,7,9,4,7,8,8},
      new List<int> {6,2,11,6,5,4,5,1,9,3}
      };
   private int total;
   private List<int> currentPosition = new List<int> {-1, -1};
   private int currRank;
   private int currSuit;
   private List<string> directions = new List<string> {"Up", "Right", "Down", "Left"};
   private bool isActive = false;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      hitButton.OnInteract += delegate () {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, hitButton.transform);
         hitButton.AddInteractionPunch();
         StartCoroutine(onHit());
         return false;
         };
      standButton.OnInteract += delegate () {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, standButton.transform);
         standButton.AddInteractionPunch(); 
         onStand();
         return false;
         };
	  Module.OnActivate += delegate() {setup();};
   }

   void setup() {
      oldCard.hideCard();
      showCards();
      currentPosition[0] = Rnd.Range(0,map.Count);
      currentPosition[1] = Rnd.Range(0,map[currentPosition[0]].Count);
      currRank = map[currentPosition[0]][currentPosition[1]];
      total = currRank;
      currSuit = Rnd.Range(0,4);
      Debug.Log("[Thirty One #"+ModuleId+"] First Card's Rank (A=1, J=11, Q=12, K=13): " + currRank);
      Debug.Log("[Thirty One #"+ModuleId+"] First Card's Suit (Spade=0, Heart=1, Club=2, Diamond=3): " + currSuit);
	  isActive = true;
      travelMap();
      StartCoroutine(onHit());
   }

   void travelMap() {
      string direction = directions[currSuit];
      Debug.Log("[Thirty One #"+ModuleId+"] Moving " + direction);
      if (direction == "Up") {
         //The + map.Count is to handle negatives
         currentPosition[0] = (currentPosition[0] - 1 + map.Count) % map.Count;
      }
      else if (direction == "Down") {
         currentPosition[0] = (currentPosition[0] + 1 + map.Count) % map.Count;
      }
      else if (direction == "Left") {
         currentPosition[1] = (currentPosition[1] - 1 + map.Count) % map.Count;
      }
      else if (direction == "Right") {
         currentPosition[1] = (currentPosition[1] + 1 + map.Count) % map.Count;
      }
      else {
         Debug.Log("Uh oh");
      }
      Debug.Log("[Thirty One #"+ModuleId+"] New Position (x,y): (" + currentPosition[1] + "," + currentPosition[0] + ")");
   }

   void showCards() {
      cardScreen.SetActive(true);
      correctScreen.SetActive(false);
      wrongScreen.SetActive(false);
   }
   void updateDirections(int suit) {
      if (suit != -1) {
         Debug.Log("[Thirty One #"+ModuleId+"] Updating Directions based on Suit (Spade=0, Heart=1, Club=2, Diamond=3): " + suit);
      }
       
      if (suit == 0) {
         Debug.Log("[Thirty One #"+ModuleId+"] Rotating the rose clockwise");
         string temp = directions[0];
         for (int i = 1; i < directions.Count; i++) {
            directions[i - 1] = directions[i];
         }
         directions[3] = temp;
      }
      else if (suit == 1) {
         Debug.Log("[Thirty One #"+ModuleId+"] Swapping rose suits going left and down.");
         int left = -1;
         int down = -1;
         for (int i = 0; i < directions.Count; i++) {
            if (directions[i] == "Left") {
               left = i;
            }
            else if (directions[i] == "Down") {
               down = i;
            }
         }
         string temp = directions[left];
         directions[left] = directions[down];
         directions[down] = temp;
      }
      else if (suit == 2) {
         Debug.Log("[Thirty One #"+ModuleId+"] Rotating rose counterclockwise.");
         string temp = directions[3];
         for (int i = directions.Count() - 1; i > 0; i--) {
            directions[i] = directions[i - 1];
         }
         directions[0] = temp;
      }
      else if (suit == 3) {
         Debug.Log("[Thirty One #"+ModuleId+"] Swapping rose suits going up and right.");
         int up = -1;
         int right = -1;
         for (int i = 0; i < directions.Count; i++) {
            if (directions[i] == "Up") {
               up = i;
            }
            else if (directions[i] == "Right") {
               right = i;
            }
         }
         string temp = directions[up];
         directions[up] = directions[right];
         directions[right] = temp;
      }
      else {
         Debug.Log("[Thirty One #"+ModuleId+"] Reseting the compass.");
         directions = new List<string> {"Up", "Right", "Down", "Left"};
      }
      Debug.Log("[Thirty One #"+ModuleId+"] Current Compass (Read out Spades, Hearts, Clubs, then Diamonds.)");
	  foreach (string i in directions) {
         Debug.Log("[Thirty One #"+ModuleId+"] Direction: " + i);
	  }
   }
   void onStand() {
      if (!isActive) {
         return;
      }
      if (total + map[currentPosition[0]][currentPosition[1]] > 31) {
         Debug.Log("[Thirty One #"+ModuleId+"] Correct! As " + total + " + " + map[currentPosition[0]][currentPosition[1]] + " is more than 31.");
         StartCoroutine(finishedSection());
      }
      else {
          Debug.Log("[Thirty One #"+ModuleId+"] Incorrect! As " + total + " + " + map[currentPosition[0]][currentPosition[1]] + " is less than or equal to 31.");
         StartCoroutine(incorrectSection());
      }
   }
   IEnumerator onHit() {
      if (!isActive) {
         yield break;
      }
	  isActive = false;
     Debug.Log("[Thirty One #"+ModuleId+"] New Card's Rank (A=1, J=11, Q=12, K=13): " + map[currentPosition[0]][currentPosition[1]]);
     total += map[currentPosition[0]][currentPosition[1]];
     if (total > 31) {
         Debug.Log("[Thirty One #"+ModuleId+"] Goes over 31! (Total: " + total + ")");
         StartCoroutine(incorrectSection());
         yield break;
      }

     transferCard.gameObject.SetActive(true);
     newCard.hideCard();

     transferCard.updateRank(ranks[currRank - 1]);
     transferCard.updateSuit(suits[currSuit]);
     transferCard.gameObject.transform.localPosition = new Vector3(0.2f, .55f, 0f);
     
	  for (var t = 0f; t < 1; t += Time.deltaTime / 0.2f)
        {
            transferCard.gameObject.transform.localPosition = Vector3.Lerp(new Vector3(0.2f, .55f, 0f), new Vector3(-0.2f, .55f, 0f), t);
            yield return null;
        }
      
      oldCard.updateRank(ranks[currRank - 1]);
      oldCard.updateSuit(suits[currSuit]);
      oldCard.showCard();

      transferCard.gameObject.SetActive(false);

      yield return new WaitForSeconds(0.05f);

      currRank = map[currentPosition[0]][currentPosition[1]];
      currSuit = Rnd.Range(0,4);
      Debug.Log("[Thirty One #"+ModuleId+"] New Card's Suit (Spade=0, Heart=1, Club=2, Diamond=3): " + currSuit);
      newCard.updateRank(ranks[currRank - 1]);
      newCard.updateSuit(suits[currSuit]);
      newCard.showCard();
      Debug.Log("[Thirty One #"+ModuleId+"] Current total: " + total);

      travelMap();
      Debug.Log("[Thirty One #"+ModuleId+"] Next Rank Will Be: " + map[currentPosition[0]][currentPosition[1]]);
      isActive = true;
   }

   IEnumerator finishedSection() {
      //Reminder to connect to counter
      isActive = false;
      cardScreen.SetActive(false);
      correctScreen.SetActive(true);
      wrongScreen.SetActive(false);
	  solveManager.handlePass();
	  if (solveManager.isComplete()) {
         ModuleSolved = true;
			Module.HandlePass();
			yield break;
	  }
      yield return new WaitForSeconds(1f);
      updateDirections(currSuit);
      setup();
   }
   IEnumerator incorrectSection() {
      //Reminder to connect to counter
      isActive = false;
      cardScreen.SetActive(false);
      correctScreen.SetActive(false);
      wrongScreen.SetActive(true);
	  solveManager.handleStrike();
	  Module.HandleStrike();
      yield return new WaitForSeconds(1f);
      updateDirections(-1);
      setup();
   }


#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} hit to press the Hit Button. Use !{0} stand to press the Stand Button.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToLower();
		yield return null;
		if (Command == "hit") {
			hitButton.OnInteract();
			yield break;
		}
      if (Command == "stand") {
			standButton.OnInteract();
			yield break;
		}
		yield return "sendtochaterror I don't understand!";
   }

   IEnumerator TwitchHandleForcedSolve () {
		while (!ModuleSolved) {
         if (isActive) {
			   if (total + map[currentPosition[0]][currentPosition[1]] > 31) {
               onStand();
            }
            else {
               StartCoroutine(onHit());
            }
         }
         yield return new WaitForSeconds(0.1f);
		}
	}
}
