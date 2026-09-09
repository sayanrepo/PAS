Run from the repository root:

```powershell
dotnet run --project tests/AccountChecks/AccountChecks.csproj -p:OutputPath=bin/AccountChecks/
```

These checks use in-memory HTTP and browser-storage substitutes. They check credential validation,
API error handling, authenticated updates, restored profile details, image constraints and
filename ownership. They do not connect to or modify the Panta database.
