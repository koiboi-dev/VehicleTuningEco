using System;
using System.Collections.Generic;
using System.IO;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Eco.Mods.MechanicExpansion
{
    internal class TuneManager
    {
        public static int TUNE_COUNT = 6;
        private static bool Initialized = false;
        private static readonly String FILE_NAME = "./Configs/MechanicExpansion.Eco";
        private static readonly String BACKUP_FILE_NAME = "./Configs/MechanicExpansionBackup.Eco";
        private static readonly int VERSION = 2;
        
        public static float[][] DRAGS = new[]
        {
            /* Max Speed       */ new []{0f   , 0.15f , 0.20f , 0f   , 0.10f , 0f}, // MaxSpeed ^, Fuel Consumption v, Emissions v, StorageCapacity ^, Durability, v, Offroad, -
            /* FuelConsumption */ new []{0.15f, 0f    , -0.25f, 0.15f, 0f    , 0f},
            /* Emissions       */ new []{0.20f, -0.25f, 0f    , 0f   , 0.4f  , 0f},
            /* StorageCapacity */ new []{0.15f, 0.15f , 0.15f , 0f   , 0.15f , 0f},
            /* Durability      */ new []{0f   , 0f    , 0f    , 0f   , 0f    , 0f},
            /* Offroading      */ new []{0f   , 0f    , 0f    , 0f   , 0f    , 0f}
        };
        
        private static Dictionary<Type, VehicleTuneData> Relations = new Dictionary<Type, VehicleTuneData>();

        private static void LoadVehicleRelations()
        {
            bool isFirst = false;
            if (!File.Exists(FILE_NAME))
            {
                File.WriteAllText(FILE_NAME, "{'vehicle_tunes':{}}");
                isFirst = true;
            }
            
            JObject expansionFile = JToken.ReadFrom((JsonReader)new JsonTextReader((TextReader)File.OpenText(FILE_NAME))) as JObject;
            if (!isFirst &&( expansionFile["version"] is null || (int)expansionFile["version"] != VERSION))
            {
                Log.WriteError(Localizer.Do($"ERROR: MechanicsExpansion.Eco is of data version {expansionFile["version"]} however, MechanicExpansion is expecting version {VERSION}.\nAll tune values WILL be reset on the next server start. To keep the data a backup file has been made with the current values. \nAfter a reset ensuring that the new file has version '{VERSION}', you can replace all the values under 'tune' with their corresponding values in the old file."));
                File.Copy(FILE_NAME, BACKUP_FILE_NAME, true);
            }

            if (expansionFile["vehicle_tunes"] == null)
            {
                Log.WriteError(Localizer.Do($"ERROR: Unable to load file due to missing tune array, using defaults instead."));
                return;
            }
            JObject weightArray = (JObject)expansionFile["vehicle_tunes"];

            foreach (KeyValuePair<string, JToken?> token in weightArray)
            {
                Type? vehicleType = Type.GetType(token.Key);
                if (vehicleType == null)
                {
                    Log.WriteError(new LocString("Unable to load vehicle of type " + token.Key + " unknown class name!"));
                    continue;
                }

                if (token.Value != null)
                {
                    VehicleTuneData data = new VehicleTuneData(token.Value);
                    Relations.Add(vehicleType, data);
                }
            }

            if (expansionFile["drags"] == null)
            {
                Log.WriteError(Localizer.Do($"ERROR: Unable to load custom drags due to missing JSON data."));
                return;
            }
            DRAGS = ((JArray)expansionFile["drags"]).ToObject<float[][]>();
        }
        
        public static void SaveVehicleRelations()
        {
            JObject tuneFile = new JObject();
            tuneFile["version"] = VERSION;
            JObject tunesObject = new JObject();
            foreach (KeyValuePair<Type, VehicleTuneData> pair in Relations)
            {
                tunesObject[pair.Key.AssemblyQualifiedName] = pair.Value.toJSON();
            }

            tuneFile["vehicle_tunes"] = tunesObject;
            tuneFile["drags"] = JArray.FromObject(DRAGS);
            File.WriteAllText(FILE_NAME, tuneFile.ToString());
        }
        
        public static void AddVehicle<T>(TuneValueTemplate maxSpeedWeights, TuneValueTemplate fuelConsumptionWeights, TuneValueTemplate co2EmissionWeights, TuneValueTemplate storageCapacityWeights, TuneValueTemplate durabilityWeights, TuneValueTemplate offroadWeights)
        {
            if (!Initialized) 
            {
                Log.WriteError(new LocString("ERROR: Tried to add vehicle relation before relation manager was initalized!"));
                return;
            }
            if (Relations.ContainsKey(typeof(T)))
            {
                return;
            }

            Type itemObjectType = typeof(T);
            Type? worldObjectItemType = itemObjectType.BaseType;
            if (worldObjectItemType == null || worldObjectItemType.GetGenericTypeDefinition() != typeof(WorldObjectItem<>))
            {
                Log.WriteError(Localizer.Do($"ERROR: Provided type {itemObjectType} does not use WorldObjectItem"));
                return;
            }

            Type worldObjectType = worldObjectItemType.GetGenericArguments()[0];
            if (!worldObjectType.IsSubclassOf(typeof(PhysicsWorldObject)))
            {
                Log.WriteError(Localizer.Do($"ERROR: Provided type {itemObjectType} is not a vehicle."));
                return;
            }
            
            
            Relations.Add(itemObjectType, new VehicleTuneData(
                maxSpeedWeights,
                fuelConsumptionWeights,
                co2EmissionWeights,
                storageCapacityWeights,
                durabilityWeights,
                offroadWeights
                )
            );
            /*MethodInfo info = worldObjectType.GetMethod("ExposedModsPreInitialize",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            Log.WriteLine(Localizer.Do($"Type: {worldObjectType}, Method: {info} + {info.GetType()}"));
            //Traverse traverse = Traverse.Create(worldObjectType);
            //traverse.Method("ModsPreInitialize");
           // VehicleMethodInfo method = new VehicleMethodInfo(info, efficiencyMultiplier, mountCount, storageSlots, weightCapacity);
            MethodInfo method2 = typeof(TuneRelationManager).GetMethod("ModsPreInitializeInj", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            Log.WriteLine(Localizer.Do($"Successful init of vehicle method: {info} and method 2: {method2}, patching now... {method2.GetType()}"));
            MechanicExpansion.harmonyInstance.Patch(info, method2);*/
        }

        public static EvaluatedData EvaluateVehicle(Type type, int[] tunes)
        {
            /*float[] fTunes = new float[tunes.Length];
            for (int i = 0; i < tunes.Length; i++)
            {
                fTunes[i] = tunes[i] / 100f;
            }*/
            
            VehicleTuneData data = Relations[type];
            return data.Evaluate(tunes);
        }

        public static void Initalize()
        {
            Log.WriteLine(new LocString("Loading vehicle relations..."));
            LoadVehicleRelations();
            Initialized = true;
        }
        
        public static void Deinitalize()
        {
            SaveVehicleRelations();
        }

        public static VehicleTuneData GetTuneRelation(Type vType)
        {
            return Relations[vType];
        }

        public static EvaluatedData GetDefaultData(Type vType)
        {
            if (!Relations.ContainsKey(vType))
            {
                Log.WriteError(Localizer.DoStr("No type"));
            }
            return Relations[vType].Evaluate(new []{0, 0, 0, 0, 0, 0});
        }
    }
}