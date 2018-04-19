FROM microsoft/aspnetcore:1.1
WORKDIR /app
EXPOSE 5000
COPY ./publish .
ENTRYPOINT ["dotnet", "HistorianService.dll"]