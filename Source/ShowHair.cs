using System.Collections.Generic;

namespace ShowHair
{
    [Verse.StaticConstructorOnStartup]
    public static class Patch
    {
        static ShowHairSettings settings = Verse.LoadedModManager.GetMod<ShowHairMod>().GetSettings<ShowHairSettings>();

        static Patch()
        {
            var original = typeof(Verse.PawnRenderTree).GetMethod(
                "AdjustParms",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            var post = typeof(ShowHair.Patch).GetMethod(
                "PostAdjustParms",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
            );
            new HarmonyLib.Harmony("RaccoonCuddler.ShowHair").Patch(
                original,
                postfix: new HarmonyLib.HarmonyMethod(post)
            );

            new HarmonyLib.Harmony("RaccoonCuddler.ShowHair").Patch(
                typeof(Verse.PawnRenderTree).GetMethod(
                    "ProcessApparel",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                ),
                postfix: new HarmonyLib.HarmonyMethod(
                    typeof(ShowHair.Patch).GetMethod(
                        "PostProcessApparel",
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                    )
                )
            );
        }

        public static void PostAdjustParms(Verse.PawnRenderTree __instance, ref Verse.PawnDrawParms parms)
        {
            var _pawn = __instance.pawn;
            if (_pawn.apparel != null && Verse.PawnRenderNodeWorker_Apparel_Head.HeadgearVisible(parms))
            {
                foreach (RimWorld.Apparel item in _pawn.apparel.WornApparel)
                {
                    if (item.def.apparel.bodyPartGroups.Contains(RimWorld.BodyPartGroupDefOf.UpperHead) && settings.showUnderUpperHeadGear)
                    {
                        parms.skipFlags &= ~RimWorld.RenderSkipFlagDefOf.Hair;
                    }
                    if (item.def.apparel.bodyPartGroups.Contains(RimWorld.BodyPartGroupDefOf.FullHead) && settings.showUnderFullHeadGear)
                    {
                        parms.skipFlags &= ~RimWorld.RenderSkipFlagDefOf.Hair;
                        parms.skipFlags &= ~RimWorld.RenderSkipFlagDefOf.Beard;
                        parms.skipFlags &= ~RimWorld.RenderSkipFlagDefOf.Eyes;
                    }
                }
            }
        }

        public static void ScaleHair(Verse.PawnRenderNode node)
        {
            if (node == null)
                return;

            if (node is Verse.PawnRenderNode_Hair)
            {
                node.Props.drawSize = new UnityEngine.Vector2(
                    settings.scaleHairAmount,
                    settings.scaleHairAmount
                );
            }

            if (node.children != null)
            {

                foreach (Verse.PawnRenderNode child in node.children)
                {
                    ScaleHair(child);
                }
            }

        }

        public static void PostProcessApparel(
            Verse.PawnRenderTree __instance,
            Dictionary<Verse.PawnRenderNodeTagDef, Verse.PawnRenderNode> ___nodesByTag,
            ref RimWorld.Apparel ap,
            ref Verse.PawnRenderNode headApparelNode)

        {
            try
            {
                bool isWearingHat = false;

                if (ap.def.apparel.parentTagDef != null)
                {
                    if (ap.def.apparel.bodyPartGroups.Contains(RimWorld.BodyPartGroupDefOf.UpperHead)
                        || ap.def.apparel.bodyPartGroups.Contains(RimWorld.BodyPartGroupDefOf.FullHead))
                    {
                        if (settings.scaleHeadGear)
                        {
                            // headApparelNode.debugScale = 2.0f;
                            headApparelNode.Props.drawSize = new UnityEngine.Vector2(
                                settings.scaleHeadGearAmount,
                                settings.scaleHeadGearAmount
                            );
                        }
                        isWearingHat = true;
                    }
                }

                // if (isWearingHat && settings.scaleHair)
                // {
                //     ___nodesByTag.TryGetValue(RimWorld.PawnRenderNodeTagDefOf.Head, out var value);
                //     ScaleHair(value);
                // }
            }
            catch (System.Exception)
            {
                Verse.Log.Error($"[ShowHair] Exception processing apparel for {ap.def.defName}");
            }
        }
    }

}