# MinimalApi.MultiTenant Sample

Demonstrates multi-tenant GovernAI usage: tenant ID resolved from `X-Tenant-Id` header or `tenant_id` JWT claim.

## How to Run

```bash
cd samples/MinimalApi.MultiTenant
dotnet run
```

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/api/chat` | Send a prompt; tenant resolved from header or claim |
| `GET` | `/api/events` | View captured events — each includes `tenantId` |

## Sample Requests

### Chat as tenant-a (via header)

```bash
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: tenant-a" \
  -d '{"prompt": "Summarize our quarterly results."}'
```

**Expected response:**

```json
{
  "response": "[Tenant-aware] Fake AI response for: Summarize our quarterly results."
}
```

### View captured events (shows tenantId)

```bash
curl http://localhost:5000/api/events
```

**Expected:** events where `tenantId` = `"tenant-a"` for the request above.

### Chat without tenant (resolves to null)

```bash
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{"prompt": "Hello"}'
```

## Tenant Resolution Priority

1. `X-Tenant-Id` request header (highest priority)
2. `tenant_id` JWT claim
3. `tid` JWT claim
4. `null` (not resolved)

## What This Sample Shows

- `HttpContextTenantResolver` automatically resolves tenant from header or claims
- Each `GovernAIEvent.TenantId` is populated without manual wiring
- Multi-tenant isolation at the event level
