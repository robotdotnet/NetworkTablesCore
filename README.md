# NetworkTablesDotNet
[![Build status](https://ci.appveyor.com/api/projects/status/q6e3jxtlavkpuf3p/branch/master?svg=true)](https://ci.appveyor.com/project/robotdotnet-admin/networktablescore/branch/master)

This is a DotNet implementation of NetworkTables, using the new ntcore native library for the 2016 season. The managed side of the code is based on the Java implentation, however all communication code is provided by the library. NetworkTables are used to pass non-Driver Station data to and from the robot across the network.

The program uses C# 6.0 features, and comes precompiled for .NET 4.5. However, as long as you have VS 2015, it will compile down to .NET 3.5 with only minor changes. 

Currently support Windows 32 and 64 bit, and the RoboRIO. The native library is not currently compiled for linux, but it probably could be without too much hastle.


Please note that NetworkTables is a protocol used for robot communication in the
          FIRST Robotics Competition, and can be used to talk to
          SmartDashboard/SFX. It does not have any security, and should never
          be used on untrusted networks.
          
Documentation
-------------
TODO

Installation
------------
The lastest version can be found on NuGet, for use with desktop applications. When you create a WPILib project for the RoboRIO, it automatically gets downloaded.
