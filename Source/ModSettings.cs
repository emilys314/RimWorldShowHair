namespace ShowHair
{
    public class ShowHairSettings : Verse.ModSettings
    {
        public bool showUnderUpperHeadGear;
        public bool showUnderFullHeadGear;
        public bool scaleHeadGear;
        public float scaleHeadGearAmount;
        public bool scaleHair;
        public float scaleHairAmount;

        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            Verse.Scribe_Values.Look(ref showUnderUpperHeadGear, "showUnderUpperHeadGear", true);
            Verse.Scribe_Values.Look(ref showUnderFullHeadGear, "showUnderFullHeadGear", true);
            Verse.Scribe_Values.Look(ref scaleHeadGear, "scaleHeadGear", true);
            Verse.Scribe_Values.Look(ref scaleHeadGearAmount, "scaleHeadGearAmount", 1.05f);
            Verse.Scribe_Values.Look(ref scaleHair, "scaleHair", true);
            Verse.Scribe_Values.Look(ref scaleHairAmount, "scaleHairAmount", 0.95f);
            base.ExposeData();
        }
    }

    public class ShowHairMod : Verse.Mod
    {
        /// <summary>
        /// A reference to our settings.
        /// </summary>
        ShowHairSettings settings;

        /// <summary>
        /// A mandatory constructor which resolves the reference to our settings.
        /// </summary>
        /// <param name="content"></param>
        public ShowHairMod(Verse.ModContentPack content) : base(content)
        {
            this.settings = GetSettings<ShowHairSettings>();
        }

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            Verse.Listing_Standard listingStandard = new Verse.Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled(
                "Show hair under hats?",
                ref settings.showUnderUpperHeadGear,
                "Should we show hair under 'Upper Headgear' types? (Helmets, hats, etc)"
            );
            listingStandard.CheckboxLabeled(
                "Show hair and beards under helmets?",
                ref settings.showUnderFullHeadGear,
                "Should we show hair, special eyes, and beards under 'Full Headgear' types? (Marine helment, tribal mask, etc)"
            );

            listingStandard.CheckboxLabeled(
                settings.scaleHeadGear ? $"Scale up headgear? [{settings.scaleHeadGearAmount}]" : "Scale up headgear?",
                ref settings.scaleHeadGear,
                "Scale headgear to help cover any hair underneath? Note: you need to re-equip items"
            );
            if (settings.scaleHeadGear)
            {
                settings.scaleHeadGearAmount = Verse.Widgets.HorizontalSlider(
                        listingStandard.GetRect(30f),
                        settings.scaleHeadGearAmount,
                        1.0f,
                        1.2f,
                        false,
                        null,
                        null,
                        null,
                        0.01f
                    );
            }

            // listingStandard.CheckboxLabeled(
            //     settings.scaleHair ? $"Scale down hair? [{settings.scaleHairAmount}]" : "Scale down hair?",
            //     ref settings.scaleHair,
            //     "Scale hair to prevent it poking out of any hats on top?"
            // );
            // if (settings.scaleHair)
            // {
            //     settings.scaleHairAmount = Verse.Widgets.HorizontalSlider(
            //             listingStandard.GetRect(30f),
            //             settings.scaleHairAmount,
            //             0.8f,
            //             1.0f,
            //             false,
            //             null,
            //             null,
            //             null,
            //             0.01f
            //         );
            // }
            listingStandard.End();

            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "Show Hair Under Stuff";
        }
    }
}