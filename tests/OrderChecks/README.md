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

The same isolated preview also covers the unified order form:

- /documents/orders/new: create with all three panel sections.
- /documents/orders/1: editable pro forma order.
- /documents/orders/2/edit: editable in-progress order.
- /documents/orders/3: read-only production order.

Preview saves stay in memory. Automated checks cover the status policy, shared
routes, legacy totals, agreed prices, forged prices, foreign item IDs, and stale
status rejection without connecting to a database.
