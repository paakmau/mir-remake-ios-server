FROM microsoft/dotnet
COPY bin/Release/netcoreapp2.2/linux-x64/publish/ /publish
WORKDIR /publish
EXPOSE 23333
CMD [ "dotnet", "mir-remake-ios-server" ]