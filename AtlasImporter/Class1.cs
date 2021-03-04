using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AtlasImporter
{
	[BepInPlugin("org.anumania.plugins.atlasimporter", "imports some atlases(not all of them)", "1.0.0.0")]
	public class Class1 : BaseUnityPlugin
    {
        void Awake()
        {
			exAtlas[] array = (exAtlas[])Resources.FindObjectsOfTypeAll(typeof(exAtlas));
			if (!Directory.Exists("./Crawl_Textures"))
			{
				Directory.CreateDirectory("./Crawl_Textures");
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (File.Exists("./Crawl_Textures/" + array[i].name + ".png"))
				{
					Console.WriteLine("found " + array[i] + ", loading...");
					byte[] array2 = File.ReadAllBytes("./Crawl_Textures/" + array[i].name + ".png");
					array[i].texture.LoadImage(array2);
					Console.WriteLine("loaded!");
				}
			}
		}
    }
}
