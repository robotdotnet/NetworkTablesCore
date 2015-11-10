# NetworkTablesCore
[![Build status](https://ci.appveyor.com/api/projects/status/q6e3jxtlavkpuf3p/branch/master?svg=true)](https://ci.appveyor.com/project/robotdotnet/networktablescore/branch/master) [![codecov.io](https://codecov.io/github/robotdotnet/NetworkTablesCore/coverage.svg?branch=master)](https://codecov.io/github/robotdotnet/NetworkTablesCore?branch=master)

This is a DotNet implementation of NetworkTables, using the new ntcore native library for the 2016 season. The managed side of the code is based on the Java implentation, however all communication code is provided by the library. NetworkTables are used to pass non-Driver Station data to and from the robot across the network.

The program uses C# 6.0 features, and comes precompiled for .NET 4.5. However, as long as you have VS 2015, it will compile down to .NET 3.5 with only minor changes. 

Currently support Windows 32 and 64 bit, Linux 32 and 64 bit, and the RoboRIO. The native library should be able to be compiled on Mac, however I do not own a Mac, and have no way to test it. If the native library is built for Mac OS, and the loading code is changed to load the right library. Arm support other then the RoboRIO should be easy to enable, and should happen soon.


Please note that NetworkTables is a protocol used for robot communication in the FIRST Robotics Competition, and can be used to talk to SmartDashboard/SFX. It does not have any security, and should never be used on untrusted networks.

Documentation
-------------
API documentation can be found [Here](http://robotdotnet.github.io/Documentation/API/html/G_NetworkTables.htm). The primary API is the NetworkTables class, which examples for its use can be found [Here](http://robotdotnet.github.io/Documentation/API/html/T_NetworkTables_NetworkTable.htm).

          
Supported Platforms
-------------------
* Windows 32 Bit
* Windows 64 Bit
* Linux 32 Bit (Needs Tested)
* Linux 64 Bit (Needs Tested)
* RoboRIO
 
Future Supported Platforms
--------------------------
* Mac OSX 32 Bit
* Mac OSX 64 Bit
* Arm Linux 32 Bit (other then RoboRIO)
* Arm Linux 64 Bit (other then RoboRIO)

Installation
------------
When you create a WPILib robot project using our Visual Studio extension, this will automatically be installed.

For creating your own desktop projects, the project can be found on [NuGet](https://www.nuget.org/packages/FRC.NetworkTables). 

If you need to support a platform other then the currently supported platforms, you can do 2 things. 
* Compile the native library for the necessary system. You will then have to change the library loader to load this new dll.
* Use the old [NetworkTablesDotNet](https://github.com/robotdotnet/NetworkTablesDotNet). This is written entirely in C# code. However this will not be updated, and will not support the NetworkTables 3.0 protocol unlesss someone wishes to do so.
