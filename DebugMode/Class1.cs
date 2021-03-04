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
        }

        private void SystemDebug_Awake(ILContext il)
        {
            Collection<Instruction> instructions = il.Body.Instructions;
            il.IL.InsertBefore(instructions[0], il.IL.Create(OpCodes.Call, APIHelpers.FindMethod("SystemDebug", "OnDebuggingEnabled"))); //this.OnDebuggingEnabled();
            il.IL.InsertBefore(instructions[0], il.IL.Create(OpCodes.Ldarg_0)); //idk why this needs to happen but it do.
        }
    }
}
