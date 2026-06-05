using Microsoft.EntityFrameworkCore;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Ports;
using SuministrosDelEste.Infrastructure.Persistence.Context;

namespace SuministrosDelEste.Infrastructure.Persistence.Repositories;

/// <summary>
/// Adaptador secundario (Secondary / Driven Adapter) que implementa IMaterialRepository.
/// Conecta el Dominio con SQL Server 2022 via Entity Framework Core 8.
///
/// Patrón Hexagonal: Secondary Adapter
/// Patrón de diseño: Repository — abstrae el acceso a datos del resto del sistema.
/// </summary>
public sealed class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Material>> ObtenerTodosAsync()
    {
        try
        {
            // Ordenar por Nombre en memoria: HasConversion con VO no es directamente
            // traducible a SQL ORDER BY. Para grandes volúmenes, usar SQL crudo.
            List<Material> materiales = await _context.Materiales
                .Where(m => m.IsActive)
                .ToListAsync();

            return materiales.OrderBy(m => m.Nombre.Valor);
        }
        catch (Exception ex)
        {
            throw new Exception($"[MaterialRepository.ObtenerTodosAsync] {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Material?> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await _context.Materiales.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"[MaterialRepository.ObtenerPorIdAsync] ID={id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Material> GuardarAsync(Material material)
    {
        try
        {
            _context.Materiales.Add(material);
            await _context.SaveChangesAsync();
            return material; // EF Core habrá asignado el MaterialId generado por BD
        }
        catch (Exception ex)
        {
            throw new Exception($"[MaterialRepository.GuardarAsync] {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Material> ActualizarAsync(Material material)
    {
        try
        {
            _context.Materiales.Update(material);
            await _context.SaveChangesAsync();
            return material;
        }
        catch (Exception ex)
        {
            throw new Exception($"[MaterialRepository.ActualizarAsync] ID={material.MaterialId}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DesactivarAsync(int id)
    {
        try
        {
            Material? material = await _context.Materiales.FindAsync(id);
            if (material is null) return false;

            material.Desactivar(); // Lógica de negocio en el Aggregate Root
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"[MaterialRepository.DesactivarAsync] ID={id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Material>> ObtenerBajoStockMinimoAsync()
    {
        try
        {
            // Filtrado en memoria con el comportamiento del Value Object Stock.
            // NOTA: HasConversion en EF Core impide comparar VO directamente en SQL.
            // TODO: Optimizar con raw SQL si el volumen de materiales crece significativamente.
            List<Material> activos = await _context.Materiales
                .Where(m => m.IsActive)
                .ToListAsync();

            return activos.Where(m => m.StockActual.EsMenorOIgualQue(m.StockMinimo));
        }
        catch (Exception ex)
        {
            throw new Exception($"[MaterialRepository.ObtenerBajoStockMinimoAsync] {ex.Message}", ex);
        }
    }
}
