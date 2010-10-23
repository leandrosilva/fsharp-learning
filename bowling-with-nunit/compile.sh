#!/bin/bash
mono $MONO_OPTIONS /opt/fsharp/bin/fsc.exe \
     -r:nunit.framework.dll \
     -r:/opt/fsharpPowerPack/bin/FSharp.PowerPack.Compatibility.dll \
     bowling.fs