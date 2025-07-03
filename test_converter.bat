@echo off
cd "JvsToNtkConverter\out"
JvsToNtkConverter.exe java2cs "..\..\input\Sample1.java" "..\..\output\Sample1.cs"
echo Conversion completed
pause 