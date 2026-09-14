  
NAudio Advanced IV is a plugin for ScriptHookDotNet 1.7.1.8 which uses NAudio to provide a full audio player. 
It's very easy to instantiate new NAudioAdvanced sounds and play or trigger them.

This library provides many advanced functions like loops, volume/panning control related to the sound position,
automatically updated for moving entities (vehicles, peds or objects) by calling TickHelper.ProcessAll() in a Tick
inside main script.


Current Version: 1.0.1

The current version is based on NAudio v. 2.2.1. You will find the required NAudio DLLs inside the package, 
according to the license (you can check License NAudio.md file).


How To Use
First you need to create a new sound using the sound name (with extension) and the path.

Example (in VB.NET):

  Public Shared Test As NAudioAdvanced
  Test = New NAudioAdvanced("test.wav", "scripts/mod")
 

To play the sound you can use these functions:

 Play(Volume, Loop, StartTime) -> Play "frontend".

 Play(Position, MaxDistance, Loop, StartTime, RealTimeVolumeControl) -> Play from a position in the game world.

 Play(Ped, MaxDistance, Loop, StartTime, RealTimeVolumeControl) -> Play from ped.

 Play(Object, MaxDistance, Loop, StartTime, RealTimeVolumeControl) -> Play from object.

 Play(Vehicle, MaxDistance, Loop, StartTime, RealTimeVolumeControl) -> Play from vehicle.

 

Otherwise you can trigger a sound "one-shot" using these functions:

 Trigger(Volume, Loop, StartTime) -> Play "fronted"

 Trigger(Position, MaxDistance, Loop, StartTime) -> Play from a position in the game world. 
						    The volume/panning will be set before triggering the sound, 
						    then they wont be changed.
 

Every sound have many properties, like BalanceLR (pan), Volume, PlaybackState and more. 
I added the main functions userful for modding, but if you need more options you can use the property "Advanced", 
which contains the original Reader, DirectSoundOut (OutputDevice) and WaveChannel32 from NAudio library.


The library provides also an automatic logging system, which will be write errors catched inside "NAudioAdvanced-IV.log" file.

 

Remember to add NAudio.dll and NativeExtended-IV.dll to project's reference and then put it inside GTA IV folder.


If you need help or you want to cooperate please contact me :)

