# Vehicle Tuning Eco
A simple mod that allows players to tweak and customize their vehicles.

## Tuning Bench
The tuning bench is where Vehicle Handlers are able to alter vehicles and tune their stats.
### Stats
The currently configurable stats are as follows:
|Stat| Description | 
|--|--|
| Max Speed | The top speed of the vehicle. Higher is better.
| Fuel Consumption  | The fuel consumption (in joules per hour). Lower is better. |
| Co2 Emissions | The pollution (in PPM) of the vehicle. Lower is better. |
| Storage Capacity | How many kg's of weight the vehicle can carry.  Also increases the number of slots available based on the kg per slot of the base vehicle. Higher is better.|
| Durability | How quickly the parts of the vehicle degrade over time, Lower is better. |
 |Offroading | How much the vehicle is influenced by the ground beneath it. Lower values reduce the speed increase of road surfaces, but increases the speed while on dirt. |


### Menus
This lets them access a small customization screen where they can increase or decrease the "tune levels" for each stat type using points. For example, they can spend 1 point to increase the top speed of the vehicle. Points can gained through vehicle handling and negative tune levels.

All stats are on a range of -10 to 10, starting at 0. Negative values worsen the stat, however give a point to the tuner to re-invest in another stat. After reaching a value of 6,  the cost per tune value increases to 2 points.
To max out a stat to 10 requires 14 points.
### Drags
All stats come with a given "drag", this pulls back on other stats slightly when you level it up. For example, increasing the value of Max Speed will slightly increase Fuel Consumption and Emissions.


## Vehicle Handling
Vehicle Handling is a new specialty for the scientist. The skill allows for a reduced calorie cost when driving and a increase in the number of points available to tune with.
Vehicle Handling also has one talent, where the player can choose between 6 bonus skillpoints or 25% calorie reduction when driving.

## Configurability
All of the values in the mod are fully configurable in the MechanicsExpansion.eco config.

**!!! All configs must be done while the server is offline, as otherwise they will be removed when the server stops. !!!**

### Drag Config
Drags are configured as a 2D array, the outer array determines what needs to be raised, while the inner array determines what is being raised. 
The drag values are in levels, with 0.5 being half a point.

The indexing is as follows: 
|Index| Name |
|--|--|
| 0 | Max Speed |
| 1 | Fuel Consumption |
| 2 | Co2 Emissions |
| 3 | Storage Capacity |
| 4 | Durability |
| 5 | Offroading |

For the below code, replace the values with the drags desired and repeat for all stats.

    "drags" : [[<Max Speed>, <Fuel Consumption>, <Co2 Emissions>, <Storage>, <Durability>, <Offroading>] <- These are the drags for max speed]
    
### Vehicle Config
Each vehicle has config of "weights" which correspond to each stat, E.G: "max_speed_weights" is for Max Speed.
Each weight contains 3 values: a "inital_value", a "lower" value and an "upper" value.
These determine the values that users can tune to.
### Initial Value
The inital_value is what untuned vehicles default to and is equivalent to a tune level of 0.

### Lower Value
The lower value is how much below the inital_value the stat can go. On an inital_value of 10 and a lower of 7, the lowest the stat can ever go is 3. This could be achieved by a user by putting in a tune level of -10.

### Upper Value
The upper value is the same as the lower value, but for when the user increases the tune level.

### Flipped Stats
Some stats are "flipped" due to them being better to keep low, the flipped stats are: Fuel Consumption, Co2 Emissions and Durability.
These stats use the "lower" value when increasing the tune levels. 

### Examples
Example 1:

    "max_speed_weights": {  
	  "initial_value": 18.0,  // All vehicles of this type will have a speed of '18'
	  "lower": 4.0, // Players can lower the speed down to 18 - 4 = 14   
	  "upper": 3.0   // Players can raise the speed up to 18 + 3 = 21
	}
Example 2:

	"fuel_consumption_weights": {  
	  "initial_value": 300.0, // Vehicles of this type start with 300
	  // Due to fuel consumption being flipped, lower is a buff for the vehicle
	  "lower": 100.0, // Players can lower it down to 200
	  "upper": 150.0  // Players can raise it up to 450
	}

 ## Compatability
 Due to the mod running in a .dll, all vanilla vehicles that can be tuned must be "deleted" from the UserCode folder. 
 This is implemented by a series of .override.cs files that only contain a namespace and no other data. If another mod tries to add back a vanilla vehicle or override it in any way, 
 its very likely a "duplicate class" error will be thrown when booting up. This mod will not be compatible with Vehicle Tuning and you must either remove it or the vehicle tuning.

## Developer Integration
If you have a vehicle mod and would like to make it compatible with vehicle tuning, follow the below steps.

1. Reference the MechanicsExpansion.dll file in your project.
2. Make your vehicle(s) require the TuneableComponent class as shown below
   <img width="736" height="153" alt="image" src="https://github.com/user-attachments/assets/3c37043f-51af-48d9-a7d2-3f1f3b5eb035" />

4. Initalize the TuneableComponent class prior to initalising the VehicleComponent class. Pass in any matching values into the instantiate method, they should be the same as the vehicle component instantiation. You also need to pass in the HumanPowered value here, or 0 if there is none.
  <img width="1147" height="428" alt="image" src="https://github.com/user-attachments/assets/50c7b4ad-bce4-4d8b-a5ef-5b93b4f4de1c" />
5. Create a IInitializablePlugin class and call TuneManager.AddVehicle<YOUR_VEHICLE_CLASS>(), along with any default tunes represented by TuneDataTemplates.
<img width="804" height="242" alt="image" src="https://github.com/user-attachments/assets/e411e895-b134-4554-ad33-a31840cc5d04" />

If you have a vehicle that uses VehicleToolComponent for storage, you can pass in the extra arguments into the param args section of the initalize function, such as:

    this.GetComponent<TuneableComponent>().Initialize( 1, 12, 2500000, 2, 100, 200, 0, true, VehicleUtilities.GetInventoryRestriction(this));
