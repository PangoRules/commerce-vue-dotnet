# Security Threat Model (Lite)

## Current Authentication Status: TODO

**No authentication or authorization is currently implemented.**

This document outlines security considerations for when auth is added.

## Threat Categories

### 1. Authentication (TODO)

| Threat | Status | Mitigation |
|--------|--------|------------|
| No user authentication | ⚠️ TODO | Implement JWT or session-based auth |
| No password policies | ⚠️ TODO | Enforce complexity, hashing (bcrypt/Argon2) |
| No MFA | ⚠️ TODO | Add TOTP or WebAuthn support |
| Session management | ⚠️ TODO | Secure cookies, token expiration |

**Recommended Approach**:
- JWT tokens for API authentication
- Refresh token rotation
- ASP.NET Core Identity or custom implementation

### 2. Authorization (TODO)

| Threat | Status | Mitigation |
|--------|--------|------------|
| No role-based access | ⚠️ TODO | Implement RBAC (Admin, User, Guest) |
| No resource ownership | ⚠️ TODO | Verify user owns resources before access |
| API endpoint protection | ⚠️ TODO | [Authorize] attributes on controllers |

### 3. Secrets Management

| Secret | Current State | Recommendation |
|--------|---------------|----------------|
| Database password | `.env` file (not committed) | ✓ Acceptable for dev |
| MinIO credentials | `.env` file | ✓ Acceptable for dev |
| JWT signing key | Not implemented | Use env var, rotate periodically |
| API keys | None yet | Use secret manager in production |

**Production Recommendations**:
- Use secret management (Vault, AWS Secrets Manager, Azure Key Vault)
- Never commit secrets to git
- Rotate credentials periodically
- Audit secret access

### 4. Storage Security (MinIO)

| Aspect | Current State | Notes |
|--------|---------------|-------|
| Bucket policy | Public download | ✓ Intentional for product images |
| Upload authentication | Via backend only | ✓ Users can't upload directly |
| HTTPS | Disabled (dev) | ⚠️ Enable in production |
| Access logging | Not configured | TODO: Enable for audit |

**Current Configuration**:
```bash
# minio-init sets public download
mc anonymous set download local/products
```

This is appropriate for public product images. Private files should use a separate bucket with authentication.

### 5. API Security

| Threat | Status | Mitigation |
|--------|--------|------------|
| CORS | Configured | ✓ Restricted to frontend origin |
| Rate limiting | ⚠️ Missing | TODO: Add rate limiting middleware |
| Input validation | ✓ FluentValidation | Validates all input DTOs |
| SQL injection | ✓ Protected | EF Core parameterized queries |
| XSS | ✓ Vue escapes by default | Don't use v-html with user content |
| CSRF | ⚠️ TODO | Add anti-forgery tokens when auth added |

### 6. Data Protection

| Data | Classification | Protection |
|------|----------------|------------|
| Product info | Public | None needed |
| User passwords | Confidential | Hash with bcrypt (TODO) |
| Order details | Private | Auth required (TODO) |
| Payment info | Sensitive | Never store raw; use payment processor |

### 7. Infrastructure Security

| Aspect | Current (Dev) | Production Recommendation |
|--------|---------------|---------------------------|
| HTTPS | Not enforced | Enforce everywhere |
| Network isolation | Docker bridge | Kubernetes network policies |
| Container security | Default | Read-only filesystems, non-root |
| Database access | Direct port | VPC/private network only |

## OWASP Top 10 Checklist

| # | Risk | Status |
|---|------|--------|
| A01 | Broken Access Control | ⚠️ No auth implemented |
| A02 | Cryptographic Failures | TODO: Verify when auth added |
| A03 | Injection | ✓ EF Core protects against SQL injection |
| A04 | Insecure Design | ✓ Layered architecture helps |
| A05 | Security Misconfiguration | ⚠️ Review before production |
| A06 | Vulnerable Components | TODO: Regular dependency updates |
| A07 | Auth Failures | ⚠️ No auth implemented |
| A08 | Data Integrity Failures | TODO: Verify updates are secure |
| A09 | Logging Failures | ⚠️ Minimal logging currently |
| A10 | SSRF | TODO: Validate if external URLs processed |

## Security TODO Priority

### High Priority (Before Production)
1. Implement authentication (JWT)
2. Add authorization (role-based)
3. Enable HTTPS everywhere
4. Add rate limiting
5. Implement proper logging

### Medium Priority
1. Dependency vulnerability scanning
2. CSRF protection
3. Security headers (CSP, HSTS, etc.)
4. Input sanitization review

### Low Priority (Ongoing)
1. Penetration testing
2. Security training
3. Incident response plan
4. Regular security audits

## Security Headers (TODO)

Add to backend middleware:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

## Dependency Security

```bash
# Check .NET vulnerabilities
dotnet list package --vulnerable

# Check npm vulnerabilities
cd frontend && npm audit
```

Run these regularly and before releases.
