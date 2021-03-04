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
            Player[] monsters = (Player[])Resources.FindObjectsOfTypeAll(typeof(Player));
            foreach (Player i in monsters)
            {
                //Console.WriteLine(i.name);
                if(i.name == "Hero")
                {
                    APIHelpers.PrintPlayer(i);   
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
        public static Player[] monsters;
        public static void Init()
        {
            monsters = (Player[])Resources.FindObjectsOfTypeAll(typeof(Player));
        }

        public static Player GetMonster(string monsterName) //this does not work with mod added monsters!
        {
            return monsters.Where(e => e.name == "EnemySkeletonPirate").FirstOrDefault();
        }

        public class EvolveCostOverride
        {
            public EvolveCostOverride(GameObject evolveTo, float _price = 0)
            {
                price = _price;
                m_evolveTo = evolveTo;
            }
            public GameObject m_evolveTo;
            public float price;
        }
        
    }
    public class CustomMonster
    {
        public CustomMonster()
        {

        }

        public void SetToDefaults()
        {
            Player hero = MonsterAPI.GetMonster("Hero");
            m_evolveTo = new Player[] { MonsterAPI.GetMonster("EnemySkeletonPirate"), MonsterAPI.GetMonster("EnemyGnome")};
            m_evolveToCostOverride = new MonsterAPI.EvolveCostOverride[]{
                new MonsterAPI.EvolveCostOverride(MonsterAPI.GetMonster("EnemySkeletonPirate").gameObject),
                new MonsterAPI.EvolveCostOverride(MonsterAPI.GetMonster("EnemyGnome").gameObject)
                };
            m_animationIdle = (exSpriteAnimClip)hero.GetType().GetField("m_animationIdle").GetValue(hero);
            m_animationRun = (exSpriteAnimClip)hero.GetType().GetField("m_animationRun").GetValue(hero);
            m_animationDead = (exSpriteAnimClip)hero.GetType().GetField("m_animationDead").GetValue(hero);
            m_animationKnockback = (exSpriteAnimClip)hero.GetType().GetField("m_animationKnockback").GetValue(hero);
            m_animationStatue = (exSpriteAnimClip)hero.GetType().GetField("m_animationStatue").GetValue(hero);
            m_shadowPrefab = (PlayerShadow)hero.GetType().GetField("m_shadowPrefab").GetValue(hero);
            m_spawnOnHit = (DamageEffectData[])hero.GetType().GetField("m_spawnOnHit").GetValue(hero);
            m_spawnOnDeath = (DamageEffectData[])hero.GetType().GetField("m_spawnOnDeath").GetValue(hero);

            m_attackOrderType = (Attacks.eAttackOrderType)hero.GetType().GetField("m_attackOrderType").GetValue(hero);
            m_comboResetTime = (float)hero.GetType().GetField("m_comboResetTime").GetValue(hero);
            m_showDirectionIndicator = true;
            m_hasSpecialAttack = (bool)hero.GetType().GetField("m_hasSpecialAttack").GetValue(hero);
            m_specialAttack = (AttackData)hero.GetType().GetField("m_specialAttack").GetValue(hero);

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
        public exSpriteAnimClip m_animationSpawn; //does not need to be filled
        public exSpriteAnimClip m_animationIdle; //fill
        public exSpriteAnimClip m_animationRun; //fill
        public exSpriteAnimClip m_animationDead; //fill
        public exSpriteAnimClip m_animationKnockback; //fill
        public exSpriteAnimClip m_animationStatue; //fill 
        public PlayerShadow m_shadowPrefab; //fill
        public bool allowRedShadow = true;
        public DamageEffectData[] m_spawnOnHit; //fill
        public DamageEffectData[] m_spawnOnDeath; //fill
        public bool m_bloodEffectsEnabled = true;
        public bool m_delayGhostSpawnOnDeath = false; //not fill

        //Attacks
        public Vector2 m_attackSpawnNode = Vector2.right;
        public Attacks.eAttackOrderType m_attackOrderType;
        public float m_comboResetTime;
        public bool m_showDirectionIndicator;
        public bool m_specialCancelsCooldown = true;
        public List<AttackData> m_attacks = new List<AttackData>(1);
        public bool m_hasSpecialAttack;
        public float m_specialAttackChargeTime = 5f;
        public AttackData m_specialAttack;
        public List<DamageEffectData> m_attackDamageEffects = new List<DamageEffectData>(1);
        public Powerup m_attackPowerupPrefab;
        public bool m_holdToAim;

        //Knockback
        public float m_knockbackSpeed = 200f;
        public float m_knockbackTime = 0.3f;
        public bool m_knockbackCancelsAttack;

        //Sound
        public AudioCue m_soundHit;
        public AudioCue m_soundDie;

        //Portrait
        public bool m_portraitEnabled = true;
        public Vector2 m_portraitOffset = Vector2.zero;
        public Vector2 m_portraitOffsetEvolve = Vector2.zero;
        public bool m_portraitFlipped;
        public exSpriteAnimClip m_portraitAnim;

        //Bots
        public float m_botTargetChance = 1f; //If less than 1 then there'll be a chance that hero  won't prioritise this as a target above others, lower the value, less the chance
        

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
        public static void RecursivePrintFields(int howMuch) //pog idea
        {

        }
        public static void PrintPlayer(Player i)
        {
            foreach (var j in i.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (j.GetCustomAttributes(true).Where(e => e.GetType().Name == "SerializeField").FirstOrDefault() != null)
                {
                    Console.WriteLine(j.Name + ":" + j.GetValue(i));
                    if (j.GetValue(i) != null && j.GetValue(i).GetType() == typeof(exSpriteAnimClip))
                    {
                        foreach (var m in j.GetValue(i).GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        {
                            var val = m.GetValue(j.GetValue(i));
                            Console.WriteLine("\t" + m.Name + ":" + val);
                            if (m.Name == "frameInfos")
                            {

                                foreach (exSpriteAnimClip.FrameInfo val2 in (List<exSpriteAnimClip.FrameInfo>)val)
                                {

                                    Console.WriteLine("\t\t" + "textureGUID" + ":" + val2.textureGUID);
                                    Console.WriteLine("\t\t" + "length" + ":" + val2.length);
                                    Console.WriteLine("\t\t" + "atlas" + ":" + val2.atlas);
                                    Console.WriteLine("\t\t" + "index" + ":" + val2.index);
                                }
                            }
                            else if (m.Name == "eventInfos") //this should work, idk though
                            {
                                foreach (exSpriteAnimClip.EventInfo val2 in (List<exSpriteAnimClip.EventInfo>)val)
                                {
                                    foreach (var val3 in val2.GetType().GetFields())
                                    {
                                        Console.WriteLine("\t\t" + val3.Name + ":" + val3.GetValue(val2));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}