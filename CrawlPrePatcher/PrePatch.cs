using Mono.Cecil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CrawlPrePatcher
{
    public static class CrawlAPI
    {
        static List<string> UnityFunctions = new List<string>(){ "Update", "Awake", "Initialize", "FixedUpdate", "Start", "OnGUI", "OnEnable", "OnDisable", "OnDestroy" };
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        // Patches the assemblies
        public static void Patch(AssemblyDefinition assembly)
        {
            Collection<TypeDefinition> baseTypes = new Collection<TypeDefinition>();
            MakeSavesLocal(assembly);
            Collection<TypeDefinition> col = assembly.MainModule.Types;
            foreach(TypeDefinition def in col)
            {
                if (def.Namespace == "")
                {
                    baseTypes.Add(def);
                    foreach (MethodDefinition methdef in def.Methods)
                    {
                        methdef.IsPrivate = false;
                    }
                    foreach (FieldDefinition fielddef in def.Fields)
                    {
                        fielddef.IsPrivate = false;
                    }
                }
            }

            /*foreach(TypeDefinition def in baseTypes)
            {
                TypeDefinition typeDef = new TypeDefinition("Crawl", def.Name, def.Attributes, def);
                assembly.MainModule.Types.Add(typeDef);
            }*/
        }

        private static void makePublic(AssemblyDefinition ass,string _type, string _method)
        {
            MethodDefinition mthd = ass.MainModule.GetType(_type).Methods.Where(e => e.Name == _method).FirstOrDefault();
            mthd.IsPublic = true;
        }

        private static void MakeSavesLocal(AssemblyDefinition ass) //this makes the game save locally instead of syncing with steam.
        {
            MethodDefinition mthd = ass.MainModule.GetType("SaveHelper").NestedTypes.Where(e => e.Name.Contains("CoroutineStartSave")).FirstOrDefault().Methods.First(); //enumerators suck to edit.
            ILProcessor il = mthd.Body.GetILProcessor();
            MethodReference fileStratRef = ass.MainModule.GetType("SaveStrategyFile").Methods.Where(a => a.Name == ".ctor").FirstOrDefault();
            MethodReference steamStratRef = ass.MainModule.GetType("SaveStrategySteam").Methods.Where(a => a.Name == ".ctor").FirstOrDefault();
            Collection<MethodDefinition> bruh = ass.MainModule.GetType("SaveHelper").NestedTypes.Where(e => e.Name.Contains("CoroutineStartSave")).FirstOrDefault().Methods;
            foreach(var inst in bruh[3].Body.Instructions)
            {
                if(inst.OpCode == OpCodes.Newobj && steamStratRef != null && (MethodReference)inst.Operand == steamStratRef) //check to make sure user doesnt already have no steam
                {
                    inst.Operand = fileStratRef; //make the game use file strategy instead of steam strategy.
                    break;
                }
            }
        }
	}
}
