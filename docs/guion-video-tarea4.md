# Guion del Video — Tarea 4: SOLID + UI/UX
## ERP Suministros del Este S.R.L. · Ingeniería de Software 2 (INF-259-01)
### Presenta: Rafi Alejandro Suero Valera · Matrícula 2022-3100150

> **Cómo usar este guion.** Lo que está **« entre comillas »** se lee en voz alta, con calma. Lo que está en _cursiva_ son **instrucciones** (qué clic dar o qué pantalla mostrar): no se dice. Los recuadros **🧠** son para que entiendas lo que dices y puedas responder si te preguntan; no se leen.
>
> **Plan de hoy:** dos pantallas — el **dashboard v2** (`frontend/dashboard-v2.html`, para funcionamiento y UI/UX) y tu **editor de código** (VS Code o Visual Studio, para el código real de SOLID). Practica el cambio entre las dos una vez antes de grabar.
>
> **Duración objetivo:** 8-9 min (dentro del rango 5-10 que pide la rúbrica).

---

## ✅ Preparación (5 minutos antes de grabar)

1. Confirma que ya aplicaste el patch/zip a tu repo, hiciste `dotnet build` limpio, y subiste los commits a GitHub — el video muestra el resultado, no tiene sentido grabar antes de eso.
2. Abre **`frontend/dashboard-v2.html`** en el navegador (doble clic). Déjalo listo en "Inventario", sin autenticar todavía.
3. Abre tu editor con estos 5 archivos en pestañas, en este orden (los vas a enseñar en el Segmento 3):
   - `docs/solid-antes-despues/antes/CalculadoraDescuentoService.cs` (el "antes")
   - `src/SuministrosDelEste.Application/Strategies/IDescuentoStrategy.cs`
   - `src/SuministrosDelEste.Application/Strategies/DescuentoPremiumStrategy.cs`
   - `src/SuministrosDelEste.Application/UseCases/CalcularDescuento/CalcularDescuentoHandler.cs`
   - _(opcional, por si te preguntan LSP o ISP)_ `docs/solid-antes-despues/antes/DescuentoClientePremium.cs`
4. Activa la **cámara (rostro)** en Picture-in-Picture y prueba el micrófono. Cierra notificaciones.

> 🧠 **Por qué dos pantallas y no solo el dashboard.** El dashboard v2 demuestra que el sistema funciona y las mejoras de UI/UX se ven — eso resuelve el punto "muestra el proyecto funcionando" y "recorre las mejoras de UI/UX" de la rúbrica. Pero SOLID se demuestra con **código real**, no con una animación — por eso el Segmento 3 se mueve al editor. Es exactamente el mismo patrón que ya usaste en el examen anterior con el dashboard de Inventario.

---

## 🎬 SEGMENTO 1 — Introducción (≈1 min)

_En pantalla: tu rostro + el dashboard v2 en reposo, pestaña Inventario._

> «Buenos días. Mi nombre es **Rafi Alejandro Suero Valera**, matrícula **2022-3100150**. Esta es la Tarea 4 de Ingeniería de Software 2: aplicar los cinco principios SOLID a la lógica de negocio y mejorar la interfaz con técnicas profesionales de UI/UX, sobre mi sistema **ERP Suministros del Este**.»

> «Para esta tarea agregué una función nueva al sistema: el **cálculo de descuentos** para el futuro Módulo de Ventas. La construí primero con los cinco olores de código a propósito, y luego la refactoricé aplicando SOLID — así puedo mostrarles un antes y un después real, no solo la teoría. Y reconstruí el panel de presentación con ocho técnicas de UI/UX, dos más de las seis que pedía la asignación.»

---

## 🎬 SEGMENTO 2 — Demostración funcional y recorrido de UI/UX (≈3 min)

_Todo ocurre en el dashboard v2. Narra qué pasa mientras haces clic._

### ▶️ Autenticación y roles
_Acción: pulsa "Conectar (Keycloak)". Luego abre el selector de rol (arriba a la derecha) y cambia a "Cajero"._

