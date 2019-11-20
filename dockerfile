FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as BUILD
COPY ./src ./buildfolder
WORKDIR /buildfolder/
#RUN dotnet test Oppsyn.sln
RUN dotnet test Oppsyn.sln --filter TestCategory=Unit
RUN dotnet publish -c Release -o /buildfolder/output/oppsyn Oppsyn/Oppsyn.csproj


FROM mcr.microsoft.com/dotnet/core/runtime:3.0 as RUNTIME
COPY --from=BUILD /buildfolder/output ./app
WORKDIR /app/oppsyn/
ENTRYPOINT ["dotnet", "Oppsyn.dll"]
