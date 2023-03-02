<h1 align="center">Api Consulta SMS</h1>

## About ##

This is a project from which a company that sends sms feeds to show the company's products. Write from your cell phone 272 with the word balance and if you are a user of the bank this will show you an interaction to show you your products through SMS.

## Features ##

* Validates if the user exists as a customer.
* Validates if the client has the correct pin.
* Consult the products Savings account, Current account, Credit card and loans.
* Consult the details of the products.

## Technologies ##

- [Dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Requirements ##

Before starting you need the following tools [Git](https://git-scm.com) and [Ef Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) installed.

## Starting ##

#### 1. Clonar proyecto
$ git clone https://dev.azure.com/BMSC/Software%20Factory/_git/BSC%20APIs

#### 3. Ingresar a carpeta en cmd o terminal en visual studio code
`cd ConsultaSMS.Api/ConsultaSMS.Api`

#### 4. Configurar conection string
$ Cambiar la variable **(`ConsultaSMSConnection`)** para que apunte a la base de datos si no existe la crea. modificar en el archivo **(`appsetting.develoment.json`)** si estas en desarrollo, si esta publicada cambiar en **(`appsetting.json`)**. 

#### 5. intalar dependecias
$ `dotnet install`

#### 6. Restaurar dependecias
$ `dotnet restore`

## Publicar proyecto ##
$ `dotnet build`

<br>
 
The server will initialize at the url <https://localhost:5001>

to run the swagger enter the following url <https://localhost:5001/swagger/index.html>


&#xa0;

<a href="#top">Back to top</a>
