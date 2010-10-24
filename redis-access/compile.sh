#!/bin/bash
mono $MONO_OPTIONS /opt/fsharp/bin/fsc.exe \
     -r:nunit.framework.dll \
     -r:/opt/redis-sharp/redis-sharp.dll \
     redis_access_test.fs