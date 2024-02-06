using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using UnityEngine;

namespace ExampleShowTamed
{
    [BepInPlugin("Fentiman.SoulsStamina", "Souls Stamina", "1.0.0")]
    [BepInProcess("valheim.exe")]
    public class SoulsStamina : BaseUnityPlugin // Class extends BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("Fentiman.SoulsStamina");

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Player))]
        static class PlayerPatch
        {
            [HarmonyTranspiler]
            [HarmonyPatch(nameof(Player.UpdateDodge))]
            static IEnumerable<CodeInstruction> UpdateDodgeTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(
                        useEnd: false,
                        new CodeMatch(OpCodes.Ldloc_2),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Character), nameof(Character.HaveStamina))))
                    .ThrowIfInvalid("Could not patch Player.UpdateDodge()! (HaveStamina)")
                    .Advance(offset: 1)
                    .InsertAndAdvance(Transpilers.EmitDelegate<Func<float, float>>(HaveStaminaDelegate))
                    .InstructionEnumeration();

            }

            static float HaveStaminaDelegate(float value)
            {
                // Conditional logic here if needed, otherwise just return 0f.
                return 0f;
            }
        }
    }
}