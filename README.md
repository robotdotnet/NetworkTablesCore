**Build Status**

| Windows                 |  Linux                  | Code Coverage         |
| ------------------------|-------------------------|-----------------------|
| [![Build status][1]][2] | [![Build Status][3]][4] | [![codecov.io][5]][6] |

[1]: https://ci.appveyor.com/api/projects/status/q6e3jxtlavkpuf3p/branch/master?svg=true
[2]: https://ci.appveyor.com/project/robotdotnet/networktablescore/branch/master
[3]: https://travis-ci.org/robotdotnet/NetworkTablesCore.svg?branch=master
[4]: https://travis-ci.org/robotdotnet/NetworkTablesCore
[5]: https://codecov.io/github/robotdotnet/NetworkTablesCore/coverage.svg?branch=master
[6]: https://codecov.io/github/robotdotnet/NetworkTablesCore?branch=master

This is a DotNet implementation of NetworkTables, using the new ntcore native library for the 2016 season. The managed side of the code is based on the Java implentation, however all communication code is provided by the library. NetworkTables are used to pass non-Driver Station data to and from the robot across the network.

The program uses C# 6.0 features, and comes precompiled for .NET 4.5. However, as long as you have VS 2015, it will compile down to .NET 3.5 with only minor changes. 

Currently supports Windows, Mac OS X, and Linux in both 32 and 64 bit configurations, and Arm v6 and v7 support. Android is not currently supported, but could be with the proper native builds.


Please note that NetworkTables is a protocol used for robot communication in the FIRST Robotics Competition, and can be used to talk to SmartDashboard/SFX. It does not have any security, and should never be used on untrusted networks.

Documentation
-------------
API documentation can be found [here](http://robotdotnet.github.io/Documentation/API/html/G_NetworkTables.htm). The primary API is the NetworkTables class, which examples for its use can be found [here](http://robotdotnet.github.io/Documentation/API/html/T_NetworkTables_NetworkTable.htm).

          
Supported Platforms
-------------------
* Windows 32 Bit (CI Tested)
* Windows 64 Bit (CI Tested)
* Linux 32 Bit - Mono won't let you switch between 32 and 64 bit, so if you have a 64 bit system it will run in 64 bit mode.
* Linux 64 Bit (CI Tested)
* Mac OSX 32 Bit
* Mac OSX 64 Bit
* Arm v6 Hard Float (i.e Raspberry Pi 1)
* Arm v7 Hard Float (i.e BeagleBoneBlack)
* RoboRIO
 
Future Supported Platforms
--------------------------
* Android (Has issues with the Arm v7 Hard Float binary. Need a compiled native library)

Installation
------------
When you create a WPILib robot project using our Visual Studio extension, this will automatically be installed.

For creating your own desktop projects, the project can be found on [NuGet](https://www.nuget.org/packages/FRC.NetworkTables). 

If you need to support a platform other then the currently supported platforms, you can do 2 things. 
* Compile the native library for the necessary system. You will then have to change the library loader to load this new dll.
* Use the old [NetworkTablesDotNet](https://github.com/robotdotnet/NetworkTablesDotNet). This is written entirely in C# code. However this will not be updated, and will not support the NetworkTables 3.0 protocol unlesss someone wishes to do so.

License
=======
See [LICENSE.txt](LICENSE.txt)

Contributors
============

Thad House (@thadhouse)
