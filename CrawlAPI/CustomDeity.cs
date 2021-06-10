﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    public class CustomDeity
    {
        public CustomDeity()
        {

        }

        public override string ToString()
        {
            FieldInfo[] fieldInfos = this.GetType().GetFields();
            foreach (FieldInfo info in fieldInfos)
            {
                if (info.FieldType == typeof(exSpriteAnimClip))
                {
                    return "still working on it";
                }
                else
                {
                    return (string)info.GetValue(this);
                }
            }
            return "still working on it";
        }
        public void SetToDefaults()
        {

            //FieldInfo[] targetFieldInfo = typeof(Deity).GetFields(BindingFlags.Instance | BindingFlags.NonPublic); //reference to field info of the class
            FieldInfo[] thisFieldInfo = typeof(CustomDeity).GetFields(BindingFlags.Instance | BindingFlags.Public); //reference to the field info here
            Deity inst = SystemDeity.GetDeity(0); //hold this bc its referenced a ton
            Console.WriteLine(inst);
            foreach(FieldInfo fieldInfo in thisFieldInfo)
            {
                FieldInfo targetFieldInfo = typeof(Deity).GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                if(targetFieldInfo!= null)
                {
                    object val = targetFieldInfo.GetValue(inst);
                    fieldInfo.SetValue(this, val);
                }
                else
                {
                    Console.WriteLine("oops" + fieldInfo.Name);
                }
            }
        }
        //text
        public string m_name = "testDeity";
        public string m_text = "no idea what this is";
        public string m_textFlavour = "flavor text";

        //anim
        public exSpriteAnimClip m_portrait;

        //audio
        public AudioCue m_soundSelected;

        //misc
        public bool m_neverChooseRandomly; //no idea what this is
        public bool m_isTrial; //neither

        //trial stuff
        public GameObject[] m_startingMonsters;
        public PlayerModifier[] m_modifiers;

        //starting modifiers
        public List<HeroStatModifier> m_heroStatModifiers = new List<HeroStatModifier>();
        public int m_gold; //starting gold
        public Weapon m_startingWeapon;
        public Powerup[] m_startingPowerups;

        //idk
        public bool m_useLargeFodder;
        public exSpriteAnimClip m_winHeroAnimation;
        public string m_winText;

        //hinderences 
        public bool m_weaponsForbidden;
        public bool m_spellsForbidden;
        public bool m_artifactsForbidden;
        public bool m_potionsForbidden;
    }
}
