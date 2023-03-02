<h1 align="center">Api Consulta SMS</h1>

## About ##

Este proyecto es para que AFE pueda obtener los datos de los productos de los clientes y el detalle de los productos y mostrarlos a traves de SMS.

## Features ##

* Valida si el usuario existe como cliente.
* Valida si el cliente tiene el pin correcto.
* Consulta los productos Cuenta ahorro, Cuenta corriente, Tarjeta de creditos y prestamos.
* Consulta el detalle de los productos.

## Technologies ##

En este proyecto se utilizaron las siguientes herramientas:

- [Dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Requirements ##

Antes de inciar necesitas las siguientes herramientas [Git](https://git-scm.com) and [Ef Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) installed.

## Starting ##

#### 1. Clonar proyecto
$ git clone https://dev.azure.com/BMSC/Software%20Factory/_git/BSC%20APIs

#### 2. Cambiar nombre carpeta
$ ~~~~BSC%20APIs~~~~ por `BSCAPIs`

#### 3. Ingresar a carpeta en cmd o terminal en visual studio code
`cd BSCAPIs/ApiConsultaSMS`

#### 4. Configurar conection string
$ Cambiar la variable **(`ConsultaSMSConnection`)** para que apunte a la base de datos si no existe la crea. modificar en el archivo **(`appsetting.develoment.json`)** si estas en desarrollo, si esta publicada cambiar en **(`appsetting.json`)**. 

#### 5. intalar dependecias
$ `dotnet install`

#### 6. Restaurar dependecias
$ `dotnet restore`

## Publicar proyecto ##
$ `dotnet build`

<br>
 
El servidor inicializara en la url <https://localhost:5001>

para ejecutar el swagger ingrese la siguiente url <https://localhost:5001/swagger/index.html>


&#xa0;

<a href="#top">Back to top</a>
