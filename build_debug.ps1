# All publishing and Unity-specific steps have been removed.

echo "--- Cleaning old build artifacts ---"
# It's good practice to also clean the Debug folder
Remove-Item Frent\bin\Debug\* -Recurse -ErrorAction SilentlyContinue

echo "--- Building Source Generator ---"
# Change -c Release to -c Debug
dotnet build -c Debug Frent.Generator\Frent.Generator.csproj

echo "--- Copying Source Generator to main project ---"
# You might need to adjust the source and destination paths if they are configuration-specific
Copy-Item -Path ".\Frent.Generator\bin\Debug\netstandard2.0\Frent.Generator.dll" -Destination ".\Frent\bin\Debug\"

echo "--- Building Frent Library ---"
# Change -c Release to -c Debug
dotnet build -c Debug Frent\Frent.csproj

echo ""
echo "--- BUILD COMPLETE! ---"
echo "Your custom Frent.dll is now located in: .\Frent\bin\Debug\"