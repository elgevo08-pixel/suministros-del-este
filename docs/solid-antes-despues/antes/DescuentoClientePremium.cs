// ═══════════════════════════════════════════════════════════════════════
// ⚠️  CÓDIGO DE REFERENCIA — "ANTES" DEL REFACTOR SOLID (LSP)  ⚠️
// ═══════════════════════════════════════════════════════════════════════
// No forma parte de la compilación. Ver CalculadoraDescuentoService.cs
// en este mismo directorio para el resto de los olores de código.
// ═══════════════════════════════════════════════════════════════════════

using System;

namespace SuministrosDelEste.Application.Descuentos.Antes;

// ---------------------------------------------------------------------
// 🔴 LSP violado: DescuentoClientePremiumService hereda de
//    CalculadoraDescuentoService, pero NO puede sustituirla sin cambiar
//    el comportamiento del programa.
//
//    La clase base acepta cualquier tipoCliente (si no lo reconoce,
//    simplemente retorna 0 de descuento — nunca lanza). Esta subclase
//    endurece la PRECONDICIÓN: exige que tipoCliente sea "Premium" y
//    lanza una excepción para cualquier otro valor.
//
//    Cualquier código escrito contra CalculadoraDescuentoService,
//    confiando en su contrato original ("nunca lanza, siempre devuelve
//    un decimal"), se rompe en tiempo de ejecución si en realidad
//    recibe esta subclase. Eso es exactamente lo que el Principio de
//    Sustitución de Liskov prohíbe: una subclase debe poder reemplazar
//    a su clase base sin sorpresas para quien la consume.
// ---------------------------------------------------------------------
public class DescuentoClientePremiumService : CalculadoraDescuentoService
{
    public new decimal CalcularDescuento(string tipoCliente, decimal montoCompra)
    {
        if (tipoCliente != "Premium")
            throw new InvalidOperationException(
                "Esta calculadora solo admite clientes Premium.");

        return montoCompra * 0.20m;
    }
}
