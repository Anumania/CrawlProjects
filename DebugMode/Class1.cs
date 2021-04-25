using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrawlAPI;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace DebugMode
{
    [BepInPlugin("org.anumania.plugins.debugmode", "DebugMode", "1.0.0.0")]
    public class DebugMode : BaseUnityPlugin
    {
        void Awake()
        {
            APIHelpers.Init();
            //GameObject a = new GameObject();
            //a.AddComponent<SystemDebug>();
            //GameObject.DontDestroyOnLoad(a);
            //Console.WriteLine(SystemDebug.Enabled);
            IL.SystemDebug.Awake += SystemDebug_Awake;
            On.SystemLevel.Start += SystemLevel_Start;
            IL.SystemDebug.Update += SystemDebug_Update;
        }

        private void SystemDebug_Update(ILContext il)
        {
            il.IL.Body.Instructions[42].Operand = 92;
        }

        private void SystemLevel_Start(On.SystemLevel.orig_Start orig, SystemLevel self)
        {
            orig(self);
            //SystemDebug.AddDebugItem("I-Player","Add gold", new Action(this.AddGold));
        }

        private void SystemDebug_Awake(ILContext il)
        {
            Collection<Instruction> instructions = il.Body.Instructions;
            il.IL.InsertBefore(instructions[0], il.IL.Create(OpCodes.Call, APIHelpers.FindMethod("SystemDebug", "OnDebuggingEnabled"))); //this.OnDebuggingEnabled();
            il.IL.InsertBefore(instructions[0], il.IL.Create(OpCodes.Ldarg_0)); //idk why this needs to happen but it do
            
        }

        private void AddGold()
        {
            var ary = UnityEngine.GameObject.FindObjectsOfType<Player>();
            foreach (Player p in ary)
            {
                if (p != null)
                {
                    if (p.GetIsHero())
                    {
                        p.GetPlayerData().StatsGame.SetStat(PlayerStats.eStat.GoldEarned, 10000000);
                    }
                }
            }
        }
    }
}
