using WebConsultaSMS.Models.Entities;

namespace ConsultaSMS.Test.Service
{
    public class TestMemoryData
    {
        public IEnumerable<RolEntity> rolEntities = new List<RolEntity>()
        {
            new RolEntity()
                {
                    RolId = 1,
                    Name = "Admin",
                    Active = true
                },
            new RolEntity() {
                    RolId = 2,
                    Name = "User",
                    Active = true
                },
            new RolEntity() {
                    RolId = 3,
                    Name = "ExternalUser",
                    Active = true
                }
        };

        public static IEnumerable<UserEntity> userEntities = new List<UserEntity>()
        {
            new UserEntity
            {
                Name = "AFE External user to login",
                Email = "external-BMSC-AFE",
                UserName = "BMSC-AFE",
                Password = "lOZgnWZGzGz1F1lCuk/DZNAYJE/qXBTE6E+S+SQogcM=$mGsEffwbE8Jb+UMW2A4MVQ==$XnPtdmi9qkuQsy0JBEMYNE9UkZWL4wQj99nWd0H/0gQ=",
                Active = true,
                RolId = 2,
                IsEncriptPassword = true
            },
             new UserEntity
            {
                Name = "external-8098166513",
                Email = "8098166513",
                UserName = "8098166513",
                Password = "8098166513",
                Pin = "",
                MDCodeClient = "",
                Active = true,
                RolId = 3,
                IsEncriptPassword = false
            },
            new UserEntity
            {
                Name = "external-8098166514",
                Email = "8098166514",
                UserName = "8098166514",
                Password = "8098166514",
                Pin = "",
                MDCodeClient = "203020",
                Active = true,
                RolId = 3,
                IsEncriptPassword = false,
                Phones = new List<PhoneEntity>() {
                    new PhoneEntity() {
                        NumDocument = "",
                        Telephone = "8098166514",
                        NumberSmsSent = 0,
                        Token = "",
                        MDCodeClient = "0",
                        ExpitarationDate = DateTime.Now.AddDays(1),
                        Active = true
                    }
                }
            },
            new UserEntity
            {
                Name = "external-8098166515",
                Email = "8098166515",
                UserName = "8098166515",
                Password = "8098166515",
                Pin = "",
                MDCodeClient = "20102",
                Active = true,
                RolId = 3,
                IsEncriptPassword = false,
                Phones = new List<PhoneEntity>() {
                    new PhoneEntity() {
                        NumDocument = "4321",
                        Telephone = "8098166515",
                        NumberSmsSent = 0,
                        Token = "",
                        MDCodeClient = "20102",
                        ExpitarationDate = DateTime.Now.AddDays(1),
                        Active = true
                    }
                }
            },
            new UserEntity
            {
                Name = "external-8098166516",
                Email = "8098166516",
                UserName = "8098166516",
                Password = "8098166516",
                Pin = "1234",
                MDCodeClient = "201052",
                Active = true,
                RolId = 3,
                IsEncriptPassword = false,
                Phones = new List<PhoneEntity>() {
                    new PhoneEntity() {
                        NumDocument = "1515",
                        Telephone = "8098166516",
                        NumberSmsSent = 0,
                        Token = "",
                        MDCodeClient = "20102",
                        ExpitarationDate = DateTime.Now.AddDays(1),
                        Active = true
                    }
                }
            }
        };


        public IEnumerable<PhoneEntity> phoneEntities = new List<PhoneEntity>()
        {
            new PhoneEntity {

             }
        };

        public static IEnumerable<MediatorEntity> mediatorEntities = new List<MediatorEntity>()
        {
            new MediatorEntity()
            {
                Id = new Guid(),
                Instance = "DEV",
                Description = "Mediator de Desarrollo 21.15",
                Url = "http://172.21.18.15:8732/ExtremeMediator/Service",
                Active = true
            }
        };

        public IEnumerable<TransactionEntity> transactionEntities = new List<TransactionEntity>()
        {
            new TransactionEntity()
            {
                Name = "ValidateUser",
                Description = "Validar si el usuario exise con cedula y telefono",
                Channel = "SMS",
                TrnCode = "1805",
                MediatorId = mediatorEntities.FirstOrDefault().Id,
                Active = true
            },
            new TransactionEntity()
            {
                Name = "DetallePrestamos",
                Description = "Consulta Detalle de prestamos",
                Channel = "B2000",
                TrnCode = "0010",
                MediatorId = mediatorEntities.FirstOrDefault().Id,
                Active = true
            },
            new TransactionEntity()
            {
                Name = "DetalleCuentas",
                Description = "Consulta Detalle de cuentas",
                Channel = "B2000",
                TrnCode = "0007",
                MediatorId = mediatorEntities.FirstOrDefault().Id,
                Active = true
            },
            new TransactionEntity()
            {
                Name = "DetalleCertificados",
                Description = "Consulta Detalle de certificados",
                Channel = "B2000",
                TrnCode = "0008",
                MediatorId = mediatorEntities.FirstOrDefault().Id,
                Active = true
            },
            new TransactionEntity()
            {
                Name = "ConsultaProductos",
                Description = "Consulta Productos SMS",
                Channel = "B2000",
                TrnCode = "0006",
                MediatorId = mediatorEntities.FirstOrDefault().Id,
                Active = true
            },
            new TransactionEntity()
            {
                Name = "DetalleTarjetaCredito",
                Description = "Consulta Detalle tarjetas de creditos",
                Channel = "B2000",
                TrnCode = "0033",
                MediatorId = mediatorEntities.FirstOrDefault().Id,
                Active = true
            }
        };

        public IEnumerable<GeneralParameterEntity> generalParameterEntities = new List<GeneralParameterEntity>()
        {
           new GeneralParameterEntity()
            {
                Name = "MediatorKeyFields",
                NameAbrev = "MDSKeys",
                Description = "Parametro para varias busquedas en la respuesta del Mediator",
                Value = "qn+IuSV1woOiKjflxMrJ6X2a3jWldld7Aqs8/sXQEOc=$ubI0gxfNMplUeu9Mb6OuUw==$nNyc3EzvEVF1bv/x1r3Pc+P58lmIG/RbHlZ29xTUUvo=",
                Active = true
           },
            new GeneralParameterEntity()
            {
                Name = "Expitation Days Token",
                NameAbrev = "ExpitationDays",
                Description = "The days to expired the token",
                Value = "15",
                Active = true
           },
            new GeneralParameterEntity()
            {
                Name = "SMS Limit",
                NameAbrev = "SMSLimit",
                Description = "The limit of query of client can send sms",
                Value = "10",
                Active = true
           }
        };



    }
}