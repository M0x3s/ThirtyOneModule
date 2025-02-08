using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class ThirtyOneModule : MonoBehaviour {
   public KMBombInfo Bomb;
   public KMAudio Audio;
   
   public KMSelectable hitButton;
   public KMSelectable standButton;

   public GameObject correctScreen;
   public GameObject wrongScreen;
   public GameObject cardScreen;

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
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */
      Debug.Log("Start!");
      hitButton.OnInteract += delegate () {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, hitButton.transform);
         hitButton.AddInteractionPunch();
         onHit();
         return false;
         };
      standButton.OnInteract += delegate () {
         Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, standButton.transform);
         standButton.AddInteractionPunch(); 
         onStand();
         return false;
         };
      setup();
   }
   void reset() {
      total = 0;
      setup();
   }
   void setup() {
      showCards();
      isActive = true;
      currentPosition[0] = UnityEngine.Random.Range(0,map.Count);
      currentPosition[1] = UnityEngine.Random.Range(0,map[currentPosition[0]].Count);
      currRank = map[currentPosition[0]][currentPosition[1]];
      total = currRank;
      currSuit = UnityEngine.Random.Range(0,4);
      travelMap();
      onHit();
   }

   void onHit() {
      if (!isActive) {
         return;
      }
      oldCard.updateRank(ranks[currRank - 1]);
      oldCard.updateSuit(suits[currSuit]);
      Debug.Log(currentPosition[0]);
      Debug.Log(currentPosition[1]);
      currRank = map[currentPosition[0]][currentPosition[1]];
      currSuit = UnityEngine.Random.Range(0,4);
      total += currRank;
      if (total > 31) {
         StartCoroutine(incorrectSection());
         return;
      }
      newCard.updateRank(ranks[currRank - 1]);
      newCard.updateSuit(suits[currSuit]);
      travelMap();
      Debug.Log(total);
   }
   void onStand() {
      if (!isActive) {
         return;
      }
      if (total + map[currentPosition[0]][currentPosition[1]] > 31) {
         StartCoroutine(finishedSection());
      }
      else {
         StartCoroutine(incorrectSection());
      }
   }

   void travelMap() {
      string direction = directions[currSuit];
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
   }

   void showCards() {
      cardScreen.SetActive(true);
      correctScreen.SetActive(false);
      wrongScreen.SetActive(false);
   }
   void updateDirections(int suit) {
      if (suit == 0) {
         string temp = directions[3];
         for (int i = 1; i < directions.Count; i++) {
            directions[i] = directions[i - 1];
         }
         directions[0] = temp;  
      }
      else if (suit == 1) {
         string temp = directions[1];
         directions[1] = directions[3];
         directions[3] = temp;
      }
      else if (suit == 2) {
         string temp = directions[0];
         for (int i = 1; i < directions.Count; i++) {
            directions[i - 1] = directions[i];
         }
         directions[3] = temp;
      }
      else if (suit == 3) {
         string temp = directions[0];
         directions[0] = directions[2];
         directions[2] = temp;
      }
      else {
         directions = new List<string> {"Up", "Right", "Down", "Left"};
      }
   }
   IEnumerator finishedSection() {
      //Reminder to connect to counter
      isActive = false;
      cardScreen.SetActive(false);
      correctScreen.SetActive(true);
      wrongScreen.SetActive(false);
      yield return new WaitForSeconds(1f);
      updateDirections(currSuit);
      reset();
   }
   IEnumerator incorrectSection() {
      //Reminder to connect to counter
      isActive = false;
      cardScreen.SetActive(false);
      correctScreen.SetActive(false);
      wrongScreen.SetActive(true);
      yield return new WaitForSeconds(1f);
      updateDirections(-1);
      reset();
   }


#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
