using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CoreLib;
using CoreLib.Submodules.CustomEntity;
using CoreLib.Submodules.CustomEntity.Atributes;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MovableRuins;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.le4fless.corelib")]
[CoreLibSubmoduleDependency(new string[]
    {
        "LocalizationModule",
        "CustomEntityModule"
    })]
public class MovableRuins : BasePlugin
{
    public const string GUID = "com.CaptainStupid.MovableRuins";
    public const string NAME = "MovableRuins";
    public const string VERSION = "1.0.0";

    internal static MovableRuins Instance { get; private set; }
    public static ManualLogSource Logger { get; private set; }

    public override void Load()
    {
        Instance = this;
        Logger = base.Log;

        foreach (ObjectID ruin in Enum.GetValues(typeof(ObjectID)))
        {
            
            if (ruin.ToString().ToLower().Contains("ruinspiece"))
            {
                Logger.LogInfo($"Adding localization for {ruin}");
                CustomEntityModule.AddEntityLocalization(ruin,
                $"Ancient Ruins {ruin.ToString().ToCharArray()[10]}",
                "Whatever purpose it served is unknown, but it might make a good decoration!");
            }

        }

        CustomEntityModule.RegisterModifications(typeof(MovableRuins));

        var ourAssembly = Assembly.GetExecutingAssembly();
        var resources = ourAssembly.GetManifestResourceNames();
        foreach (var resource in resources)
        {
            if (!resource.EndsWith(".png"))
                continue;

            var stream = ourAssembly.GetManifestResourceStream(resource);

            var ms = new MemoryStream();
            stream.CopyTo(ms);
            var resourceName = Regex.Match(resource, @"([a-zA-Z\d\-_]+)\.png").Groups[1].ToString();
            ResourceManager.LoadSprite("MovableRuins", resourceName, ms.ToArray());
        }

        Log.LogInfo($"{NAME} is loaded!");
    }

    [EntityModification(ObjectID.RuinsPiece1)]
    private static void RuinsPiece1(EntityMonoBehaviourData entity)
    {
        ModifyRuins(entity);

    }

    [EntityModification(ObjectID.RuinsPiece2)]
    private static void RuinsPiece2(EntityMonoBehaviourData entity)
    {
        ModifyRuins(entity);
    }


    [EntityModification(ObjectID.RuinsPiece3)]
    private static void RuinsPiece3(EntityMonoBehaviourData entity)
    {
        ModifyRuins(entity);
    }

    [EntityModification(ObjectID.RuinsPiece4)]
    private static void RuinsPiece4(EntityMonoBehaviourData entity)
    {
        ModifyRuins(entity);
    }

    private static void ModifyRuins(EntityMonoBehaviourData entity)
    {
        Logger.LogInfo($"Entity name: {entity.name}");
        GameObject gameObject = entity.gameObject;
        gameObject.AddComponent<RequiresDrillCDAuthoring>();
        gameObject.AddComponent<DestructibleObjectCDAuthoring>();
        gameObject.AddComponent<PugAutomation.AutomatedMineableAuthoring>();
        HealthCDAuthoring healthCDAuthoring = gameObject.GetComponent<HealthCDAuthoring>();
        healthCDAuthoring.maxHealth = 12000;
        healthCDAuthoring.startHealth = 12000;
        entity.objectInfo.icon = ResourceManager.GetSprite($"MovableRuins.{entity.name}16x");
        entity.objectInfo.smallIcon = ResourceManager.GetSprite($"MovableRuins.{entity.name}8x");
        entity.objectInfo.objectType = ObjectType.PlaceablePrefab;
        GameObject.Destroy(gameObject.GetComponent<HealthRegenerationCDAuthoring>());
        GameObject.Destroy(gameObject.GetComponent<IndestructibleCDAuthoring>());
    }
}
