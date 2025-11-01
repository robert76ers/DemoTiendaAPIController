CREATE OR ALTER VIEW dbo.vProductosConCategoria
AS
SELECT 
    p.Id,
    p.Nombre        AS NombreProducto,
    p.Precio,
    p.EsActivo,
    p.FechaCreacion,
    c.Id            AS IdCategoria,
    c.Nombre        AS NombreCategoria
FROM dbo.Producto p
JOIN dbo.Categoria c ON c.Id = p.IdCategoria;
GO
