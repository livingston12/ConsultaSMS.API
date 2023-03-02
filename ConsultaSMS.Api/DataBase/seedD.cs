using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebConsultaSMS.Models.Entities;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.DataBase
{
    public class SeedD
    {
        private ApiContext dbContext;

        public SeedD(ApiContext context)
        {
            dbContext = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                await CheckMediators();
                await CheckGeneralParameters();
                await CheckRoles();
                await CheckUsers();
                await checkTransaction();
                await ChekRoleTransaction();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async Task CheckUsers()
        {
            var afe = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == "BMSC-AFE");
            int rolId = (int)Roles.User;

            if (afe == null)
            {
                await dbContext.Users.AddAsync(
                    new UserEntity()
                    {
                        Name = "AFE External user to login",
                        UserName = "BMSC-AFE",
                        Email = "external-BMSC-AFE",
                        IsEncriptPassword = true,
                        CreatedDate = DateTime.Now,
                        Password = FunctionsHelper.Encryptor("EXTERNAL-BMSC-AFE"),
                        RolId = rolId,
                        MDCodeClient = string.Empty,
                        Pin = string.Empty,
                        Active = true
                    }
                );
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task ChekRoleTransaction()
        {
            List<string> transactionsInsert = new List<string>
            {
                Generals.ConsultaProductos.ToString(),
                Generals.ValidateUser.ToString(),
                Generals.DetalleCuentas.ToString(),
                Generals.DetalleTarjetaCredito.ToString(),
                Generals.DetallePrestamos.ToString()
            };
            // TODO Search externalRols on the database
            IEnumerable<RoleTransactionEntity> externalRols = dbContext.RoleTransactions
                .Where(x => x.Rol.Name == Roles.User.ToString() && x.Active)
                .Include(x => x.Rol);

            // TODO get the rolId of external user
            int rolId = dbContext.Roles
                .FirstOrDefaultAsync(x => x.Name == Roles.User.ToString() && x.Active)
                .GetAwaiter()
                .GetResult()
                .RolId;

            // TODO get all transaction default
            IEnumerable<Guid> transactionIds = dbContext.Transactions
                .Where(x => x.Active && transactionsInsert.Contains(x.Name))
                .Select(x => x.Id);

            if (!externalRols.Any())
            {
                foreach (Guid tranId in transactionIds)
                {
                    await AddRoleTransaction(rolId, tranId);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private async Task AddRoleTransaction(int rolId, Guid transactionId)
        {
            await dbContext.RoleTransactions.AddRangeAsync(
                new List<RoleTransactionEntity>()
                {
                    new RoleTransactionEntity()
                    {
                        Active = true,
                        RolId = rolId,
                        TransactionId = transactionId
                    }
                }
            );
        }

        private async Task checkTransaction()
        {
            List<string> transactionsInsert = new List<string>
            {
                Generals.ConsultaProductos.ToString(),
                Generals.ValidateUser.ToString()
            };

            var transactions = dbContext.Transactions.Where(
                x => transactionsInsert.Contains(x.Name)
            );

            if (transactions == null || !transactions.Any())
            {
                await AddTransaction();
            }
        }

        private async Task AddTransaction()
        {
            var mediator = await dbContext.Mediators.FirstOrDefaultAsync(x => x.Active);
            await dbContext.Transactions.AddRangeAsync(
                new List<TransactionEntity>()
                {
                    new TransactionEntity()
                    {
                        Name = Generals.ConsultaProductos.ToString(),
                        Description = "Consulta Productos SMS",
                        Active = true,
                        CreatedDate = DateTime.Now,
                        TrnCode = "2106",
                        MediatorId = mediator.Id,
                        Channel = "CHANEL",
                    },
                    new TransactionEntity()
                    {
                        Name = Generals.DetalleCuentas.ToString(),
                        Description = "Consulta Detalle de cuentas",
                        Active = true,
                        CreatedDate = DateTime.Now,
                        TrnCode = "2109",
                        MediatorId = mediator.Id,
                        Channel = "CHANEL",
                    },
                    new TransactionEntity()
                    {
                        Name = Generals.DetalleCertificados.ToString(),
                        Description = "Consulta Detalle de certificados",
                        Active = true,
                        CreatedDate = DateTime.Now,
                        TrnCode = "2108",
                        MediatorId = mediator.Id,
                        Channel = "CHANEL",
                    },
                    new TransactionEntity()
                    {
                        Name = Generals.DetallePrestamos.ToString(),
                        Description = "Consulta Detalle de prestamos",
                        Active = true,
                        CreatedDate = DateTime.Now,
                        TrnCode = "2110",
                        MediatorId = mediator.Id,
                        Channel = "CHANEL",
                    },
                    new TransactionEntity()
                    {
                        Name = Generals.DetalleTarjetaCredito.ToString(),
                        Description = "Consulta Detalle tarjetas de creditos",
                        Active = true,
                        CreatedDate = DateTime.Now,
                        TrnCode = "2133",
                        MediatorId = mediator.Id,
                        Channel = "CHANEL",
                    },
                    new TransactionEntity()
                    {
                        Name = "ValidateUser",
                        Description = "Validar si el usuario exise con cedula y telefono",
                        Active = true,
                        CreatedDate = DateTime.Now,
                        TrnCode = "2105",
                        MediatorId = mediator.Id,
                        Channel = "SMS"
                    },
                }
            );
            int inserted = await dbContext.SaveChangesAsync();
            if (inserted <= 0)
            {
                throw new Exception($"Problem to insert the initial data (mediator) to dataBase");
            }
        }

        private async Task CheckRoles()
        {
            List<string> rolesPermited = new List<string>() { "Admin", "User", "ExternalUser" };

            var roles = dbContext.Roles.Where(
                x => x.Name == "Admin" || x.Name == "User" || x.Name == "ExternalUser"
            );

            if (roles == null)
            {
                await AddRoles();
            }
            else if (roles.Count() < 3)
            {
                var listRols = listRoles();
                var insertedRols = listRols.Where(x => !roles.Select(x => x.Name).Contains(x.Name));
                dbContext.Roles.AddRange(insertedRols);
                int inserted = await dbContext.SaveChangesAsync();
                if (inserted <= 0)
                {
                    throw new Exception($"Problem to insert the initial data (Roles) to dataBase");
                }
            }
        }

        private async Task AddRoles()
        {
            dbContext.Roles.AddRange(listRoles());
            int inserted = await dbContext.SaveChangesAsync();
            if (inserted <= 0)
            {
                throw new Exception($"Problem to insert the initial data (Roles) to dataBase");
            }
        }

        private List<RolEntity> listRoles()
        {
            return new List<RolEntity>()
            {
                new RolEntity()
                {
                    Name = Roles.Admin.ToString(),
                    Description = "Adiministradores",
                    Active = true
                },
                new RolEntity()
                {
                    Name = Roles.User.ToString(),
                    Description =
                        "Usuarios debe insertar las transaciones que necesita acceder en la tabla userTransactions",
                    Active = true
                },
                new RolEntity()
                {
                    Name = Roles.ExternalUser.ToString(),
                    Description =
                        "Usuarios debe insertar las transaciones que necesita acceder en la tabla userTransactions",
                    Active = true
                }
            };
        }

        private async Task CheckGeneralParameters()
        {
            var generalParameter = dbContext.GeneralParameters
                .FirstOrDefaultAsync(x => x.NameAbrev == Generals.MDSKeys.ToString() && x.Active)
                .GetAwaiter()
                .GetResult();

            var expitationTimeToken = dbContext.GeneralParameters
                .FirstOrDefaultAsync(
                    x => x.NameAbrev == Generals.ExpitationDays.ToString() && x.Active
                )
                .GetAwaiter()
                .GetResult();

            var smsLimit = dbContext.GeneralParameters
                .FirstOrDefaultAsync(
                    x => x.NameAbrev == Generals.SMSLimit.ToString() && x.Active
                )
                .GetAwaiter()
                .GetResult();

            if (generalParameter == null)
            {
                await validateGeneralParameters();
            }

            if (expitationTimeToken == null)
            {
                generalParameter = new GeneralParameterEntity()
                {
                    Name = "Expitation Days Token",
                    NameAbrev = Generals.ExpitationDays.ToString(),
                    CreatedDate = DateTime.Now,
                    Description = "The days to expired the token",
                    Value = "15",
                    Active = true
                };
                await dbContext.GeneralParameters.AddAsync(generalParameter);
                await dbContext.SaveChangesAsync();
            }

            if (smsLimit == null)
            {
                generalParameter = new GeneralParameterEntity()
                {
                    Name = "SMS Limit ",
                    NameAbrev = Generals.SMSLimit.ToString(),
                    CreatedDate = DateTime.Now,
                    Description = "The limit of query of client can send sms",
                    Value = "10",
                    Active = true
                };

                await dbContext.GeneralParameters.AddAsync(generalParameter);
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckMediators()
        {
            var mediators = dbContext.Mediators.Where(
                x =>
                    x.Instance == "DEV"
                    || x.Instance == "QA"
                    || x.Instance == "PROD"
            );
            await validateMediators(mediators);
        }

        private async Task<bool> validateGeneralParameters()
        {
            await AddGeneralParameter(
                "MediatorKeyFields",
                "XMLResponse,ResponseXML",
                Generals.MDSKeys.ToString(),
                "Parametro para varias busquedas en la respuesta del Mediator"
            );

            return await dbContext.SaveChangesAsync() > 0;
        }

        private async Task validateMediators(IEnumerable<MediatorEntity> mediators)
        {
            if (mediators.Count() != 5)
            {
                dbContext.Mediators.RemoveRange(mediators);
                var inserted = await InsertMediatorData();
                if (!inserted)
                {
                    throw new Exception(
                        $"Problem to insert the initial data ({nameof(mediators)}) to dataBase"
                    );
                }
            }
        }

        private async Task<bool> InsertMediatorData()
        {
            List<MediatorEntity> mediators = new List<MediatorEntity>()
            {
                new MediatorEntity()
                {
                    Id = new Guid(),
                    Description = "Mediator de Desarrollo",
                    Instance = "DEV",
                    Url = "URLSERVICE",
                    Active = true,
                    CreatedDate = DateTime.Now
                },
                new MediatorEntity()
                {
                    Id = new Guid(),
                    Description = "Mediator de QA",
                    Instance = "QA",
                    Url = "URLSERVICE",
                    Active = false,
                    CreatedDate = DateTime.Now
                },
                new MediatorEntity()
                {
                    Id = new Guid(),
                    Description = "Mediator de Produccion",
                    Instance = "PROD",
                    Url = "URLSERVICE",
                    Active = false,
                    CreatedDate = DateTime.Now
                },
            };
            await dbContext.Mediators.AddRangeAsync(mediators);

            return await dbContext.SaveChangesAsync() > 0;
        }

        private async Task AddGeneralParameter(
            string name,
            string value,
            string nameAbrev,
            string description = ""
        )
        {
            await dbContext.GeneralParameters.AddAsync(
                new GeneralParameterEntity
                {
                    Name = name,
                    Value = FunctionsHelper.Encryptor(value),
                    CreatedDate = DateTime.Now,
                    Description = description,
                    NameAbrev = nameAbrev,
                    Active = true,
                }
            );
        }
    }
}
