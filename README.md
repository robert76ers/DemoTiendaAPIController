# DemoTiendaApi.Minimal (.NET 9 + EF Core)

## Descripción General
Este proyecto es una **API REST** creada con **.NET 9** que implementa buenas prácticas modernas de **configuración**, **inyección de dependencias (DI)** y **logging estructurado**.  
Incluye ejemplos de **endpoints CRUD básicos** para demostrar el flujo completo desde la configuración hasta la ejecución.

---

## Requisitos Previos

1. Visual Studio 2022 o VS Code  
2. .NET 9 SDK  
3. SQL Server o LocalDB  
4. Git  

---

## Instalación y Ejecución

### Clonar el repositorio
```bash
git clone https://github.com/tuusuario/DemoTiendaApi.Minimal.git
cd DemoTiendaApi.Minimal
2. **Crear la base de datos**  
   Abrir los scripts en la carpeta `/Database`  
   Ejecutarlos en **SQL Server Management Studio** o **Azure Data Studio** en este orden:

   1. `1 CREATE BD AND TABLES.sql`  
   2. `2 CREATE OR ALTER VIEW dbo.vProductosCategoria.sql`  
   3. `3 INSERTS.sql`

3. **Configurar la conexión**  
   En el archivo `appsettings.json`, actualizar la cadena de conexión según tu entorno local.  
   Ejemplo:
   ```json
   "ConnectionStrings": {
     "DemoTienda": "Server=.\\SQLEXPRESS;Database=DemoTienda;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

4. **Ejecutar la API**  
   Desde Visual Studio o la terminal, ejecutar:
   ```bash
   dotnet run
   ```
   Luego abrir en el navegador:  
    [https://localhost:7249/swagger](https://localhost:7249/swagger)

---

##  Aprendizaje Esperado
- Comprender la **estructura de una API moderna**.  
- Configurar una API usando `appsettings.json` y **Dependency Injection**.  
- Probar endpoints REST mediante **Swagger/OpenAPI**.  
- Comprender la conexión entre **EF Core** y **SQL Server**.  

---

##  Licencia
Proyecto de práctica con fines educativos.  
Puedes usarlo libremente para tu aprendizaje o como base para otros ejercicios.

---

 **Autor:** _José Roberto Villavicencio Alcívar_  
 Curso: *Prácticas de desarrollo moderno en .NET 9*