> «Primero, seguridad: me conecto con Keycloak igual que en el sistema real.» _(indicador verde)_ «Y aquí está la primera técnica de UI/UX: un selector de rol. Como Administrador veo todo. Si cambio a Cajero» _(cambia el rol)_ «el menú Configuración desaparece, y el botón de Registrar material se deshabilita — no se queda ahí sin función, se apaga con una explicación al pasar el mouse. Eso es **ocultación y deshabilitación por permisos**, una de las técnicas obligatorias.»

_Acción: vuelve a rol Administrador._

### ▶️ Menú superior y sidebar
_Acción: abre el menú "Inventario" en la barra superior. Señala los atajos de teclado. Luego colapsa el sidebar._

> «Arriba tengo una **barra de menú** organizada por categorías — Archivo, Inventario, Ventas, Reportes, Configuración — con íconos y atajos, como cualquier sistema de escritorio profesional.» _(colapsa el sidebar)_ «Y a la izquierda, un **menú lateral colapsable**: si necesito más espacio para trabajar, lo reduzco a solo íconos.»

### ▶️ DataGrid y estados de carga
_Acción: haz clic en el encabezado "Stock" para ordenar. Señala la paginación. Luego registra un material y señala el spinner del botón mientras carga._

> «La tabla de inventario ahora es un **DataGrid profesional**: encabezados que ordenan al hacer clic, filas cebra, y paginación real.» _(registra un material)_ «Y miren el botón mientras registro: se deshabilita y muestra un spinner. Antes esto no pasaba — era un hallazgo real de un diagnóstico anterior de este mismo proyecto: sin indicador de carga, el usuario no sabe si su clic funcionó, y puede hacer doble clic por accidente.»

> 🧠 Esto conecta tu Tarea 4 con trabajo previo del curso — muestra continuidad, no que empezaste de cero.

### ▶️ Wizard y modales
_Acción: ve a "Ventas" → "Nueva venta". Recorre los 3 pasos hasta confirmar. Luego ve a Inventario, haz clic en un material, y abre el modal de desactivar (sin confirmar)._

> «Aquí está el **asistente por pasos**: para una venta, en vez de un formulario largo, tres pasos con barra de progreso — cliente, producto, confirmar.» _(avanza los pasos)_ «El paso 3 aplica en vivo la estrategia de descuento según el tipo de cliente, y confirma.» _(clic en un material de la tabla)_ «Y esto es una **ventana modal**: veo y edito el material sin salir de la pantalla. Si intento desactivarlo» _(abre el modal de confirmación)_ «un segundo modal me confirma antes de ejecutar la acción — nunca se pierde el contexto ni se borra algo por accidente.»

### ▶️ Paleta documentada
_Acción: ve a Configuración → Guía de estilo._

> «Y aquí, la **paleta de color documentada**: cada tono tiene su código HEX y su significado — verde para éxito, rojo para error, ámbar como acento de marca. No es decoración: cada color comunica un estado del sistema.»

---

## 🎬 SEGMENTO 3 — SOLID en código real (≈3 min)

_Cambia a tu editor de código. Aquí se explican al menos 3 principios, como exige la rúbrica — el guion cubre los 5 por si el profesor pregunta más._

### Parte A — El "antes": los 5 olores en una sola clase (≈40 seg)
_Abre `docs/solid-antes-despues/antes/CalculadoraDescuentoService.cs`._

> «Antes de refactorizar, así se veía la función de descuentos.» _(señala el método CalcularDescuento)_ «Esta clase calcula el descuento, guarda auditoría Y envía un correo — tres responsabilidades distintas. Tiene un if/else que crece cada vez que aparece un tipo de cliente nuevo. Y crea sus dependencias con "new" directo en el método, así que no se puede probar sin una base de datos real corriendo. Ahora el después.»

### Parte B — SRP y DIP en el Handler (≈1 min)
_Abre `CalcularDescuentoHandler.cs`._

