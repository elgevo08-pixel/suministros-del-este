# 🏗️ Suministros del Este — ERP

[![CI](https://github.com/TU_USUARIO/suministros-del-este/actions/workflows/ci.yml/badge.svg)](https://github.com/TU_USUARIO/suministros-del-este/actions/workflows/ci.yml)
[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app/template/suministros-del-este)

Sistema de gestión integral para distribuidora de materiales de construcción **Suministros del Este S.R.L.**

> 🌐 **Demo en vivo:** `https://TU_APP.railway.app/swagger`

---

## Arquitectura Hexagonal (Ports & Adapters) + DDD

```
┌──────────────────────────────────────────────────────────────┐
│              API REST  (Primary Adapter)                     │
│           MaterialesController  [Authorize]                  │
└────────────────────────┬─────────────────────────────────────┘
                         │ usa IRegistrarMaterialUseCase
                         │     IConsultarInventarioUseCase
┌────────────────────────▼─────────────────────────────────────┐
│           APPLICATION  (Use Cases / Orquestación)            │
│  RegistrarMaterialHandler  ·  ConsultarInventarioHandler     │
│  MaterialFactory (Factory) ·  StockMinimoAlertStrategy (Strategy)│
└────────────────────────┬─────────────────────────────────────┘
                         │ usa IMaterialRepository (puerto)
                         │     IEventPublisher (puerto)
┌────────────────────────▼─────────────────────────────────────┐
│              DOMAIN  (Núcleo — sin dependencias)             │
│  Material (Aggregate Root)                                   │
│  NombreMaterial · Stock · Precio  (Value Objects)           │
│  MaterialRegistradoEvent · StockBajoMinimoEvent (Events)    │
└────────────────────────▲─────────────────────────────────────┘
                         │ implementa los puertos
┌────────────────────────┴─────────────────────────────────────┐
│          INFRASTRUCTURE  (Secondary Adapters)                │
│  MaterialRepository (EF Core / SQLite / SQL Server)          │
│  InMemoryEventPublisher · KeycloakAuthExtensions             │
└──────────────────────────────────────────────────────────────┘
```

## Patrones y Principios

| Categoría | Patrón / Principio | Ubicación |
|-----------|-------------------|-----------|
| DDD | Aggregate Root | `Domain/Aggregates/Material/Material.cs` |
| DDD | Value Objects (3) | `Domain/Aggregates/Material/ValueObjects/` |
| DDD | Domain Events (2) | `Domain/Events/` |
| Diseño | Factory | `Application/Factories/MaterialFactory.cs` |
| Diseño | Strategy | `Application/Strategies/IStockAlertStrategy.cs` |
| Diseño | Observer | Domain Events + `InMemoryEventPublisher` |
| Diseño | Repository | `Infrastructure/Persistence/Repositories/` |
| SOLID | SRP / OCP / ISP / DIP | Aplicado en todas las capas |
| Seguridad | OAuth2/JWT | Keycloak + `KeycloakAuthExtensions` |
| DevOps | Docker | `docker-compose.yml` + `Dockerfile` |
| CI/CD | GitHub Actions | `.github/workflows/ci.yml` |
| Calidad | ArchUnit | `tests/SuministrosDelEste.ArchTests/` |
| IA | CLAUDE.md / AGENTS.md | Raíz del repositorio |

## 🚀 Inicio Rápido

### Opción A — Demo en Railway.app (recomendado para entrega)

```bash
git clone https://github.com/TU_USUARIO/suministros-del-este.git
# Seguir: docs/guia-despliegue.md
```

### Opción B — Local completo con Docker

```bash
docker-compose up -d
# API: http://localhost:5000/swagger
# Keycloak Admin: http://localhost:8080 (admin/admin123)
```

### Opción C — Local sin Docker

```bash
# Prerrequisito: .NET 8 SDK
dotnet build
dotnet ef database update \
  --project src/SuministrosDelEste.Infrastructure \
  --startup-project src/SuministrosDelEste.API
dotnet run --project src/SuministrosDelEste.API
```

## 📡 Endpoints REST

| Método | Ruta | Rol | Descripción |
|--------|------|-----|-------------|
| `GET` | `/api/v1/materiales` | Autenticado | Catálogo de materiales |
| `GET` | `/api/v1/materiales/alertas` | Autenticado | Alertas de reabastecimiento |
| `POST` | `/api/v1/materiales` | `inventario-admin` | Registrar material |
| `POST` | `/api/v1/descuentos/calcular` | Autenticado | Calcular descuento (Strategy) |

## 🎓 Tarea 4 — SOLID + UI/UX (Ingeniería de Software 2)

**Backend (Tema 1 — SOLID):** función nueva de cálculo de descuentos, construida a propósito
con los 5 olores de código y refactorizada con SOLID. El "antes" (God Class con SRP/OCP/ISP/DIP
violados + subclase con LSP violado) vive en `docs/solid-antes-despues/antes/`, fuera de la
compilación. El "después" es código real e integrado:
- `src/SuministrosDelEste.Application/Strategies/IDescuentoStrategy.cs` + 3 estrategias (OCP, ISP, LSP)
- `src/SuministrosDelEste.Application/UseCases/CalcularDescuento/` (SRP, DIP)
- `src/SuministrosDelEste.Domain/Events/DescuentoAplicadoEvent.cs` — reutiliza `IEventPublisher`
- `src/SuministrosDelEste.API/Controllers/DescuentosController.cs`

**Frontend (Tema 2 — UI/UX):** `frontend/dashboard-v1.html` (antes) vs. `frontend/dashboard-v2.html`
(después), con 8 técnicas profesionales — 2 por encima del mínimo de 6:
Wizard por pasos · Modales · Menú superior (MenuStrip) · Ocultación por rol · Sidebar colapsable ·
Paleta documentada · DataGrid con orden/paginación · Estados de carga + toasts.

Evidencia completa (antes/después con capturas y justificación) en el documento técnico entregado
junto a esta tarea.

## 🧪 Tests de Arquitectura

```bash
dotnet test tests/SuministrosDelEste.ArchTests/
```

Validan automáticamente que ninguna capa viole los límites de la Arquitectura Hexagonal.

## 📁 Documentación

| Archivo | Contenido |
|---------|-----------|
| `CLAUDE.md` | Contexto del proyecto para Claude AI |
| `AGENTS.md` | Guía para agentes de IA autónomos |
| `docs/evidencia-ia.md` | Prompts y metodología de IA usada |
| `docs/guia-despliegue.md` | Pasos para publicar en Railway.app |
| `docs/guion-video.md` | Script para el video de presentación |

---

*Empresa: Distribuidora Suministros del Este S.R.L. — Santo Domingo, RD*
