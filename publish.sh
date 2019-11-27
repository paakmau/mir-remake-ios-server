user="hebo"
# pass="gfyTQL233"
host="39.99.134.200"
src="bin/Release/netcoreapp2.2/linux-x64/publish/"
dest="~/"

dotnet publish -c Release -r linux-x64
cp Data/*.json $src/Data/
# scp -r $src $user@$host:$dest