using Eco.Core.Controller;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Shared.Localization;
using Eco.Shared.Logging;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using System.ComponentModel;
using Eco.Core.Utils;
using Eco.Core.PropertyHandling;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Store;
using Eco.Gameplay.DynamicValues;
using Eco.Gameplay.Economy;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Gameplay.Utils;
using Eco.Mods.TechTree;
using Eco.Shared.Items;

namespace Eco.Mods.MechanicExpansion
{
    /*public class TuneBarsContainer : IController, INotifyPropertyChanged
    {
        public ControllerList<TuneBar> TuneBars { get; set; }

        public TuneBarsContainer()
        {
            TuneBars = new ControllerList<TuneBar>(this, nameof(TuneBars));
        }
        
        
        #region IController
        public event PropertyChangedEventHandler PropertyChanged;
        int controllerID;
        public ref int ControllerID => ref this.controllerID;
        #endregion
    }*/
    
    [Serialized]
    public class TuneBar: IController, INotifyPropertyChanged
    {
        [Eco] [Serialized, SyncToView, Range(-10, 10)] public int Tune { 
            get => tune;
            set
            {
                tune = Math.Clamp(value, -10, 10);
                this.Changed(nameof(Tune));
            }
        }
        private int tune;
        
        #region IController
        public event PropertyChangedEventHandler PropertyChanged;
        int controllerID;
        public ref int ControllerID => ref this.controllerID;
        #endregion
    }
    
    [Serialized]
    [Priority(-2)]
    [CreateComponentTabLoc("Tune Bench", true)]
    [LocDescription("CONFIGURE EVERYTHING !!!!!")]
    [RequireComponent(typeof (StatusComponent), null)]
    [RequireComponent(typeof(PluginModulesComponent), null)]
    [HasIcon(null)]
    public class TuningComponent : WorldObjectComponent, IHasClientControlledContainers
    {
        public User? vehicleAddedUser;
        
        [SyncToView(null, true)]
        [DependsOnMember("Inventory")]
        public bool HasTunableItem
        {
            get { return _HasTunableItem; }
            set
            {
                _HasTunableItem = value;
                this.Changed(nameof(HasTunableItem));
            }
        }
        
        private bool _HasTunableItem;

        [Autogen]
        [SyncToView]
        [UITypeName("StringTitle")]
        [PropReadOnly]
        public string InventoryTitle
        {
            get => Localizer.Do($"Item to Tune");
        }
        [Autogen]
        [SyncToView]
        [UITypeName("ItemInput")]
        [AutoRPC]
        [Serialized]
        public Inventory Inventory { get; set; }
        
        [Serialized, Eco,SyncToView, HideRoot, NewTooltipChildren(CacheAs.Instance)]
        [VisibilityParam("HasTunableItem")]
        [ShowFullObject]
        public TuneBar[] TuneBars { get; set; }

        [Autogen]
        [SyncToView]
        [UITypeName("StringTitle")]
        [PropReadOnly]
        [VisibilityParam("HasTunableItem")]
        public string TuneValueString
        {
            get => GetTuneString().ToString();
        }

        public static SkillModifiedValue MechanicSkillBonusPoint = new SkillModifiedValue(0f, new AdditiveStrategy(new float[]{0, 1, 2, 3, 4, 5, 6}), typeof(MechanicsSkill), typeof(MechanicsSkill), new LocString("Gives you more points to allocate towards tuning"), DynamicValueType.Yield);

        private bool HasTuneChanged = false;
        

        public TuningComponent()
        {
            Inventory = new AuthorizationInventory(1);
        }
        
        public override void Initialize()
        {
            HasTunableItem = false;
            base.Initialize();
            //this.Parent.GetComponent<PluginModulesComponent>().OnChanged.Add(OnVehicleAdded);
            Inventory.OnChanged.Add(OnVehicleAdded);
            TuneBars = new TuneBar[TuneRelationManager.TUNE_COUNT];
            for (int i = 0; i < TuneBars.Length; i++)
            {
                TuneBars[i] = new TuneBar();
                TuneBars[i].Subscribe(nameof(TuneBar.Tune), OnTuneChanged);
            }
            Inventory.SetOwner(Parent);
        }

