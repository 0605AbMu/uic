# C# ASN.1 UserIdentity Certificate (UIC)

This repository demonstrates encoding and decoding a `UserIdentity` object into ASN.1 DER format and PEM representation
using .NET 8.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later installed.
- Git CLI installed.

---

## Getting Started

1. **Clone the repository**

```bash
git clone https://github.com/0605AbMu/uic.git
cd uic
````

2. **Build and run the console example**

```bash
dotnet run --project uic/uic.csproj
```

**Expected Output:**

```
Process started 12/11/2025 14:57:40.
Pem Result:

-----BEGIN USER IDENTITY-----
MC0CASoMCEFzeW5jUHJvFg9kZXZAZXhhbXBsZS5jb20wDQwEcmVhZAwFd3JpdGU=
-----END USER IDENTITY-----

Encoding finished: ************/UIC/UIC/bin/Debug/net8.0/temp/test.pem
Parsed user: {"UserId":42,"Username":"AsyncPro","Email":"dev@example.com","Permissions":["read","write"]}
Process finished 12/11/2025 14:57:40.
```

3. **Run Unit Tests**

```bash
dotnet test
```

---

## Features

* Encode `UserIdentity` into **ASN.1 DER** format.
* Convert DER to **PEM** format.
* Decode PEM back to `UserIdentity`.
* Validate data during encoding and decoding.
* Includes unit tests for all core functionality.

---

**That's all. Thanks!**