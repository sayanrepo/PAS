From the repository root:

```powershell
dotnet run --project tests/OrderChecks -p:OutputPath=bin/OrderChecks/
```

Optional browser check using the actual web components with sample data:

```powershell
dotnet run --project tests/OrderChecks -p:OutputPath=bin/OrderChecks/ -- --preview
```

Open http://localhost:5089/documents/orders. The test host uses an isolated identity and
in-memory orders; it cannot connect to the real API or database. Check search, paging,
invalid dates, empty results, and each row's detail link. No preview route is added to the application.
