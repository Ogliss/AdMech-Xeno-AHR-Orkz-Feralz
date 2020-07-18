using FeralOrkz.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace FeralOrkz
{

	public sealed class FeralOrkzMod : Mod
	{

		private Vector2 scrollPosition = Vector2.zero;
		private float scrollViewHeight = 0f;
		public FeralOrkzSettings settings;


		public FeralOrkzMod(ModContentPack mcp) : base(mcp)
		{
			LongEventHandler.ExecuteWhenFinished(GetSettings);
			this.settings = GetSettings<FeralOrkzSettings>();
		}


		public void GetSettings()
		{
			GetSettings<FeralOrkzSettings>();
		}


		public override void WriteSettings()
		{
			base.WriteSettings();
		}

		public override void DoSettingsWindowContents(Rect rect)
		{
			Listing_Standard list = new Listing_Standard()
			{
				ColumnWidth = rect.width
			};

			Rect topRect = rect.TopPart(0.25f);
			Rect bottomRect = rect.BottomPart(0.75f);
			Rect fullRect = list.GetRect(Text.LineHeight).ContractedBy(4);
			Rect leftRect = fullRect.LeftHalf().Rounded();
			Rect rightRect = fullRect.RightHalf().Rounded();

			list.Begin(rect);

			Listing_Standard listing_FungalLabel = list.BeginSection(fullRect.height/10);
			listing_FungalLabel.ColumnWidth *= 0.488f;
			listing_FungalLabel.TextFieldNumericLabeled<float>("AMO_FungusOptions".Translate(), ref settings.FungusSpawnChance, ref settings.FungusSpawnChanceBuffer, 0f, 1f, "AMO_FungusOptionsToolTip".Translate(), 0.75f, 0.25f);
			listing_FungalLabel.NewColumn();
			listing_FungalLabel.TextFieldNumericLabeled<float>("AMO_CocoonOptions".Translate(), ref settings.CocoonSpawnChance, ref settings.CocoonSpawnChanceBuffer, 0f, 1f, "AMO_CocoonOptionsToolTip".Translate(), 0.75f, 0.25f);
			list.EndSection(listing_FungalLabel);

			Listing_Standard listing_Fungus = list.BeginSection(fullRect.height - (fullRect.height / 10));
			listing_Fungus.ColumnWidth *= 0.488f;
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Squig".Translate(), ref settings.FungusSquigChance, ref settings.FungusSquigChanceBuffer, 0f, 1f, "AMO_SquigToolTip".Translate(), 0.75f, 0.25f);
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Snot".Translate(), ref settings.FungusSnotChance, ref settings.FungusSnotChanceBuffer, 0f, 1f, "AMO_SnotToolTip".Translate(), 0.75f, 0.25f);
			//    listing_Fungus.NewColumn();
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Grot".Translate(), ref settings.FungusGrotChance, ref settings.FungusGrotChanceBuffer, 0f, 1f, "AMO_GrotToolTip".Translate(), 0.75f, 0.25f);
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Ork".Translate(), ref settings.FungusOrkChance, ref settings.FungusOrkChanceBuffer, 0f, 1f, "AMO_OrkToolTip".Translate(), 0.75f, 0.25f);
			listing_Fungus.NewColumn();
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Squig".Translate(), ref settings.CocoonSquigChance, ref settings.CocoonSquigChanceBuffer, 0f, 1f, "AMO_SquigToolTip".Translate(), 0.75f, 0.25f);
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Snot".Translate(), ref settings.CocoonSnotChance, ref settings.CocoonSnotChanceBuffer, 0f, 1f, "AMO_SnotToolTip".Translate(), 0.75f, 0.25f);
			//    listing_Fungus.NewColumn();
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Grot".Translate(), ref settings.CocoonGrotChance, ref settings.CocoonGrotChanceBuffer, 0f, 1f, "AMO_GrotToolTip".Translate(), 0.75f, 0.25f);
			listing_Fungus.TextFieldNumericLabeled<float>("AMO_Ork".Translate(), ref settings.CocoonOrkChance, ref settings.CocoonOrkChanceBuffer, 0f, 1f, "AMO_OrkToolTip".Translate(), 0.75f, 0.25f);

			list.EndSection(listing_Fungus);

		}
	}
}
