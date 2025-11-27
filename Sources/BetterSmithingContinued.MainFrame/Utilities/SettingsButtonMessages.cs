using System;
using System.Collections.Generic;
using BetterSmithingContinued.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Utilities
{
	public static class SettingsButtonMessages
	{
		public static void DisplayNextSettingsButtonMessage()
		{
			string text = SettingsButtonMessages.m_Messages[SettingsButtonMessages.m_CurrentMessageIndex];
			if (SettingsButtonMessages.m_CurrentMessageIndex == SettingsButtonMessages.m_Messages.Length - 1)
			{
				InformationManager.DisplayMessage(new InformationMessage(text));
				return;
			}
			if (text == "Beep")
			{
				SettingsButtonMessages.m_BeepsRemaining--;
				if (SettingsButtonMessages.m_BeepsRemaining > 0)
				{
					InformationManager.DisplayMessage(new InformationMessage(text));
					return;
				}
			}
			else if (text == "ExecuteLord")
			{
				Hero spouse = MobileParty.MainParty.LeaderHero.Spouse;
				string text2 = (spouse != null) ? spouse.Name.ToString() : null;
				if (text2 != null && !SettingsButtonMessages.m_ExecutedHeroes.Contains(text2))
				{
					text = "Oh geez, was that your Spouse? Bummer.";
					SettingsButtonMessages.m_CurrentMessageIndex--;
				}
				else
				{
					Hero hero;
					do
					{
						hero = Campaign.Current.AliveHeroes[SettingsButtonMessages.rand.Next(0, Campaign.Current.AliveHeroes.Count - 1)];
						text2 = ((hero != null) ? hero.Name.ToString() : null);
					}
					while (text2 == null || SettingsButtonMessages.m_ExecutedHeroes.Contains(text2) || hero.IsDead || hero == Hero.MainHero);
					text = SettingsButtonMessages.m_ExecuteLordMessage[SettingsButtonMessages.m_CurrentExecuteLordMessageIndex];
					if (SettingsButtonMessages.m_CurrentExecuteLordMessageIndex != 3)
					{
						text += text2;
					}
					SettingsButtonMessages.m_CurrentExecuteLordMessageIndex++;
				}
				SettingsButtonMessages.m_ExecutedHeroes.Add(text2);
				GameTexts.SetVariable("PARTNAME", text2);
				MBInformationManager.AddQuickInformation(
					TextObjectUtilities.CreateTextObject<string>("{=p9F90bc0}{PARTNAME} has been Executed.", null),
					3,
					null,
                    null,
                    "event:/ui/notification/quest_fail"
				);
			}
			else
			{
				if (text == "GiveMoney")
				{
					MBTextManager.SetTextVariable("GOLD_AMOUNT", Math.Abs(1));
					InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_you_received_gold_with_icon", null).ToString(), "event:/ui/notification/coins_positive"));
					SettingsButtonMessages.m_CurrentMessageIndex++;
					return;
				}
				if (text == "LoseGold")
				{
					InformationManager.DisplayMessage(new InformationMessage(SettingsButtonMessages.m_LoseGoldMessages[SettingsButtonMessages.m_CurrentLoseGoldMessageIndex], "event:/ui/notification/coins_negative"));
					SettingsButtonMessages.m_CurrentLoseGoldMessageIndex++;
					SettingsButtonMessages.m_CurrentMessageIndex++;
					return;
				}
			}
			SettingsButtonMessages.m_CurrentMessageIndex++;
			if (SettingsButtonMessages.m_Messages[SettingsButtonMessages.m_CurrentMessageIndex] == "Beep")
			{
				SettingsButtonMessages.m_BeepsRemaining = SettingsButtonMessages.rand.Next(8, 25);
			}
			InformationManager.DisplayMessage(new InformationMessage(text));
		}

		private static List<string> m_ExecutedHeroes = new List<string>();

		private static int m_CurrentMessageIndex;

		private static int m_CurrentExecuteLordMessageIndex;

		private static int m_BeepsRemaining;

		private static string[] m_Messages = new string[]
		{
			"You pressed the settings button.",
			"You pressed it again!",
			"You really like this button.",
			"Now you're just pressing it to see if I'll say something interesting.",
			"Well, maybe eventually.",
			"But probably not.",
			"Do you think maybe if you press it enough...",
			"Eventually the settings menu will actually open?",
			"What if though-",
			"-the guy who programmed this is a bad person",
			"And every time you press the button,",
			"You're losing gold?",
			"LoseGold",
			"LoseGold",
			"LoseGold",
			"Oops, forgot to add some zeros to those previous amounts.",
			"Your total net loss is now -8,878,517 Gold.",
			"You went to check didn't you?",
			"Come on now, I wouldn't touch your gold dude. That's sacred.",
			"I wouldn't be opposed to killing random lords though...",
			"ExecuteLord",
			"ExecuteLord",
			"Ahahaha!",
			"ExecuteLord",
			"ExecuteLord",
			"No but for real though, thank you for downloading my mod :)",
			"Don't worry, no lords actually died during the making of this mod.",
			"If you enjoy it, please endorse it. The more endorsements it gets, the more people will see it",
			"which encourages me to keep investing my free time adding on to it.",
			"All future messages will now just be beeps",
			"Beep",
			"Oh hey, still there?",
			"Beep",
			"No really, there's nothing else.",
			"Beep",
			"Fo real my boy. You've either got way too much time to spare, or your expectations of me are way too high",
			"Beep",
			"Maybe if you keep going, I really will execute a random lord?",
			"Beep",
			"Or maybe I'll grant you incredible wealth?",
			"Beep",
			"GiveMoney",
			"There you go, now don't spend it all in one place!",
			"Beep",
			"The worst part is, I didn't actually give you any gold...",
			"Beep",
			"Better Smithing Continued Settings (Coming Soon... Maybe)"
		};

		private static int m_CurrentLoseGoldMessageIndex;

		private static string[] m_LoseGoldMessages = new string[]
		{
			"Op, there goes 36 Gold.",
			"Another 12 Gold.",
			"Au revoir 40 Gold."
		};

		private static string[] m_ExecuteLordMessage = new string[]
		{
			"Bye bye ",
			"So long ",
			"Ciao ",
			"Oh wait, I actually kinda liked that one."
		};

		private static Random rand = new Random();
	}
}
