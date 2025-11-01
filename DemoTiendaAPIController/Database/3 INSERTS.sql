USE DemoTienda;
GO

/* Categorías base */
IF NOT EXISTS (SELECT 1 FROM dbo.Categoria)
BEGIN
    INSERT INTO dbo.Categoria (Nombre, Descripcion)
    VALUES 
      (N'Electrónica', N'Dispositivos y accesorios'),
      (N'Hogar',       N'Artículos para el hogar'),
      (N'Oficina',     N'Suministros y equipos');
END

/* Productos de ejemplo */
IF NOT EXISTS (SELECT 1 FROM dbo.Producto)
BEGIN
    DECLARE @IdElec  INT = (SELECT Id FROM dbo.Categoria WHERE Nombre = N'Electrónica');
    DECLARE @IdHogar INT = (SELECT Id FROM dbo.Categoria WHERE Nombre = N'Hogar');
    DECLARE @IdOfi   INT = (SELECT Id FROM dbo.Categoria WHERE Nombre = N'Oficina');

    INSERT INTO dbo.Producto (Nombre, Descripcion, Precio, IdCategoria)
    VALUES
      (N'Teclado',          N'Teclado USB mecánico',            39.90, @IdElec),
      (N'Mouse',            N'Mouse óptico inalámbrico',        19.50, @IdElec),
      (N'Lampara LED',      N'Lámpara de escritorio',           24.99, @IdHogar),
      (N'Silla ergonómica', N'Silla para oficina con soporte', 149.00, @IdOfi);
END
GO
