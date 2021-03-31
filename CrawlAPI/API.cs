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
            MonsterAPI.Init();
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
            //SystemDeity bigDeity = (SystemDeity)Resources.FindObjectsOfTypeAll(typeof(SystemDeity)).FirstOrDefault();
            On.SystemMain.Awake += (a, b) =>
            {
                foreach (Deity i in SystemDeity.GetDeities())
                {
                    //Console.WriteLine(i);
                }
                CustomMonster monst = new CustomMonster();
                monst.SetToDefaults();
                MonsterAPI.AddMonster(monst);
            };
            /*Deity[] deities = (Deity[])Resources.FindObjectsOfTypeAll(typeof(UnityEngine.));
            foreach (Player i in monsters)
            {
                //Console.WriteLine(i.name);
                if (i.name == "Hero")
                {
                    APIHelpers.PrintPlayer(i);
                }
            }*/
            /*[] weapons = (Weapon[])Resources.FindObjectsOfTypeAll(typeof(Weapon));
            foreach (Weapon i in array)
            {
                Console.WriteLine(i.name);
            }*/
            //stream.Close();
        }
    }

    
    
    public class WeaponAPI //used to add weapons. will work better probably.
    {

    }
    public class ItemAPI //items(not weapons, like the passive ones, ill look into this more)
    {

    }
}