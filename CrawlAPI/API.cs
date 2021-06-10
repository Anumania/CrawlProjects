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
            //exAtlas[] array = (exAtlas[])Resources.FindObjectsOfTypeAll(typeof(exAtlas));
            APIHelpers.Init();
            MonsterAPI.Init();
            DeityAPI.Init();
            IFormatter form = new BinaryFormatter();
            On.MenuMain.Update += (a, b) => { Debug.developerConsoleVisible = false; }; //in debug mode, this dev console sucks

            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));

            GameObject noConsole = new GameObject();
            noConsole.AddComponent<NoConsole>();
            GameObject.DontDestroyOnLoad(noConsole);

            SystemDebug.AddDebugItem("I - Player", "change to debug char", new Action(this.changeToTestChar));

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
           
                
            /*for (int i = 0; i < 10; i++)
            {
                CustomDeity testDeity = new CustomDeity(); //done at main menu start or null reference crash?
                testDeity.SetToDefaults();
                testDeity.m_name = "test deity!";
                testDeity.m_text = "test deity! ";
                AddDeity(testDeity);
            }*/
            Console.WriteLine(SystemDeity.GetTrialDeity(0));


            //exAtlas modAtlas = new exAtlas();
            try
            {
                exAtlas modAtlas = ScriptableObject.CreateInstance<exAtlas>();
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


                CustomDeity testman = new CustomDeity();
                testman.SetToDefaults();
                testman.m_text = "im test";
                testman.m_textFlavour = "im also test";
                DeityAPI.AddDeity(testman);
                DeityAPI.AddDeity(testman);
                DeityAPI.AddDeity(testman);

                    
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
            On.MenuDeitySelectPlayer.Start += MenuDeitySelectPlayer_Start;
            IL.MenuDeitySelectPlayer.Update += IL_MenuDeitySelectPlayer_Update;
            On.MenuDeitySelectPlayer.Update += MenuDeitySelectPlayer_Update;
        }

        private void MenuDeitySelectPlayer_Update(On.MenuDeitySelectPlayer.orig_Update orig, MenuDeitySelectPlayer self)
        {

            orig(self);
            //throw new NotImplementedException();
        }

        private void IL_MenuDeitySelectPlayer_Update(ILContext il)
        {
            il.IL.Body.Instructions[32].OpCode = Mono.Cecil.Cil.OpCodes.Ldc_I4_2; //make trials display as the second to last option, not the last, because custom deities display as last.
        }

        private void MenuDeitySelectPlayer_Start(On.MenuDeitySelectPlayer.orig_Start orig, MenuDeitySelectPlayer self)
        {
            self.gameObject.AddComponent<MenuDeitySelectPlayerExtension>(); // catch custom mod deity related messages.
            orig(self);
            FieldInfo menu = self.GetType().GetField("m_menu", BindingFlags.Instance | BindingFlags.NonPublic); //add a new button that shows mod deities
            MethodInfo addItemToMenu = typeof(MenuTextMenu).GetMethod("AddItem", BindingFlags.Public | BindingFlags.Instance);
            MenuTextMenuItemData modDeitiesList = new MenuTextMenuItemData
            {
                m_enabled = true,
                m_prefabMenuItem = (MenuTextMenuItem)self.GetType().GetField("m_prefabMenuItemTrial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self),
                m_message = "MsgOnSelectModDeities",
            };
            addItemToMenu.Invoke(menu.GetValue(self), new object[] { modDeitiesList });
        }
        public void changeToTestChar() //debug command TODO:only add once
        {
            
            GameObject debugMonst = SystemDeity.GetDeity(0).GetStartingMonster(0);
            SystemPlayers.GetPlayer(0).OnGameObjectAssigned(Instantiate(debugMonst),false,true,true);
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
    public class MenuDeitySelectPlayerExtension : MonoBehaviour //adding additional methods to MenuDeitySelectPlayer
    {
        public GameObject trialsMenu;
        public GameObject trialsText;
        public void Awake()
        {
           
        }
        public void MsgOnSelectModDeities()
        {
            Console.WriteLine("Deity Mod Menu selected");
            
            base.SendMessageUpwards("MsgQueueState", "DeityModdedSelect");

        }
    }
}