> «Este es el orquestador refactorizado. **Responsabilidad Única**: esta clase solo coordina el cálculo — ya no audita ni notifica por su cuenta.» _(señala el constructor)_ «Y **Inversión de Dependencias**: mira el constructor — recibe `IEnumerable<IDescuentoStrategy>` y `IEventPublisher`, ambas interfaces, inyectadas. Nunca hace "new" de una clase concreta. Puedo probar esta clase con un publisher falso, sin tocar infraestructura real.»

### Parte C — OCP e ISP en la Strategy (≈1 min)
_Abre `IDescuentoStrategy.cs` y luego `DescuentoPremiumStrategy.cs`._

> «**Abierto/Cerrado**: en vez del if/else que vimos antes, cada tipo de cliente es una clase que implementa esta interfaz. Si mañana agrego un descuento "Corporativo", creo una clase nueva y la registro — no toco ninguna de las que ya funcionan y ya están probadas.» _(señala la interfaz)_ «Y **Segregación de Interfaces**: la interfaz tiene un solo método. Compárenla con la interfaz gorda del "antes", que obligaba a implementar auditoría y reportes aunque no tuvieran nada que ver.»

### Parte D — LSP, rápido (≈30 seg)
_Opcional: abre `DescuentoClientePremium.cs` en docs/ si quieres cubrir los 5._

> «Y de paso, **Sustitución de Liskov**: en el código "antes" había una subclase que lanzaba una excepción si el cliente no era Premium — rompía el contrato de la clase base. Mis tres estrategias de hoy cumplen todas el mismo contrato: nunca lanzan, siempre devuelven un valor válido. Por eso son intercambiables sin sorpresas.»

---

## 🎬 SEGMENTO 4 — Justificación de decisiones (≈1.5 min)

_Vuelve a tu rostro en cámara, o al dashboard de fondo._

> «Dos decisiones que quiero justificar. Primero, en vez de inventar un mecanismo de notificación nuevo para los descuentos, reutilicé `IEventPublisher` — el mismo puerto que ya usa el módulo de Inventario. Menos código, y el patrón Observer se mantiene consistente en todo el sistema.»

> «Segundo, en el frontend elegí evolucionar el panel existente en vez de empezar uno nuevo desde cero. Así puedo comparar objetivamente un antes y un después reales, con capturas del mismo sistema — no una demo genérica. Y encontré y corregí dos bugs reales durante las pruebas: un desbordamiento horizontal en móvil y un botón que perdía su texto durante la animación de carga. Los dejo mencionados porque construir con SOLID y con buena UI/UX no es solo seguir una plantilla — es también probar lo que uno construye.»

---

## 🎬 SEGMENTO 5 — Cierre (≈30 seg)

> «Para cerrar: los cinco principios SOLID no son reglas separadas — se refuerzan entre sí. Sin OCP no tendría sentido inyectar estrategias (DIP); sin ISP, esas estrategias tendrían métodos de más. Y una interfaz que respeta LSP es la que hace que OCP funcione de verdad: puedo agregar clases nuevas con confianza porque todas cumplen el mismo contrato.»

> «Con esto concluyo mi presentación de la Tarea 4. Muchas gracias.»

---

## ⏱️ Tiempos (total ≈9 min, dentro del rango 5–10)

| Segmento | Tiempo | Cubre de la rúbrica |
|---|---|---|
| 1. Introducción | 1:00 | — |
| 2. Demostración + recorrido UI/UX | 3:00 | Funcionamiento · UI/UX (mín. 6 técnicas) |
| 3. SOLID en código real | 3:00 | Mín. 3 principios con código |
| 4. Justificación de decisiones | 1:30 | Justificación de decisiones |
| 5. Cierre | 0:30 | — |

**Recordatorios:** rostro visible en las partes que no son pantalla completa · muestra código real, no solo el dashboard · si te trabas en una palabra, respira y sigue — la claridad importa más que la perfección.
