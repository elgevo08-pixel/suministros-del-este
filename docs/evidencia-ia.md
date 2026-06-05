# Evidencia de Programación Asistida por IA

**Proyecto:** ERP Suministros del Este S.R.L.
**Herramienta:** Claude Sonnet (claude.ai)
**Fecha:** Junio 2025

---

## 1. Archivos de Reglas para la IA

Se crearon dos archivos de contexto que guían a la IA en cada sesión de trabajo:

### `CLAUDE.md` (raíz del repositorio)
Archivo leído automáticamente por Claude AI al abrir el proyecto. Contiene:
- Stack tecnológico y arquitectura del sistema
- Reglas de codificación activas (surgical-modification, tipado estricto, XML comments)
- Patrones en uso y dónde están en el código
- Comandos frecuentes (EF migrations, ArchTests, Docker)
- Estado actual del proyecto por módulos

### `AGENTS.md` (raíz del repositorio)
Guía para agentes de IA autónomos (Claude Code, Copilot Workspace). Contiene:
- Tabla de capas y sus restricciones de dependencia
- Flujo obligatorio para agregar nuevas entidades de dominio (10 pasos)
- Restricciones absolutas (nunca importar Infrastructure en Domain, nunca DELETE físico, etc.)
- Verificación final antes de reportar tarea como completa

---

## 2. Prompts Utilizados Durante el Desarrollo

### PROMPT 1 — Generación del proyecto base (Tarea 1)

```
Crea un proyecto .NET 8 C# para el ERP de Suministros del Este S.R.L.,
una distribuidora de materiales de construcción (cemento, varillas, agregados).

Arquitectura en capas:
- SuministrosDelEste.Entities: entidad Material con campos MaterialId, Nombre,
  Descripcion, StockActual, StockMinimo, PrecioUnitario, FechaRegistro, IsActive
- SuministrosDelEste.Data: DbContext (EF Core + SQL Server), IMaterialRepository,
  MaterialRepository
- SuministrosDelEste.Business: IMaterialService, MaterialService
- SuministrosDelEste.UI: Console App con inyección de dependencias

Reglas:
- Eliminación lógica (IsActive=false), nunca DELETE físico
- decimal(18,4) para stock, decimal(18,2) para precio
- try-catch en todos los métodos de repositorio
- FechaRegistro asignado en el repositorio, no en la UI
- Generar .sln, .gitignore y README.md
```

**Resultado:** Proyecto completo en capas con consola funcional, 17 archivos.

---

### PROMPT 2 — Upgrade a Arquitectura Hexagonal + DDD (Tarea 2)

```
Transforma el proyecto actual a Arquitectura Hexagonal (Ports & Adapters) con DDD táctico.
Debe incluir todo lo siguiente:

1. Arquitectura Hexagonal: separar dominio, aplicación e infraestructura
2. Principios SOLID en clases reales del proyecto
3. DDD táctico: al menos un Aggregate, un Value Object y un Domain Event
4. Patrones de diseño: Factory, Strategy, Observer, Repository
5. Seguridad: autenticación con Keycloak / OAuth2 / JWT
6. Docker para levantar el sistema; ArchUnit para validar la arquitectura
7. Archivo CLAUDE.md / AGENTS.md y uso de prompts

Estructura requerida:
- SuministrosDelEste.Domain: Aggregate Root Material, Value Objects (NombreMaterial,
  Stock, Precio), Domain Events (MaterialRegistradoEvent, StockBajoMinimoEvent),
  Ports (IMaterialRepository)
- SuministrosDelEste.Application: Use Cases (RegistrarMaterial, ConsultarInventario),
  Factory (MaterialFactory), Strategy (IStockAlertStrategy), DTOs
- SuministrosDelEste.Infrastructure: EF Core con HasConversion para Value Objects,
  InMemoryEventPublisher, KeycloakAuthExtensions, InfrastructureExtensions
- SuministrosDelEste.API: MaterialesController con [Authorize], Program.cs, Dockerfile
- SuministrosDelEste.ArchTests: HexagonalArchitectureTests con TngTech.ArchUnitNET
- Raíz: docker-compose.yml (SQL Server + Keycloak + API), CLAUDE.md, AGENTS.md
```

**Resultado:** 46 archivos generados, arquitectura hexagonal completa.

---

### PROMPT 3 — Diseño del Aggregate Root con Value Objects

```
Para la entidad Material como Aggregate Root, necesito:

1. Value Objects con validación en factory method Crear():
   - NombreMaterial: no nulo, máx 100 chars, trim automático, comparación insensible
   - Stock: no negativo, decimal(18,4), método EsMenorOIgualQue(Stock) y EsAgotado()
   - Precio: no negativo, decimal(18,2)

2. El Aggregate Root Material debe:
   - Constructor privado para EF Core
   - Factory Method estático Material.Crear(...)
   - Elevar MaterialRegistradoEvent en Crear()
   - Elevar StockBajoMinimoEvent si stock inicial <= mínimo
   - Método ActualizarStock(Stock) que también eleva evento si es crítico
   - Método Desactivar() con validación de idempotencia

3. EF Core: usar HasConversion para mapear Value Objects a primitivos en SQL
   sin romper las propiedades private set del Aggregate Root

Aplicar: DDD Value Object base class con igualdad por valor y operadores == y !=
```

