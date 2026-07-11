// ═══════════════════════════════════════════════════════════════════════
// ⚠️  CÓDIGO DE REFERENCIA — "ANTES" DEL REFACTOR SOLID  ⚠️
// ═══════════════════════════════════════════════════════════════════════
// Este archivo documenta cómo se veía la funcionalidad de cálculo de
// descuentos ANTES de aplicar los principios SOLID. NO forma parte de
// la compilación (no está referenciado en ningún .csproj): es evidencia
// para la Tarea 4 (INF-259-01 · SOLID + UI/UX), no código de producción.
//
// La versión SOLID equivalente vive en el código real del proyecto:
//   src/SuministrosDelEste.Application/Strategies/IDescuentoStrategy.cs
//   src/SuministrosDelEste.Application/Strategies/Descuento*Strategy.cs
//   src/SuministrosDelEste.Application/UseCases/CalcularDescuento/
//
// Contiene, a propósito, 4 de los 5 olores de código (SRP, OCP, ISP, DIP).
// El quinto (LSP) está en DescuentoClientePremium_ANTES.cs, en este mismo
// directorio, porque necesita una subclase para poder violarse.
// ═══════════════════════════════════════════════════════════════════════

using System;

namespace SuministrosDelEste.Application.Descuentos.Antes;

// ---------------------------------------------------------------------
// 🔴 ISP violado: interfaz "gorda". Calcular un descuento no tiene
// ninguna relación con generar un reporte mensual o exportar a Excel,
// pero cualquier clase que implemente IDescuentoOperations está obligada
// a tener los cinco métodos, use los que use.
// ---------------------------------------------------------------------
public interface IDescuentoOperations
{
    decimal CalcularDescuento(string tipoCliente, decimal montoCompra);
    void GuardarAuditoria(string mensaje);
    void EnviarNotificacionEmail(string destinatario, string mensaje);
    void GenerarReporteMensual();
    void ExportarAExcel();
}

// ---------------------------------------------------------------------
// 🔴 SRP violado: esta clase calcula, audita Y notifica por correo.
//    Tres razones distintas para cambiar (cambia el % de descuento,
//    cambia cómo se audita, cambia el proveedor de correo) en un
//    mismo archivo.
//
// 🔴 OCP violado: CalcularDescuento() tiene un if/else por tipo de
//    cliente. Agregar un tipo de cliente nuevo (ej. "Corporativo")
//    obliga a EDITAR este método ya probado, arriesgando romper los
//    tres casos que ya funcionaban.
//
// 🔴 DIP violado: GuardarAuditoria() y EnviarNotificacionEmail()
//    crean sus dependencias con "new" adentro del método. La lógica
//    de negocio queda atada a SQL Server y a SMTP concretos — no se
//    puede probar CalcularDescuento() sin una base de datos y un
//    servidor de correo reales corriendo.
// ---------------------------------------------------------------------
public class CalculadoraDescuentoService : IDescuentoOperations
{
    public decimal CalcularDescuento(string tipoCliente, decimal montoCompra)
    {
        decimal porcentaje;

        // OCP violado: cada tipo de cliente nuevo = tocar este bloque.
        if (tipoCliente == "Premium")
        {
            porcentaje = 0.15m;
        }
        else if (tipoCliente == "Frecuente")
        {
            porcentaje = 0.10m;
        }
        else if (tipoCliente == "PorVolumen")
        {
            porcentaje = montoCompra > 50000 ? 0.12m : 0.05m;
        }
        else
        {
            porcentaje = 0m;
        }

        decimal descuento = montoCompra * porcentaje;

        // SRP violado: además de calcular, audita...
        GuardarAuditoria($"Descuento calculado: {descuento:C2} para cliente {tipoCliente}");

        // ...y notifica. Tres responsabilidades, una clase.
        EnviarNotificacionEmail("ventas@suministrosdeleste.com",
            $"Se aplicó un descuento de {descuento:C2} a una compra de {montoCompra:C2}.");

        return descuento;
    }

    public void GuardarAuditoria(string mensaje)
    {
        // DIP violado: instancia concreta creada aquí mismo, no inyectada.
        SqlAuditoriaRepository repo = new();
        repo.Guardar(mensaje);
    }

    public void EnviarNotificacionEmail(string destinatario, string mensaje)
    {
        // DIP violado: mismo problema con el notificador.
        SmtpNotificador notificador = new();
        notificador.Enviar(destinatario, mensaje);
    }

    // ISP violado: nada de esto tiene que ver con "calcular un descuento",
    // pero la interfaz gorda obliga a implementarlo igual.
    public void GenerarReporteMensual() { /* ... */ }
    public void ExportarAExcel() { /* ... */ }
}

// Dependencias concretas de infraestructura, acopladas directo a la
// lógica de negocio — la causa raíz del DIP violado arriba.
public class SqlAuditoriaRepository
{
    public void Guardar(string mensaje) { /* INSERT directo a SQL Server */ }
}

public class SmtpNotificador
{
    public void Enviar(string destinatario, string mensaje) { /* SMTP directo */ }
}
