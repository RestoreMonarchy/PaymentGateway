FROM bitnami/dotnet-sdk:5 AS db-build-env
WORKDIR /db
# Copy and build database project
COPY /src/Database ./
RUN dotnet build Build/Database.Build.csproj --configuration Release -o out

FROM markhobson/sqlpackage:latest
ARG CONNECTION_STRING
ENV CONNECTION_STRING=$CONNECTION_STRING

WORKDIR /db
COPY --from=db-build-env /db/out .
ENTRYPOINT sqlpackage /a:Publish /tcs:"$CONNECTION_STRING" /sf:Database.Build.dacpac