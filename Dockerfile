FROM dotnet
COPY bin/Release/netcoreapp2.2/linux-x64/publish/ /publish
WORKDIR /publish
EXPOSE 23333
CMD [ "./mir-remake-ios-server" ]