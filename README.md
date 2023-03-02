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

#### 1. Clone project
$ git clone https://github.com/livingston12/ConsultaSMS.API

#### 3. Enter folder in cmd or terminal in visual studio code
`cd ConsultaSMS.Api/ConsultaSMS.Api`

#### 4. Configure connection string
$ Change the variable **(`QuerySMSConnection`)** to point to the database if it doesn't exist create it. modify in the file **(`appsetting.develoment.json`)** if you are in development, if it is published change in **(`appsetting.json`)**. 

#### 5. Install dependencies
$ `dotnet install`

#### 6. Restore dependencies
$ `dotnet restore`

## publish project ##
$ `dotnet build`

<br>
 
The server will initialize at the url <https://localhost:5001>

to run the swagger enter the following url <https://localhost:5001/swagger/index.html>


&#xa0;

<a href="#top">Back to top</a>
