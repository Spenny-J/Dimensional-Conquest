using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
	public class AI_LIB : MonoBehaviour
	{
		public enum AttackPreference { Closest, Weakest, Strongest }; //Used in attack script
		public enum AttackStyle { UntilDead, OnDamaged, Proximity }; //Used in attack script
		public enum ActionType { Moving, Turning, Waiting }; //Used in the roam script but can be expanded
		public enum Faction { Player, Friendly, Enemy, Neutral }; //Used in ID script and Attack
		public enum Role {  Player, AI, StoryNPC, Companion, LawEnforcement, Bandit }  //Not sure what this is currently used for

	
	}
}