        public void OnVehicleAdded(User user)
        {
            if (!IsItemTunable(Inventory.Stacks.First().Item))
            {
                HasTunableItem = false;
                vehicleAddedUser = null;
                return;
            }
            HasTunableItem = !Inventory.IsEmpty;
            vehicleAddedUser = user;
            base.Parent.SetDirty();
        }

        public static bool IsItemTunable(Item item)
        {
            if (item is IPersistentData data)
            {
                if (data.PersistentData is ItemPersistentData itemData)
                {
                    return itemData.Entries.Keys.Contains(Type.GetType("Eco.Mods.MechanicExpansion.TuneableComponent"));
                }
            }

            return false;
        }
        
        [RPC]
        [Autogen]
        [UITypeName("BigButton")]
        [VisibilityParam("HasTunableItem")]
        public void ApplyTune(Player player)
        {
            Log.WriteLine(Localizer.Do($"HOLY TUNE"));
            if (HasTunableItem)
            {
                ItemStack item = Inventory.Stacks.First();
                int[] tuneValues = new int[TuneRelationManager.TUNE_COUNT];
                for (int i = 0; i < TuneRelationManager.TUNE_COUNT; i++)
                {
                    tuneValues[i] = TuneBars[i].Tune;
                }
                EvaluatedData evalData = TuneRelationManager.EvaluateVehicle(item.Item.GetType(), tuneValues);
                if (item.Item is IPersistentData data && data.PersistentData is ItemPersistentData itemData)
                {
                    itemData.SetPersistentData<TuneableComponent>(evalData);
                }
            }
        }

        public void OnTuneChanged()
        {
            Log.WriteLine(new LocString($"Values have been changed, "));
            this.Changed(nameof(TuneBars));
            this.Changed(nameof(TuneValueString));
            HasTuneChanged = true;
        }

        [DependsOnMember("TuneBars")]
        public LocString GetTuneString()
        {
            Log.WriteLine(Localizer.Do($"Getting Tune String"));
            return GetTuneStringWithInaccuracy(0.75f);
        }
        
        private EvaluatedData? PrevData = null;
        
        [DependsOnMember("TuneBars")]
        public LocString GetTuneStringWithInaccuracy(float accuracy)
        {
            EvaluatedData? data = GetEvaluatedData();
            if (data == null)
            {
                return Localizer.Do($"No vehicle to tune.");
            }

            if (vehicleAddedUser == null || !vehicleAddedUser.IsOnline)
            {
                return Localizer.Do($"Invalid vehicle to tune.");
            }

            if (PrevData == null)
            {
                PrevData = data;
            }

            TuneRelation relation = TuneRelationManager.GetTuneRelation(Inventory.Stacks.First().Item.GetType());
            Log.WriteLine(Localizer.Do($"relation: {relation}"));
            LocString output = Localizer.Do(
                $"""
                 Current Tuner: {vehicleAddedUser.Name}
                 Available Points: <color={GetPointsColor(relation)}>{GetPoints(relation)}</color>
                 Bonus Points: <color=green>{MechanicSkillBonusPoint.GetCurrentValueInt(new UserDynamicValueContext(vehicleAddedUser), this)}</color>
                 Accuracy: <color={AccuracyToColor(accuracy)}>{accuracy}</color>%
                 <color={relation.MaxSpeedWeights.GetColorFromEvaluated(data.Value.MaxSpeedValue)}>Max Speed: {data.Value.MaxSpeedValue:0.0}</color> kmph ({GetTrendIcon(data.Value.MaxSpeedValue, PrevData.Value.MaxSpeedValue)})
                 <color={relation.FuelConsumptionWeights.GetColorFromEvaluated(data.Value.FuelConsumptionValue)}>Fuel Consumption: {data.Value.FuelConsumptionValue:0}</color> joules/s ({GetTrendIcon(data.Value.FuelConsumptionValue, PrevData.Value.FuelConsumptionValue, true)})
                 <color={relation.CO2EmissionWeights.GetColorFromEvaluated(data.Value.CO2EmissionValue)}>Emissions: {data.Value.CO2EmissionValue:0.00}</color> ppm/hour ({GetTrendIcon(data.Value.CO2EmissionValue, PrevData.Value.CO2EmissionValue, true)})
                 <color={relation.StorageCapacityWeights.GetColorFromEvaluated(data.Value.StorageCapacityValue)}>Storage Capacity: {data.Value.StorageCapacityValue/1000:0}</color> kg ({GetTrendIcon(data.Value.StorageCapacityValue, PrevData.Value.StorageCapacityValue)})
                 <color={relation.DecayMultiplierWeights.GetColorFromEvaluated(data.Value.DecayMultiplier)}>Decay Multiplier: {data.Value.DecayMultiplier*100:0.0}</color> % ({GetTrendIcon(data.Value.DecayMultiplier, PrevData.Value.DecayMultiplier, true)})
                 """
            );
            if (HasTuneChanged)
            {
                PrevData = data;
                HasTuneChanged = false;
            }
            return output;
        }
        public string GetTrendIcon(float current, float prev, bool flip = false)
        {
            if (current == prev)
            {
                return "<color=white>-</color>";
            }
            
            float difference = current - prev;
            float absoluteDifference = Math.Abs(difference);
            float percentDiff = absoluteDifference / prev;
            string color;
            if ((difference > 0 && !flip) || (difference < 0 && flip))
            {
                color = InterpolateColorGradient(110, 224, 110, 25, 97, 25, Math.Clamp(percentDiff * 8, 0, 1));
                return $"<color={color}>{(!flip ? '\u25b2' : '\u25bc')}</color>";
            }
            color = InterpolateColorGradient(255, 125, 125, 115, 0, 0, Math.Clamp(percentDiff * 8, 0, 1));
            return $"<color={color}>{(!flip ? '\u25bc' : '\u25b2')}</color>";
        }

