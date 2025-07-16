# All publishing and Unity-specific steps have been removed.

echo "--- Cleaning old build artifacts ---"
Remove-Item Frent\bin\Release\* -Recurse -ErrorAction SilentlyContinue

echo "--- Building Source Generator ---"
dotnet build -c Release Frent.Generator\Frent.Generator.csproj

echo "--- Copying Source Generator to main project ---"
Copy-Item -Path ".\Frent.Generator\bin\Release\netstandard2.0\Frent.Generator.dll" -Destination ".\Frent\bin\Release\"

echo "--- Building Frent Library ---"
dotnet build -c Release Frent\Frent.csproj

echo ""
echo "--- BUILD COMPLETE! ---"
echo "Your custom Frent.dll is located in: .\Frent\bin\Release\"