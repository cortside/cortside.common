#!/bin/bash
set -e

PROJECT=$1
VERSION=$2
NUGET=../../../bin/nuget.exe

if [ "$OS" = "Windows_NT" ]; then
   DNU=dnu.cmd
   NUGET=../../../bin/nuget.exe
else
   DNU=dnu
   NUGET="mono ../../../bin/nuget.exe"
fi

echo "packaging...."
echo $PROJECT
echo $VERSION

cd $PROJECT
sed -i "s|1.0.0-\*|$VERSION-*|g" project.json
$DNU restore
$DNU build
$DNU pack
cd  bin/Debug
$NUGET push $PROJECT.$VERSION.nupkg -s https://www.myget.org/F/xxx/api/v2/package xxx
$NUGET push $PROJECT.$VERSION.symbols.nupkg -s https://www.myget.org/F/xxx/api/v2/package xxx

