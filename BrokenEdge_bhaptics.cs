using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

namespace BrokenEdge_bhaptics
{
    public class BrokenEdge_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool twoHanded = false;
        public static bool isRightHand = true;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(Player.Items.Sword.SwordController), "ProcessSwordHit", new Type[] { typeof(GameState.RoundManager.SwordHitEvent) })]
        public class bhaptics_SwordHit
        {
            [HarmonyPostfix]
            public static void Postfix(Player.Items.Sword.SwordController __instance)
            {
                twoHanded = (__instance.IsTwoHanded);
                isRightHand = (__instance.CurrentHand.IsRightHand);
                float intensity = __instance.TipSpeed.magnitude / 8.0f;
                //tactsuitVr.LOG("Sword speed: " + intensity);
                tactsuitVr.SwordRecoil(isRightHand, twoHanded, intensity);
            }
        }

        [HarmonyPatch(typeof(Player.Items.Sword.SwordController), "BreakSword", new Type[] {  })]
        public class bhaptics_BreakSword
        {
            [HarmonyPostfix]
            public static void Postfix(Player.Items.Sword.SwordController __instance)
            {
                twoHanded = (__instance.IsTwoHanded);
                isRightHand = (__instance.CurrentHand.IsRightHand);
                tactsuitVr.SwordRecoil(isRightHand, twoHanded, 1.0f);
                tactsuitVr.PlaybackHaptics("BrokenSword");
                tactsuitVr.StartHeartBeat();
            }
        }

        [HarmonyPatch(typeof(Player.Items.Sword.SwordController), "ResetSwords", new Type[] { })]
        public class bhaptics_ResetSwords
        {
            [HarmonyPostfix]
            public static void Postfix(Player.Items.Sword.SwordController __instance)
            {
                tactsuitVr.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(Player.Items.Sword.SwordController), "OnDefeatAnimationStarted", new Type[] { typeof(GameState.DefeatManager.DefeatAnimationInfo) })]
        public class bhaptics_PlayerDefeat
        {
            [HarmonyPostfix]
            public static void Postfix(Player.Items.Sword.SwordController __instance, GameState.DefeatManager.DefeatAnimationInfo info)
            {
                if (info.LosingPlayer != __instance.Archetype.PlayerNumber) return;
                tactsuitVr.PlaybackHaptics("SlashDefault");
            }
        }


    }
}
