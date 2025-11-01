/* === 1. Crear base de datos (idempotente) === */
IF DB_ID('DemoTienda') IS NULL
BEGIN
    CREATE DATABASE DemoTienda;
END
GO

USE DemoTienda;
GO

/* === 2. Tablas === */

/* Categoría */
IF OBJECT_ID('dbo.Categoria', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categoria
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categoria PRIMARY KEY CLUSTERED,
        Nombre        NVARCHAR(100)     NOT NULL,
        Descripcion   NVARCHAR(500)     NULL,
        EsActiva      BIT               NOT NULL CONSTRAINT DF_Categoria_EsActiva DEFAULT (1),
        FechaCreacion DATETIME2(0)      NOT NULL CONSTRAINT DF_Categoria_FechaCreacion DEFAULT (SYSUTCDATETIME())
    );

    /* Única por nombre (evita duplicados de categoría) */
    CREATE UNIQUE INDEX UX_Categoria_Nombre ON dbo.Categoria (Nombre);
END
GO

/* Producto */
IF OBJECT_ID('dbo.Producto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Producto
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Producto PRIMARY KEY CLUSTERED,
        Nombre          NVARCHAR(150)     NOT NULL,
        Descripcion     NVARCHAR(1000)    NULL,
        Precio          DECIMAL(18,2)     NOT NULL CONSTRAINT DF_Producto_Precio DEFAULT (0),
        IdCategoria     INT               NOT NULL,
        EsActivo        BIT               NOT NULL CONSTRAINT DF_Producto_EsActivo DEFAULT (1),
        FechaCreacion   DATETIME2(0)      NOT NULL CONSTRAINT DF_Producto_FechaCreacion DEFAULT (SYSUTCDATETIME())
    );

    /* FK y comportamiento */
    ALTER TABLE dbo.Producto
      ADD CONSTRAINT FK_Producto_Categoria
      FOREIGN KEY (IdCategoria) REFERENCES dbo.Categoria(Id)
      ON UPDATE NO ACTION
      ON DELETE NO ACTION; -- (si quisieras borrado en cascada, cambia a ON DELETE CASCADE)

    /* Evita duplicar nombres dentro de la misma categoría */
    CREATE UNIQUE INDEX UX_Producto_Nombre_Categoria
      ON dbo.Producto (IdCategoria, Nombre);

    /* Búsquedas típicas */
    CREATE INDEX IX_Producto_IdCategoria ON dbo.Producto (IdCategoria);
    CREATE INDEX IX_Producto_EsActivo ON dbo.Producto (EsActivo);
END
GO