        public static string InterpolateColorGradient(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, float t)
        {
            return $"#{((int)(r1 + (r2 - r1) * t)):X02}{(int)(g1 + (g2 - g1) * t):X02}{(int)(b1 + (b2 - b1) * t):X02}";
        }

        public string AccuracyToColor(float accuracy, bool flipped = false)
        {
            if (!flipped)
            {
                switch (accuracy)
                {
                    case < 0.1f:
                        return "red";
                    case < 0.25f:
                        return "fc5d60";
                    case < 0.5f:
                        return "#ffd30d";
                    case < 0.75f: 
                        return "#6eff0d";
                    default:
                        return "green";
                }
            }
            switch (accuracy)
            {
                case < 0.1f:
                    return "#6eff0d";
                case < 0.25f:
                    return "#ffd30d";
                case < 0.5f:
                    return "#f79d48";
                case < 0.75f:
                    return "fc5d60";
                default:
                    return "red";
            }
        }
        
        public int GetPoints(TuneRelation relation)
        {
            int points = 0;
            for (int i = 0; i < TuneBars.Length; i++)
            {
                points -= relation[i].GetPointCost(TuneBars[i].Tune);
            }

            if (vehicleAddedUser != null)
            {
                points += MechanicSkillBonusPoint.GetCurrentValueInt(new UserDynamicValueContext(vehicleAddedUser), this);
            }

            return points;
        }

        public string GetPointsColor(TuneRelation relation)
        {
            switch (GetPoints(relation))
            {
                case < 0:
                    return "red";
                case 0:
                    return "green";
                case <3:
                    return "#fcec03";
                default:
                    return "#fc3d03";
            }
        }
        public EvaluatedData? GetEvaluatedData()
        {
            if (!HasTunableItem)
            {
                return null;
            }
            ItemStack item = Inventory.Stacks.First();
            int[] tuneValues = new int[TuneRelationManager.TUNE_COUNT];
            for (int i = 0; i < TuneRelationManager.TUNE_COUNT; i++)
            {
                tuneValues[i] = TuneBars[i].Tune;
            }
            return TuneRelationManager.EvaluateVehicle(item.Item.GetType(), tuneValues);
        }

        /*ItemStack item = Parent.GetComponent<PluginModulesComponent>().Inventory.Stacks.First();
                Log.WriteLine(new LocString("Current Tune: " + Tune));
                if (item.Item is IPersistentData data)
                {
                    Log.WriteLine(new LocString("IPersistentData: " + data.PersistentData));
                    if (data.PersistentData is ItemPersistentData itemData)
                    {
                        object tune = 0;
                        itemData.TryGetPersistentData<TuneableComponent>(out tune);

                        Log.WriteLine(new LocString("Data: " + tune));
                        itemData.SetPersistentData<TuneableComponent>(Tune);
                    }
                }*/
    }
}