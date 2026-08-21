using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HG;
using R2API;
using R2API.AddressReferencedAssets;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2BepInExPack.GameAssetPaths;
using RoR2BepInExPack.GameAssetPathsBetter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using RoR2.Navigation;
using DesolateCoast.Content;
//Copied from a template project that I made copied from Broadcast Perch copied from a private Unity project I use for testing maps copied from Ancient Observatory copied from Wetland Downpour copied from Fogbound Lagoon copied from Nuketown


#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace DesolateCoast
{
    [BepInPlugin(GUID, Name, Version)]
    public class DesolateCoast : BaseUnityPlugin
    {
        public const string Author = "wormsworms";

        public const string Name = "Desolate_Coast";

        public const string Version = "1.0.0";

        public const string GUID = Author + "." + Name;

        public static DesolateCoast instance;

        public static ConfigEntry<bool> enableRegular;
        public static ConfigEntry<bool> enableSimulacrum;
        public static ConfigEntry<bool> stage1Simulacrum;

        public static ConfigEntry<bool> toggleSandCrab;

        public static ConfigEntry<bool> toggleWayfarer;

        public const string mapName = "coast_wormsworms";
        public const string simuName = "itcoast_wormsworms";
        private static GameObject fanPrefab;


        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            ConfigSetup();

            ContentManager.collectContentPackProviders += GiveToRoR2OurContentPackProviders;

            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            SceneManager.sceneLoaded += SceneSetup;

            RoR2.RoR2Application.onLoadFinished += AddModdedEnemies;

        }
        
        public static void AddModdedEnemies()
        {
            if (IsEnemiesReturns.enabled)
            {
                EnemiesReturnsCompat.AddEnemies(); //Sand Crab
            }
            if (IsStarstorm2.enabled)
            {
                Starstorm2Compat.AddEnemies(); //Wayfarer
            }
        }
        

        private void Destroy()
        {
            RoR2.Language.collectLanguageRootFolders -= CollectLanguageRootFolders;
        }

        private static void GiveToRoR2OurContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentProvider());
        }

        public void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Language"));
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Plugins/Language"));
        }

        private void CoastAmbienceSetup()
        {
            GameObject ambience = GameObject.Find("Ambience");
            if (ambience)
            {
                AkBank bank = ambience.GetComponent<AkBank>();
                AkAmbient[] ambientList = ambience.GetComponents<AkAmbient>();
                AkAmbient ambient1 = ambientList[0];
                AkAmbient ambient2 = ambientList[1];
                if (bank)
                {
                    WwiseBankReference rjSound = Addressables.LoadAssetAsync<WwiseBankReference>("Wwise/8AC8A9CB-604F-43BC-A864-873DC735786F.asset").WaitForCompletion();
                    WwiseEventReference startRJSound = Addressables.LoadAssetAsync<WwiseEventReference>("Wwise/6C9A5B06-3C87-4DD2-835F-B0F2385B7700.asset").WaitForCompletion();
                    WwiseEventReference stopSound = Addressables.LoadAssetAsync<WwiseEventReference>("Wwise/6F2ADD1C-BD55-431F-A62F-80CCD5F9631D.asset").WaitForCompletion();
                    bank.data.WwiseObjectReference = rjSound;
                    ambient1.data.WwiseObjectReference = startRJSound;
                    ambient2.data.WwiseObjectReference = stopSound;
                }
            } else
            {
                Log.Error("no ambience :(");
            }

        }

        private void SceneSetup(Scene newScene, LoadSceneMode loadSceneMode)
        {
            if (newScene.name == mapName || newScene.name == simuName)
            {
                CoastAmbienceSetup();

                Transform geyserHolder = GameObject.Find("HOLDER: Geysers").transform;
                for (int i = 0; i < geyserHolder.childCount; i++)
                {
                    GameObject geyserRing1 = geyserHolder.GetChild(i).GetChild(0).GetChild(0).gameObject;
                    GameObject geyserRing2 = geyserHolder.GetChild(i).GetChild(0).GetChild(1).gameObject;
                    geyserRing1.GetComponent<MeshRenderer>().material = DesolateCoastContent.terrainMaterial;
                    geyserRing2.GetComponent<MeshRenderer>().material = DesolateCoastContent.terrainMaterial;
                }

                Transform pillarGeyser = GameObject.Find("HOLDER: Random/Spires/Geyser").transform;
                GameObject pgRing1 = pillarGeyser.GetChild(0).GetChild(0).gameObject;
                GameObject pgRing2 = pillarGeyser.GetChild(0).GetChild(1).gameObject;
                pgRing1.GetComponent<MeshRenderer>().material = DesolateCoastContent.terrainMaterial;
                pgRing2.GetComponent<MeshRenderer>().material = DesolateCoastContent.terrainMaterial;
            }

        }

        private void ConfigSetup()
        {
            enableRegular =
                base.Config.Bind<bool>("00 - Stages",
                                       "Enable Desolate Coast",
                                       true,
                                       "If true, Desolate Coast can appear in regular runs.");
            enableSimulacrum =
                base.Config.Bind<bool>("00 - Stages",
                                       "Enable Simulacrum Variant",
                                       true,
                                       "If true, Desolate Coast can appear in the Simulacrum.");
            stage1Simulacrum =
                base.Config.Bind<bool>("00 - Stages",
                                       "Enable Simulacrum Variant on Stage 1",
                                       false,
                                       "If false, Desolate Coast will only appear after clearing at least one stage in the Simulacrum, like Commencement.");
            toggleSandCrab =
                base.Config.Bind<bool>("01 - Monsters: EnemiesReturns",
                                       "Enable Sand Crab",
                                       true,
                                       "If true, Sand Crabs will appear in Desolate Coast.");
            toggleWayfarer =
                base.Config.Bind<bool>("03 - Monsters: Starstorm 2",
                                       "Enable Wayfarer",
                                       true,
                                       "If true, Wayfarers will appear in Desolate Coast.");
        }
    }
}
