# Guión del Video — ERP Suministros del Este

> **Duración estimada:** 15–18 minutos
> **Herramienta sugerida:** OBS Studio o Loom
> **Qué mostrar:** navegador + editor de código (VS Code o Rider)

---

## PARTE 1 — Introducción (1 min)

**[Abrir navegador con el Swagger del API desplegado]**

> "El sistema que van a ver es el ERP de Suministros del Este, una distribuidora de
> materiales de construcción. Desarrollé una API REST con .NET 8 aplicando Arquitectura
> Hexagonal, Domain-Driven Design, principios SOLID, y patrones de diseño como
> Factory, Strategy y Observer. El sistema también incluye seguridad OAuth2/JWT con
> Keycloak, Docker para el despliegue, y ArchUnit para validar la arquitectura en tests."

**[Mostrar la URL en vivo: `https://<tu-app>.railway.app/swagger`]**

---

## PARTE 2 — Sistema en Uso (4 min)

**[Con el Swagger o Postman abierto, ejecutar en vivo]**

### 2.1 Registrar materiales (POST /api/v1/materiales)

```json
{
  "nombre": "Cemento Portland Tipo I",
  "descripcion": "Sacos de 42.5 kg",
  "stockActual": 150,
  "stockMinimo": 50,
  "precioUnitario": 850.00
}
```
> "El POST registra el material. Internamente, el dominio eleva el evento
> MaterialRegistradoEvent que es publicado después de persistir."

```json
{
  "nombre": "Varillas 3/8",
  "descripcion": "Quintal 100 lb",
  "stockActual": 10,
  "stockMinimo": 25,
  "precioUnitario": 3200.00
}
```
> "Este segundo material tiene stock crítico (10 < mínimo 25). El dominio
> eleva además un StockBajoMinimoEvent."

### 2.2 Consultar inventario (GET /api/v1/materiales)
> "El GET retorna el catálogo completo con el campo RequiereReabastecimiento calculado."

### 2.3 Ver alertas (GET /api/v1/materiales/alertas)
> "Las alertas filtran solo los materiales con stock crítico, con el mensaje
> generado por el patrón Strategy."

### 2.4 Validación de dominio en acción
```json
{ "nombre": "", "stockActual": -5, "precioUnitario": 850.00, ... }
```
> "Las validaciones viven en los Value Objects del dominio, no en el controlador.
> Recibimos HTTP 400 con el mensaje de la DomainException."

---

## PARTE 3 — Estructura Interna (8 min)

**[Abrir VS Code / Rider con el proyecto]**

### 3.1 Arquitectura Hexagonal — Las 4 Capas

**[Abrir el explorador de archivos, mostrar la estructura de carpetas]**

```
src/
├── SuministrosDelEste.Domain/       ← Núcleo, sin dependencias
├── SuministrosDelEste.Application/  ← Casos de uso
├── SuministrosDelEste.Infrastructure/ ← Adaptadores
└── SuministrosDelEste.API/          ← Presentación
```

> "La regla clave es que el Dominio no conoce a nadie. La Aplicación solo conoce
> al Dominio. La Infraestructura implementa los puertos del Dominio."

### 3.2 DDD — Aggregate Root y Value Objects

**[Abrir `Domain/Aggregates/Material/Material.cs`]**

> "Material es el Aggregate Root. Tiene propiedades con private set para que
> nadie las modifique desde afuera. El único camino para crear un material es
> el factory method estático `Material.Crear(...)`, que aplica invariantes
> y eleva los eventos de dominio."

**[Abrir `Domain/Aggregates/Material/ValueObjects/Stock.cs`]**

> "Los Value Objects encapsulan la validación. El método `Crear()` rechaza
> stock negativo con una DomainException. `EsMenorOIgualQue` es comportamiento
> de dominio, no una simple comparación de decimales."

### 3.3 Domain Events — Patrón Observer

**[Abrir `Domain/Events/StockBajoMinimoEvent.cs` y `Shared/AggregateRoot.cs`]**

> "Cuando el stock baja del mínimo, el Aggregate eleva un StockBajoMinimoEvent.
> El AggregateRoot tiene una lista interna de eventos. Después de persistir,
> el Handler los publica via IEventPublisher — ese es el patrón Observer."

### 3.4 Patrón Factory

**[Abrir `Application/Factories/MaterialFactory.cs`]**

> "La Factory traduce el Command primitivo al Aggregate Root con Value Objects
> validados. Esto separa la construcción del uso — el Handler no sabe cómo
> construir un Material, delega eso a la Factory."

### 3.5 Patrón Strategy

**[Abrir `Application/Strategies/IStockAlertStrategy.cs` y `StockMinimoAlertStrategy.cs`]**

> "El algoritmo de alerta es intercambiable. Si mañana necesito alertar con
> el 20% sobre el mínimo, creo una nueva estrategia y la registro en DI
> sin tocar el Handler — esto es Open/Closed Principle."

### 3.6 Seguridad JWT / Keycloak

**[Abrir `Infrastructure/Security/KeycloakAuthExtensions.cs`]**

> "El controlador tiene `[Authorize]` en todos los endpoints.
> El POST de registro además requiere el rol `inventario-admin` de Keycloak.
> La configuración JWT apunta al realm de Keycloak como Authority —
> él valida la firma del token automáticamente via OIDC Discovery."

### 3.7 EF Core con Value Objects

**[Abrir `Infrastructure/Persistence/Context/AppDbContext.cs`]**

> "`HasConversion` le dice a EF Core cómo serializar el Value Object a SQL
> y cómo rehidratarlo al leer. Así la BD guarda `decimal` pero el dominio
> trabaja con `Stock` y `Precio`."

---

## PARTE 4 — Tests de Arquitectura (2 min)

**[Abrir `tests/SuministrosDelEste.ArchTests/HexagonalArchitectureTests.cs`]**

> "ArchUnitNET valida automáticamente que no se rompan las reglas de capas.
> Si alguien agrega un `using SuministrosDelEste.Infrastructure` en el Dominio,
> este test falla en el CI antes de mergear."

**[Mostrar el GitHub Actions en el repo: Actions → última ejecución → Tests pasan ✅]**

---

## PARTE 5 — Evidencia de IA (1 min)

**[Abrir `docs/evidencia-ia.md` y `CLAUDE.md`]**

> "El desarrollo se realizó con asistencia de Claude AI. El archivo CLAUDE.md
> le da contexto a la IA sobre arquitectura, reglas y estado del proyecto.
> AGENTS.md guía agentes autónomos. En `docs/evidencia-ia.md` están
> documentados los 7 prompts principales usados y la metodología aplicada."

---

## PARTE 6 — Cierre (30 seg)

> "En resumen: el sistema implementa Arquitectura Hexagonal donde el Dominio
> es el núcleo sin dependencias, la Aplicación orquesta via puertos,
> la Infraestructura provee los adaptadores, y la API es solo el punto de entrada.
> Esto hace que el sistema sea testeable, extensible y mantenible."

**[Mostrar la URL del repo GitHub en pantalla]**
