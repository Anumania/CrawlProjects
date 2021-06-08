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
using System.Reflection.Emit;


namespace CrawlAPI
{
    [BepInPlugin("org.anumania.plugins.crawlapi", "CrawlAPI", "1.0.0.0")]
    public class CrawlAPI : BaseUnityPlugin
    {
        void Awake()
        {
            Console.WriteLine("CrawlAPI Started!");
            //DynamicMethod testMethod = new DynamicMethod("testMethod", typeof(void), null);
            //exAtlas[] array = (exAtlas[])Resources.FindObjectsOfTypeAll(typeof(exAtlas));
            APIHelpers.Init();
            MonsterAPI.Init();
            //GameObject a = new GameObject();
            //a.AddComponent<SystemDebug>();
            //GameObject.DontDestroyOnLoad(a);
            //Console.WriteLine(SystemDebug.Enabled);
            IFormatter form = new BinaryFormatter();
            On.MenuMain.Update += (a, b) => { Debug.developerConsoleVisible = false; }; //in debug mode, this dev console sucks

            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));

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
                Console.WriteLine("ueah");
                GameObject noConsole = new GameObject();
                noConsole.AddComponent<NoConsole>();
                GameObject.DontDestroyOnLoad(noConsole);

                SystemDebug.AddDebugItem("I - Player", "change to debug char", new Action(this.changeToTestChar));
                
                //exAtlas modAtlas = new exAtlas();
                try
                {
                    CustomDeity testDeity = new CustomDeity();
                    testDeity.SetToDefaults();
                    //Console.WriteLine(testDeity.ToString());
                    AddDeity(testDeity);

                    exAtlas modAtlas = ScriptableObject.CreateInstance<exAtlas>();
                    //DontDestroyOnLoad(modAtlas);
                    Texture2D modAtlasTexture = new Texture2D(2048, 2048);
                    byte[] array2 = File.ReadAllBytes("./Crawl_Textures/bruh.png");
                    modAtlasTexture.LoadImage(array2);
                    Console.WriteLine("loaded!");
                    modAtlas.name = "modAtlasTexture";

                    
                    modAtlas.texture = modAtlasTexture;
                    modAtlas.elements = new exAtlas.Element[1];
                    modAtlas.elements[0] = new exAtlas.Element();
                    modAtlas.elements[0].name = "test";
                    modAtlas.elements[0].coords = new Rect(0, 0, 4, 4);
                    modAtlas.elements[0].originalHeight = 64;
                    modAtlas.elements[0].originalWidth = 64;
                    modAtlas.elements[0].trimRect = new Rect(0, 0, 31, 31);
                   

                    
                    foreach (Deity i in SystemDeity.GetDeities())
                    {
                        //Console.WriteLine(i);
                    }
                    CustomMonster monst = new CustomMonster();
                    monst.SetToDefaults();
                    Material matt = monst.m_animationIdle.frameInfos[0].atlas.material;
                    Material matthew = new Material(matt);
                    matthew.mainTexture = modAtlas.texture;



                    foreach (exAtlas mat in Resources.FindObjectsOfTypeAll(typeof(exAtlas)))
                    {
                        if (mat.name == "AtlasMonsters")
                        {
                            
                            //mat.SetTexture(name)
                            
                            //Console.WriteLine("bruh");
                            //monst.m_animationIdle.frameInfos[0].atlas.material = mat;
                            //mat.mainTexture = modAtlasTexture;
                        }
                    }
                    monst.m_animationIdle.frameInfos[0].atlas = modAtlas;
                    monst.m_animationIdle.frameInfos[0].index = 0;
                    monst.m_animationIdle.frameInfos[0].atlas.material = matthew;

                    monst.m_animationIdle.frameInfos[0].atlas.texture = modAtlasTexture;

                    MonsterAPI.AddMonster(monst);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            };
 
            On.SystemDebug.OnGUI += (a, b) =>
            {
                BotHero obj = FindObjectOfType<BotHero>();
                if (obj != null)
                {
                    //GUI.Label(new Rect(0, 0, 100, 20), "yeah");
                }
                else
                {
                    //GUI.Label(new Rect(0, 0, 100, 20), "nah");
                }
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
        public void changeToTestChar() //debug command TODO:only add once
        {
            
            GameObject debugMonst = SystemDeity.GetDeity(0).GetStartingMonster(0);
            SystemPlayers.GetPlayer(0).OnGameObjectAssigned(Instantiate(debugMonst),false,true,true);
        }
        public void AddDeity(CustomDeity customDeity)
        {
            Deity deity = new Deity();
            foreach (var field in typeof(CustomDeity).GetFields())
            {
                FieldInfo fldInfo = typeof(Deity).GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fldInfo == null)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        fldInfo.SetValue(deity, field.GetValue(customDeity));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        fldInfo.SetValue(deity, null);
                    }
                }
            }

            FieldInfo deityListRef = typeof(SystemDeity).GetField("m_deities", BindingFlags.NonPublic|BindingFlags.Instance);
            List<Deity> deityList = (List<Deity>)deityListRef.GetValue(SystemDeity.Instance);
            deityList.Add(deity);
            deityListRef.SetValue(SystemDeity.Instance, deityList);
        }
    }

    

    public class WeaponAPI //used to add weapons. will work better probably.
    {

    }
    public class ItemAPI //items(not weapons, like the passive ones, ill look into this more)
    {

    }
    public class NoConsole : MonoBehaviour //in my setup, an annoying console keeps popping up and i cant see the screen at all, this should fix
    {
        void OnGUI()
        {
            Debug.developerConsoleVisible = false;
        }
    }
}