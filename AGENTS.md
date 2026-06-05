# AGENTS.md — Guía para Agentes de IA

> Este archivo guía a agentes de IA (Claude Code, Copilot, Cursor, etc.)
> al trabajar de forma autónoma en este repositorio.

---

## Contexto del Sistema

**Suministros del Este ERP** — sistema de gestión para distribuidora de materiales de construcción.
Arquitectura: Hexagonal + DDD Táctico + SOLID. Stack: .NET 8, C#, SQL Server 2022.

---

## Regla #1: Verificar Arquitectura Antes de Modificar

Antes de cualquier cambio de código, el agente DEBE identificar en qué capa ocurre:

| Capa | Namespace | Puede depender de |
|------|-----------|-------------------|
| Domain | `SuministrosDelEste.Domain` | Nada externo |
| Application | `SuministrosDelEste.Application` | Solo Domain |
| Infrastructure | `SuministrosDelEste.Infrastructure` | Domain + Application |
| API | `SuministrosDelEste.API` | Application + Infrastructure |

**Violar estas reglas romperá los ArchTests — el agente DEBE corregirlo antes de finalizar.**

---

## Regla #2: Ejecutar ArchTests Tras Cambios Estructurales

```bash
dotnet test tests/SuministrosDelEste.ArchTests/
```

Si algún test falla, la tarea NO está completa.

---

## Flujo para Agregar una Nueva Entidad de Dominio

```
1. Domain/Aggregates/<Entidad>/<Entidad>.cs          ← Aggregate Root
2. Domain/Aggregates/<Entidad>/ValueObjects/          ← Value Objects
3. Domain/Events/<Entidad>*Event.cs                  ← Domain Events
4. Domain/Ports/I<Entidad>Repository.cs              ← Outbound Port
5. Application/UseCases/<Caso>/<Caso>Command.cs      ← Command
6. Application/UseCases/<Caso>/<Caso>Handler.cs      ← Handler (implements Inbound Port)
7. Application/Ports/I<Caso>UseCase.cs               ← Inbound Port
8. Application/DTOs/<Entidad>Dto.cs                  ← DTO
9. Infrastructure/Persistence/Repositories/           ← Implementa Outbound Port
10. API/Controllers/<Entidad>Controller.cs            ← REST Adapter
11. Infrastructure/DependencyInjection/InfrastructureExtensions.cs ← Registrar
12. API/Program.cs                                   ← Registrar use cases
```

---

## Restricciones Absolutas para Agentes

- ❌ **NUNCA** importar namespaces de Infrastructure en Domain o Application.
- ❌ **NUNCA** reescribir una clase completa para un cambio puntual.
- ❌ **NUNCA** agregar DELETE físico en repositorios. Usar `Desactivar()` del Aggregate.
- ❌ **NUNCA** instanciar Aggregates con `new Material()` directamente. Usar `Material.Crear(...)`.
- ✅ **SIEMPRE** documentar con XML summary los miembros nuevos.
- ✅ **SIEMPRE** incluir `try-catch` en repositorios y handlers.
- ✅ **SIEMPRE** validar en Value Objects usando `DomainException`.

---

## Verificación Final del Agente

Antes de reportar una tarea como completa, ejecutar:

```bash
dotnet build                                    # Sin errores de compilación
dotnet test tests/SuministrosDelEste.ArchTests/ # Todos los tests pasan
```
