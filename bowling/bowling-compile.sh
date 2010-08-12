#!/bin/bash
mono $MONO_OPTIONS /opt/fsharp/bin/fsc.exe \
     -r:/opt/nunit/bin/net-2.0/nunit.framework.dll \
     -r:/opt/fsharpPowerPack/bin/FSharp.PowerPack.Compatibility.dll \
     bowling.fs