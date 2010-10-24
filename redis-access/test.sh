#!/bin/bash

MONO_PATH=$MONO_PATH:/opt/redis-sharp/
export MONO_PATH

nunit-console redis_access_test.exe