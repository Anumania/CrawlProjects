using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    public static class MonsterAPI //used to add monsters. probably not gonna work well, replacing is probably better.
    {
        public static Player[] monsters;
        public static void Init()
        {
            monsters = (Player[])Resources.FindObjectsOfTypeAll(typeof(Player));

        }

        public static Player GetMonster(string monsterName) //this does not work with mod added monsters!
        {
            return monsters.Where(e => e.name == monsterName).FirstOrDefault();
        }

        public static void AddMonster(CustomMonster monst)
        {
            Deity bart = SystemDeity.GetDeity(0);
            GameObject[] startingMonsters = bart.GetStartingMonsters();

            Player p = startingMonsters[0].GetComponent<Player>();
            //a.SetActive(true);
            foreach (var field in monst.GetType().GetFields())
            {
                //Console.WriteLine(field.Name);
                FieldInfo fldInfo = p.GetType().GetField(field.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fldInfo == null)
                {
                    continue;
                }
                if (fldInfo.Name == "m_evolveToCostOverride")
                {
                    Player.EvolveCostOverride[] e = EvolveCostOverride.convert((EvolveCostOverride[])field.GetValue(monst));

                    fldInfo.SetValue(p, e);
                }
                else
                {
                    try
                    {
                        fldInfo.SetValue(p, field.GetValue(monst));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        fldInfo.SetValue(p, null);
                    }
                }
            }
            //APIHelpers.PrintPlayer(p);
            //bart.GetType().GetField("m_startingMonsters", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(bart, startingMonsters);
        }
        public class EvolveCostOverride
        {
            public EvolveCostOverride(GameObject evolveTo, float _price = 0)
            {
                price = _price;
                m_evolveTo = evolveTo;
            }
            public static implicit operator Player.EvolveCostOverride(EvolveCostOverride e)
            {
                var eo = new Player.EvolveCostOverride();
                eo.m_evolveTo = e.m_evolveTo;
                eo.price = e.price;
                return eo;
            }
            public static Player.EvolveCostOverride[] convert(EvolveCostOverride[] e)
            {
                var eoa = new Player.EvolveCostOverride[e.Length];
                for (int i = 0; i < eoa.Length; i++)
                {
                    var eo = new Player.EvolveCostOverride();
                    eo.m_evolveTo = e[i].m_evolveTo;
                    eo.price = e[i].price;
                    eoa[i] = eo;
                }
                return eoa;
            }
            /*public static implicit operator Player.EvolveCostOverride[](EvolveCostOverride[] e)
            {
                var eoa = new Player.EvolveCostOverride[e.Length];
                for (int i = 0; i < eoa.Length; i++)
                {
                    var eo = new Player.EvolveCostOverride();
                    eo.m_evolveTo = e[i].m_evolveTo;
                    eo.price = e[i].price;
                    eoa[i] = eo;
                }
                return eoa;
            }*/
            public GameObject m_evolveTo;
            public float price;
        }

    }
}