**Resultado:** `Material.cs`, `NombreMaterial.cs`, `Stock.cs`, `Precio.cs`, `ValueObject.cs`

---

### PROMPT 4 — Implementación del patrón Strategy para alertas

```
Implementa el patrón Strategy para las alertas de reabastecimiento de stock:

Interfaz IStockAlertStrategy con:
- bool RequiereAlerta(Stock stockActual, Stock stockMinimo)
- string ObtenerMensaje(string nombreMaterial, Stock stockActual, Stock stockMinimo)

Implementación concreta StockMinimoAlertStrategy:
- Alerta cuando stockActual <= stockMinimo
- Mensaje diferenciado: "⛔ AGOTADO" o "⚠️ CRÍTICO" según si es cero o bajo
- Incluir cálculo del déficit en el mensaje

El ConsultarInventarioHandler debe recibir IStockAlertStrategy por inyección
para que sea intercambiable sin modificar el handler (principio OCP).
```

**Resultado:** `IStockAlertStrategy.cs`, `StockMinimoAlertStrategy.cs`, `ConsultarInventarioHandler.cs`

---

### PROMPT 5 — Configuración JWT y Keycloak

```
Crea la extensión KeycloakAuthExtensions para configurar autenticación OAuth2/JWT:

- Leer configuración desde sección "Jwt" de appsettings.json
  (Authority, Audience, RequireHttpsMetadata)
- Configurar JwtBearer con TokenValidationParameters:
  ValidateIssuer=true, ValidateAudience=true, ValidateLifetime=true
- Mapear roles de Keycloak: el claim de roles en Keycloak es "realm_access.roles"
- Agregar evento OnAuthenticationFailed para log de errores
- El controlador debe tener [Authorize] general y
  [Authorize(Roles = "inventario-admin")] en el POST de registro

También agregar DemoModeAuthorizationHandler: cuando Jwt:DemoMode=true,
auto-aprobar todas las peticiones (solo para demo público sin Keycloak).
```

**Resultado:** `KeycloakAuthExtensions.cs`, `JwtSettings.cs`, `DemoModeAuthorizationHandler.cs`

---

### PROMPT 6 — Tests de Arquitectura con ArchUnitNET

```
Crea los tests de arquitectura usando TngTech.ArchUnitNET v0.10.6 con xUnit:

Cargar los 4 ensamblados (Domain, Application, Infrastructure, API) con ArchLoader.
Validar estas reglas de la arquitectura hexagonal:
1. Domain no depende de Application
2. Domain no depende de Infrastructure
3. Domain no depende de API
4. Application no depende de Infrastructure
5. Application no depende de API
6. Value Objects en el namespace ...ValueObjects deben heredar de ValueObject base

Usar fluent API de ArchUnitNET:
  Types().That().ResideInNamespace(ns, true)
    .Should().NotDependOnAnyTypesThat()
    .ResideInNamespace(ns2, true)
    .Because("razón")
    .Check(Architecture)
```

**Resultado:** `HexagonalArchitectureTests.cs` con 7 reglas de validación automática

---

### PROMPT 7 — Despliegue y CI/CD

```
Agrega soporte para despliegue gratuito en Railway.app:

1. Soporte SQLite como alternativa a SQL Server para demo:
   - Detectar automáticamente por la cadena de conexión (Data Source= vs Server=)
   - appsettings.Production.json con SQLite y DemoMode=true

2. GitHub Actions (.github/workflows/ci.yml):
   - Trigger: push a main y pull_request
   - Jobs: restore, build Release, dotnet test ArchTests, docker build

3. railway.json con builder DOCKERFILE y política de restart

4. Dockerfile actualizado con directorio /app/data para SQLite y VOLUME

5. DemoModeAuthorizationHandler: cuando DemoMode=true, aprobar todos los 
   IAuthorizationRequirement automáticamente (bypass JWT para demo público)
```

**Resultado:** `railway.json`, `.github/workflows/ci.yml`, `DemoModeAuthorizationHandler.cs`

---

## 3. Metodología de Uso de IA

### Enfoque aplicado: "Surgical Modification"
Se instruyó a Claude a modificar **solo el bloque afectado** en cada prompt, nunca reescribir clases completas. Esto garantizó:
- Trazabilidad de cambios en git
- Menor riesgo de introducir regresiones
- Revisión rápida del código generado

### Validación post-generación
Cada bloque de código generado fue revisado para verificar:
- Namespaces correctos y consistentes
- Invariantes de dominio preservadas
- Reglas de arquitectura respetadas (confirmadas con ArchTests)

### Lo que NO generó la IA
- La estructura de base de datos (diseñada previamente)
- El modelo de negocio (proporcionado como contexto)
- Las reglas de validación de negocio (definidas por el desarrollador)
- La decisión de usar Value Objects vs primitivos (decisión de arquitectura previa)

---

## 4. Reflexión

El uso de IA en este proyecto demostró ser más efectivo cuando:
1. El prompt incluye el **contexto completo** (reglas de negocio + restricciones técnicas)
2. Se piden **clases pequeñas y cohesivas** en lugar de módulos grandes
3. Se especifican los **patrones y principios** a aplicar explícitamente
4. Se usa el **archivo CLAUDE.md** para que la IA mantenga consistencia entre sesiones

La IA fue especialmente útil para: boilerplate repetitivo (DTOs, Value Objects similares),
configuraciones complejas (EF Core HasConversion, JWT Bearer), y estructura de tests.
