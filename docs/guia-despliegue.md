# Guía de Despliegue — Railway.app

> El sistema queda disponible en una URL pública tipo `https://suministros-api.railway.app`

---

## Paso 1: Subir el proyecto a GitHub

```bash
# En la carpeta raíz del proyecto
git init
git add .
git commit -m "feat: ERP Suministros del Este - Hexagonal + DDD + SOLID"

# Crear repo en github.com y luego:
git remote add origin https://github.com/TU_USUARIO/suministros-del-este.git
git branch -M main
git push -u origin main
```

---

## Paso 2: Crear cuenta en Railway.app

1. Ir a **[railway.app](https://railway.app)** → Sign Up with GitHub
2. Crear un **New Project**
3. Elegir **"Deploy from GitHub repo"**
4. Seleccionar el repositorio `suministros-del-este`

---

## Paso 3: Configurar Variables de Entorno

En el dashboard de Railway → pestaña **Variables**, agregar:

| Variable | Valor |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | `Data Source=/app/data/suministros.db` |
| `Jwt__DemoMode` | `true` |
| `PORT` | `80` |

> 💡 El archivo `appsettings.Production.json` ya tiene estos valores como fallback.

---

## Paso 4: Configurar el Servicio

Railway detecta el `railway.json` automáticamente y usará el `Dockerfile`.

En la pestaña **Settings** del servicio:
- **Build Command:** *(vacío — usa Dockerfile)*
- **Root Directory:** `/` *(raíz del repo)*
- **Health Check Path:** `/swagger`

---

## Paso 5: Deploy

Railway lanza el build automáticamente. Esperar ~3 minutos.

Una vez completado, el panel muestra la URL pública:
```
https://suministros-api-production.up.railway.app
```

Abrir `<URL>/swagger` para ver la API funcionando en vivo.

---

## Paso 6: Verificar Endpoints Funcionando

```bash
# Variables
BASE=https://suministros-api-production.up.railway.app

# GET - Listar materiales (vacío al inicio)
curl $BASE/api/v1/materiales

# POST - Registrar material
curl -X POST $BASE/api/v1/materiales \
  -H "Content-Type: application/json" \
  -d '{"nombre":"Cemento Portland","descripcion":"Saco 42.5kg","stockActual":100,"stockMinimo":20,"precioUnitario":850.00}'

# GET - Alertas de reabastecimiento
curl $BASE/api/v1/materiales/alertas
```

---

## Estructura en Producción

```
Railway Service: suministros-api
├── Docker container (mcr.microsoft.com/dotnet/aspnet:8.0)
├── Base de datos: SQLite en /app/data/suministros.db
├── Auth: DemoMode=true (sin Keycloak)
└── URL: https://<app>.railway.app
```

---

## Alternativa: Despliegue Local Completo con Docker

Para levantar el stack **completo** (SQL Server + Keycloak + API) en local:

```bash
# Requiere Docker Desktop instalado
docker-compose up -d

# Ver estado
docker-compose ps

# La API queda en: http://localhost:5000/swagger
# Keycloak Admin:  http://localhost:8080 (admin/admin123)

# Aplicar migraciones (primera vez)
docker exec suministros-api dotnet ef database update \
  --project SuministrosDelEste.Infrastructure \
  --startup-project SuministrosDelEste.API
```

---

## Solución de Problemas

| Problema | Solución |
|----------|----------|
| Build falla en Railway | Verificar que `railway.json` está en la raíz |
| Error "Connection string not found" | Verificar variables de entorno en Railway dashboard |
| Swagger no carga | Asegurar que `PORT=80` está configurado |
| Migraciones no se aplican | El `Program.cs` las aplica automáticamente en `DemoMode=true` |
