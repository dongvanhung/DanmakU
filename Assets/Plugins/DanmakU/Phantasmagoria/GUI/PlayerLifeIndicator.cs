﻿// Copyright (c) 2015 James Liu
//	
// See the LISCENSE file for copying permission.

using UnityEngine;
using System.Collections;
using UnityUtilLib.GUI;
using DanmakU.Phantasmagoria;

namespace DanmakU.Phantasmagoria.GUI {

	public class PlayerLifeIndicator : MultiObjectValueIndicator {

		private PhantasmagoriaGameController gameControl;

		void Awake() {
			gameControl = (PhantasmagoriaGameController)GameController;
		}

		protected override int GetMaxValue () {
			return DanmakuGameController.MaximumLives;
		}

		protected override int GetValue () {
			return ((player) ? gameControl.player1 : gameControl.player2).Field.Player.LivesRemaining;
		}
	}
}