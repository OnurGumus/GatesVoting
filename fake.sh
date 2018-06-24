#!/usr/bin/env bash

set -eu
set -o pipefail

TOOL_PATH=.fake

if ! [ -e $TOOL_PATH/fake ] 
then
  dotnet tool install fake-cli --tool-path $TOOL_PATH --version 5.*
fi
$TOOL_PATH/fake "$@"