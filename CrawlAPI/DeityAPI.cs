using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    class DeityAPI
    {
        //TODO: standardize either the use of "custom deity" or "modded deity" terminology in the api;
        public static List<Deity> customDeities;
        public static List<MenuDeitySelectPlayer> deitySelectMenu;
        public static List<MenuTextMenu> customDeityMenu;

        public static void Init()
        {
            customDeities = new List<Deity>();
            On.MenuMain.Start += MenuStartHook;

            On.MenuDeitySelectPlayer.Update += (a, b) =>
            {
                

            };
        }

        private static void MenuStartHook(On.MenuMain.orig_Start orig, MenuMain self)
        {
            orig(self);
            MenuDeitySelectPlayer[] asd = Resources.FindObjectsOfTypeAll<MenuDeitySelectPlayer>();
            deitySelectMenu = new List<MenuDeitySelectPlayer>(new MenuDeitySelectPlayer[] { null, null, null, null }); //for some reason new list constructor doesnt work :/
            customDeityMenu = new List<MenuTextMenu>(new MenuTextMenu[] { null, null, null, null }); 
            foreach (MenuDeitySelectPlayer mdsp in asd)
            {
                try
                {
                    int index = MenuPlayerNumber.GetPlayerNumber(mdsp.gameObject);
                    deitySelectMenu[index] = mdsp;

                    MenuStateMachine sm = mdsp.transform.parent.gameObject.GetComponent<MenuStateMachine>();

                    MenuState deityTrialSelectState = sm.m_states[sm.GetStateId("DeityTrialSelect")];
                    MenuState deityCustomSelectState = new MenuState();

                    foreach (FieldInfo field in typeof(MenuState).GetFields()) //perform shallow copy to make a new state
                    {
                        field.SetValue(deityCustomSelectState, field.GetValue(deityTrialSelectState));
                    }
                    deityCustomSelectState.m_actions = new List<MenuStateAction>();
                    deityCustomSelectState.m_turnOn = new List<GameObject>(); //reset this baby because its a copy of the trials state
                    deityCustomSelectState.m_name = "DeityModdedSelect";
                    sm.m_states.Add(deityCustomSelectState);


                    GameObject deityCustomMenu = GameObject.Instantiate(mdsp.gameObject.transform.FindChild("ContainerInfo").FindChild("DeityMenu").gameObject);
                    GameObject deityCustomText = GameObject.Instantiate(mdsp.gameObject.transform.FindChild("TextWorships").gameObject);

                    deityCustomMenu.SetActive(false);
                    deityCustomText.SetActive(false);

                    deityCustomMenu.AddComponent<DeityModMenu>().index = index;

                    customDeityMenu[index] = deityCustomMenu.GetComponent<MenuTextMenu>();

                    deityCustomMenu.name = "DeityCustomMenu";
                    deityCustomText.name = "DeityCustomText";

                    deityCustomMenu.transform.parent = mdsp.gameObject.transform;
                    deityCustomText.transform.parent = mdsp.gameObject.transform;

                    deityCustomMenu.transform.localPosition = mdsp.gameObject.transform.FindChild("ContainerInfo").FindChild("DeityMenu").gameObject.transform.localPosition;

                    deityCustomText.transform.localPosition = mdsp.gameObject.transform.FindChild("TextWorships").gameObject.transform.localPosition;

                    deityCustomText.GetComponent<TextMesh>().text = "Custom Deities";

                    sm.m_states[sm.GetStateId("DeityModdedSelect")].m_turnOn.AddRange(new List<GameObject> { deityCustomMenu, deityCustomText });
                    sm.m_states[sm.GetStateId("DeityModdedSelect")].m_turnOff.AddRange(new List<GameObject> { mdsp.gameObject.transform.FindChild("ContainerInfo").FindChild("DeityMenu").gameObject, mdsp.gameObject.transform.FindChild("TextWorships").gameObject });

                    sm.m_states[sm.GetStateId("DeitySelect")].m_turnOff.AddRange(new GameObject[] { deityCustomMenu, deityCustomText });
                    

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //Console.WriteLine(e.StackTrace);
                }

            }
        }

        public static void AddDeity(CustomDeity customDeity)
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
            customDeities.Add(deity);
        }
        //dont run this too early or get big crash
        public static void UpdateCustomDeities(int index)
        {
            MenuTextMenu customMenu = customDeityMenu[index];
            for(int i = 0; i < customMenu.GetItemCount(); i++) //reset menu
            {
                customMenu.RemoveItem(i);
            }
            if (customMenu)//somehow this is null sometimes.
            {
                FieldInfo menu = typeof(MenuDeitySelectPlayer).GetField("m_menu", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo addItemToMenu = typeof(MenuTextMenu).GetMethod("AddItem", BindingFlags.Public | BindingFlags.Instance);
                foreach (Deity deity in customDeities)
                {
                    MenuTextMenuItemData modDeitiesList = new MenuTextMenuItemData
                    {
                        m_enabled = true,
                        m_prefabMenuItem = (MenuTextMenuItem)typeof(MenuDeitySelectPlayer).GetField("m_prefabMenuItemTrial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(deitySelectMenu[index]),
                        m_message = "MsgOnSelectModDeities",
                    };
                    customMenu.AddItem(modDeitiesList);
                }

            }
            
        }
    }
    //
    public class DeityModMenu:MonoBehaviour
    {
        public int index;
        private void OnEnable()
        {
            DeityAPI.UpdateCustomDeities(index);
        }

        private void Update()
        {
            
        }
    }
}
