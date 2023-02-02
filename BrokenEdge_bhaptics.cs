using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;

[assembly: MelonInfo(typeof(BrokenEdge_bhaptics.BrokenEdge_bhaptics), "BrokenEdge_bhaptics", "2.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("TREBUCHET", "Broken Edge")]

namespace BrokenEdge_bhaptics
{
    public class BrokenEdge_bhaptics : MelonMod
    {
        
        public static TactsuitVR tactsuitVr = null!;
        public static bool twoHanded = false;
        public static bool isRightHand = true;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }
        
        [HarmonyPatch(typeof(Il2CppPlayer.Items.Sword.SwordController), "ProcessSwordHit", new Type[] { typeof(Il2CppGameState.RoundManager.SwordHitEvent) })]
        public class bhaptics_SwordHit
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppPlayer.Items.Sword.SwordController __instance)
            {
                if (!__instance.Archetype.IsLocal) return;
                isRightHand = (__instance.State == Il2CppPlayer.Items.ItemBase.ItemState.InRightHand);
                //twoHanded = (__instance.IsHeldWithTwoHands);
                float intensity = __instance.TipSpeed.magnitude / 8.0f;
                tactsuitVr.SwordRecoil(isRightHand, twoHanded, intensity);
            }
        }

        [HarmonyPatch(typeof(Il2CppPlayer.Items.Sword.SwordController), "BreakSword", new Type[] {  })]
        public class bhaptics_BreakSword
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppPlayer.Items.Sword.SwordController __instance)
            {
                if (!__instance.Archetype.IsLocal) return;
                twoHanded = (__instance.IsHeldWithTwoHands);
                isRightHand = (__instance.State == Il2CppPlayer.Items.ItemBase.ItemState.InRightHand);
                tactsuitVr.SwordRecoil(isRightHand, twoHanded, 1.0f);
                tactsuitVr.PlaybackHaptics("BrokenSword");
                tactsuitVr.StartHeartBeat();
            }
        }

        [HarmonyPatch(typeof(Il2CppPlayer.Items.Sword.SwordController), "ResetSwords", new Type[] { })]
        public class bhaptics_ResetSwords
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppPlayer.Items.Sword.SwordController __instance)
            {
                tactsuitVr.StopHeartBeat();
            }
        }
        
        [HarmonyPatch(typeof(Il2CppPlayer.Items.Sword.SwordController), "OnDefeatAnimationStarted", new Type[] { typeof(Il2CppGameState.DefeatManager.DefeatAnimationInfo) })]
        public class bhaptics_PlayerDefeat
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppPlayer.Items.Sword.SwordController __instance, Il2CppGameState.DefeatManager.DefeatAnimationInfo info)
            {
                if (!__instance.Archetype.IsLocal) return;
                if (info.LosingPlayer != __instance.Archetype.PlayerNumber)
                {
                    //twoHanded = (__instance.IsHeldWithTwoHands);
                    isRightHand = (__instance.State == Il2CppPlayer.Items.ItemBase.ItemState.InRightHand);
                    tactsuitVr.SwordRecoil(isRightHand, twoHanded, 1.0f);
                }
                else tactsuitVr.PlaybackHaptics("SlashDefault");
            }
        }

        [HarmonyPatch(typeof(Il2CppPlayer.Items.Shield.ShieldController), "DisableShieldParts", new Type[] { typeof(string) })]
        public class bhaptics_ShieldHit
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppPlayer.Items.Shield.ShieldController __instance)
            {
                if (!__instance.Archetype.IsLocal) return;
                twoHanded = false;
                isRightHand = (__instance.State == Il2CppPlayer.Items.ItemBase.ItemState.InRightHand);
                tactsuitVr.SwordRecoil(isRightHand, twoHanded, 1.0f);
            }
        }

    }
}
