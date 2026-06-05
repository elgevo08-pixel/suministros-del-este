# CLAUDE.md — Contexto del Proyecto para Claude AI

> Este archivo es leído automáticamente por Claude al trabajar en este repositorio.
> Actualizar cuando cambie la arquitectura, reglas o estado del proyecto.

---

## Sistema

**ERP Suministros del Este S.R.L.** — Distribuidora de materiales de construcción (cemento, varillas, agregados).

**Stack:** .NET 8, C#, SQL Server 2022, Entity Framework Core 8, Keycloak 24, Docker.

**Arquitectura:** Hexagonal (Ports & Adapters) + DDD Táctico + SOLID.

---

## Capas y Dependencias

```
Domain       → Núcleo puro. SIN dependencias externas.
Application  → Casos de uso. Depende SOLO de Domain.
Infrastructure → Adaptadores. Depende de Domain + Application.
API          → Presentación REST. Depende de Application + Infrastructure.
```

**Regla de oro:** el código que agrega nunca puede romper los ArchTests.

---

## Reglas de Codificación Activas

1. **Surgical modification**: modificar SOLO el bloque afectado. Nunca reescribir clases completas.
2. **Tipado estricto**: no usar `var` donde el tipo no sea evidente en la misma línea.
3. **try-catch obligatorio** en todo repositorio y handler de casos de uso.
4. **XML summary** en toda propiedad y método nuevo.
5. **No añadir paquetes NuGet** sin autorización explícita del desarrollador.
6. **Eliminación lógica**: `IsActive = false`. Nunca `DELETE` físico en BD.

---

## Patrones en Uso

| Patrón | Dónde |
|--------|-------|
| Aggregate Root | `Domain/Aggregates/Material/Material.cs` |
| Value Objects | `Domain/Aggregates/Material/ValueObjects/` |
| Domain Events | `Domain/Events/` |
| Factory | `Application/Factories/MaterialFactory.cs` |
| Strategy | `Application/Strategies/IStockAlertStrategy.cs` |
| Observer | Domain Events + `Infrastructure/Events/InMemoryEventPublisher.cs` |
| Repository | `Infrastructure/Persistence/Repositories/MaterialRepository.cs` |

---

## Comandos Frecuentes

```bash
# Levantar ambiente completo (SQL Server + Keycloak + API)
docker-compose up -d

# Crear migración EF Core
dotnet ef migrations add <NombreMigracion> \
  --project src/SuministrosDelEste.Infrastructure \
  --startup-project src/SuministrosDelEste.API

# Aplicar migraciones
dotnet ef database update \
  --project src/SuministrosDelEste.Infrastructure \
  --startup-project src/SuministrosDelEste.API

# Tests de arquitectura (deben pasar siempre)
dotnet test tests/SuministrosDelEste.ArchTests/

# Compilar toda la solución
dotnet build

# Ejecutar API localmente
dotnet run --project src/SuministrosDelEste.API
```

---

## Estado del Proyecto

### Fase 1 — Módulo Inventario ✅
- Arquitectura Hexagonal completa
- DDD: Aggregate Root `Material`, Value Objects, Domain Events
- Seguridad JWT/Keycloak configurada
- Docker + docker-compose funcional
- ArchUnit con 7 reglas de validación

### Fase 2 — Pendiente
- Módulo Clientes y Ventas
- Módulo Proveedores y Compras
- Módulo Despacho

---

## Prompt Tipo para Claude

> "Siguiendo la arquitectura hexagonal del proyecto, agrega el caso de uso `[X]`.
> Aplica el patrón `[Factory/Strategy/Observer]` donde corresponda.
> Respeta las reglas de surgical-csharp-modification.
> Los ArchTests deben seguir pasando."
