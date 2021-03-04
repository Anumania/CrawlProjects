using System;
using BepInEx;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Mono.Collections.Generic;
using Mono.Collections;
using MonoMod.Utils;
using Mono.Cecil;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace CrawlAPI
{
    [BepInPlugin("org.anumania.plugins.crawlapi", "CrawlAPI", "1.0.0.0")]
    public class CrawlAPI : BaseUnityPlugin
    {
        void Awake()
        {
            APIHelpers.Init();
            //GameObject a = new GameObject();
            //a.AddComponent<SystemDebug>();
            //GameObject.DontDestroyOnLoad(a);
            //Console.WriteLine(SystemDebug.Enabled);
            IFormatter form = new BinaryFormatter();
            On.MenuMain.Update += (a, b) => { Debug.developerConsoleVisible = false; }; //in debug mode, this dev console sucks

            UnityEngine.Object[] objects = (UnityEngine.Object[])Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));

            List<Type> objList = new List<Type>();
            foreach (UnityEngine.Object i in objects)
            {
                if (!objList.Contains(i.GetType()))
                {
                    objList.Add(i.GetType());
                    //Console.WriteLine(i.GetType());
                }
            }
            Player[] monsters = (Player[])Resources.FindObjectsOfTypeAll(typeof(Player));
            foreach (Player i in monsters)
            {
                if(i.name == "Hero")
                {
                    foreach( var j in i.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        if(j.GetCustomAttributes(true).Where(e=>e.GetType().Name == "SerializeField").FirstOrDefault() != null)
                        {
                            Console.WriteLine(j.Name + ":" + j.GetValue(i));
                        }
                    }
                }
            }
            /*[] weapons = (Weapon[])Resources.FindObjectsOfTypeAll(typeof(Weapon));
            foreach (Weapon i in array)
            {
                Console.WriteLine(i.name);
            }*/
            //stream.Close();
        }
    }
    public static class MonsterAPI //used to add monsters. probably not gonna work well, replacing is probably better.
    {
        public class EvolveCostOverride
        {
            public GameObject m_evolveTo;
            public float price;
        }
    }
    public class CustomMonster
    {
        public CustomMonster()
        {

        }
        //Type
        public bool m_isHero = false;
        public bool m_isGhost = false;
        //XP
        public MinMaxRange m_xpOnKill = new MinMaxRange(3f, 4f);
        //Evolve
        public float m_evolveCost = 1f;
        public Player[] m_evolveTo; //fill
        public MonsterAPI.EvolveCostOverride[] m_evolveToCostOverride; //fill
        //Visuals
        public exSpriteAnimClip m_animationSpawn; //fill
        public exSpriteAnimClip m_animationIdle; //fill
        public exSpriteAnimClip m_animationRun; //fill
        public exSpriteAnimClip m_animationDead; //fill
        public exSpriteAnimClip m_animationKnockback; //fill
        public exSpriteAnimClip m_animationStatue; //fill 
        public PlayerShadow m_shadowPrefab; //fill

    }
    public class WeaponAPI //used to add weapons. will work better probably.
    {

    }
    public class ItemAPI //items(not weapons, like the passive ones, ill look into this more)
    {

    }
    public static class APIHelpers
    {
        public static AssemblyDefinition AssemblyCS; //unity c#
        public static void Init()
        {
            AssemblyCS = AssemblyDefinition.ReadAssembly(BepInEx.Paths.ManagedPath + "\\Assembly-CSharp.dll");
        }
        public static MethodDefinition FindMethod(AssemblyDefinition ass, string type, string method)
        {
            return null;
        }
        public static MethodDefinition FindMethod(string type, string method)
        {
            return AssemblyCS.MainModule.GetType(type).Methods.Where(e => { return e.Name == method; }).First();
        }
    }
}