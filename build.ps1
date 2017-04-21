cd src\StockportContentApi
dotnet restore
dotnet publish --configuration Release -o publish
cd ..\..
python zip.py src\StockportContentApi\publish package.zip
