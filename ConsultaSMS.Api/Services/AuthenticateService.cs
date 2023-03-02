using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebConsultaSMS.DataBase;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Entities;
using WebConsultaSMS.Models.Enums;
using WebConsultaSMS.Models.Responses;
using WebConsultaSMS.Utils;

namespace WebConsultaSMS.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private IConfiguration _configuration;
        private ApiContext dbContext;
        public BaseService baseService { get; set; }
        private string userPin { get; set; } = string.Empty;
        private IHttpContextAccessor accessor;

        public AuthenticateService(
            ApiContext dbContext,
            IHttpContextAccessor accessor,
            IConfiguration config,
            IBaseService baseService
        )
        //: base(dbContext, accessor, baseService)
        {
            _configuration = config;
            this.dbContext = dbContext;
            this.accessor = accessor;

            this.baseService = new BaseService(dbContext, accessor);
            this.baseService._baseService = baseService;
        }

        public async Task<AuthenticateResponse> PostAsync(
            [Required, FromBody] AuthenticateRequest request
        )
        {
            AuthenticateResponse result = new AuthenticateResponse();
            try
            {
                if (request != null && request.pCedula != null && !string.IsNullOrEmpty(request.pTelefono))
                {
                    bool userExists = false;

                    UserEntity currentUser = await dbContext.Users.FirstOrDefaultAsync(
                        x =>
                            x.UserName == request.pTelefono
                            && x.Password == request.pTelefono
                            && x.Active
                    );
                    userExists = currentUser != null;
                    currentUser = currentUser ?? new UserEntity();
                    PhoneEntity phone = new PhoneEntity();
                    (PhoneEntity phone, bool exist) resultPhone = (null, false);

                    try
                    {
                        // validate if the phone is register on database
                        resultPhone = await ValidatePhone(request, currentUser);
                        phone = resultPhone.phone;
                    }
                    catch (MemberAccessException)
                    {
                        return new AuthenticateResponse()
                        {
                            Token = string.Empty,
                            IsValid = false,
                            ExpirationDate = null,
                            Message = UtilsResponse.clienteSinAccesoEnviarSMS
                        };
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            if (ex.InnerException.GetType() == typeof(SystemException))
                            {
                                string messageError = string.IsNullOrEmpty(request.pCedula) ?
                                                         UtilsResponse.clienteConError :
                                                         UtilsResponse.clienteConCedulaIncorrecta;

                                return new AuthenticateResponse()
                                {
                                    Token = string.Empty,
                                    IsValid = false,
                                    ExpirationDate = null,
                                    Message = messageError
                                };
                            }
                        }
                        currentUser = null;
                    }

                    if (currentUser != null && !string.IsNullOrEmpty(currentUser?.UserName))
                    {
                        //create claims details based on the user information
                        var claims = new List<Claim>();
                        claims.AddRange(
                            new List<Claim>()
                            {
                                new Claim(
                                    JwtRegisteredClaimNames.Sub,
                                    _configuration["Jwt:Subject"]
                                ),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                new Claim(
                                    "UserName",
                                    FunctionsHelper.Encryptor(currentUser.UserName)
                                ),
                                new Claim("Email", FunctionsHelper.Encryptor(currentUser.Email))
                            }
                        );
                        // Add all roles to user
                        AddRoles(claims, currentUser);

                        DateTime expirationDate =
                            phone?.ExpitarationDate ?? DateTime.Now.AddDays(1);

                        var userPhoneExist = userExists && resultPhone.exist; // If user and phone exist
                        string _token = GenerateToken(claims, expirationDate, !userPhoneExist);
                        _token = _token ?? phone?.Token; // If _token is null set phone?.Token;
                        expirationDate = phone?.ExpitarationDate ?? expirationDate;
                        result = new AuthenticateResponse()
                        {
                            Token = _token,
                            IsValid = true,
                            ExpirationDate = expirationDate,
                            Message = string.Empty
                        };

                        phone.Token = _token;
                        if (!userPhoneExist)
                        {
                            phone.Active = true;
                            await dbContext.Phones.AddAsync(phone);
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        result = new AuthenticateResponse()
                        {
                            Token = string.Empty,
                            IsValid = false,
                            ExpirationDate = null,
                            Message = UtilsResponse.clienteNoRegistrado
                        };
                    }
                }
                else
                {
                    throw new Exception(UtilsResponse.clienteConError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(UtilsResponse.clienteConError);
            }

            return result;
        }

        private void AddRoles(List<Claim> claims, UserEntity currentUser)
        {
            // Add Roles user from table rol
            if (currentUser?.Rol != null)
            {
                claims.Add(AddRole(currentUser.Rol.Name));
            }

            // Add roles to user one by one
            if (currentUser?.UserTransactions != null && currentUser.UserTransactions.Any())
            {
                // Add transactions To Roles
                foreach (UserTransactionEntity userTransaction in currentUser?.UserTransactions)
                {
                    if (userTransaction?.Transaction.Name != null)
                    {
                        claims.Add(AddRole(userTransaction.Transaction.Name));
                    }
                }
            }

            // Add general rol by transaction
            var roleTransactions = dbContext.RoleTransactions
                .Include(x => x.Transaction)
                .Where(x => x.Active);

            if (roleTransactions.Any())
            {
                foreach (var tran in roleTransactions)
                {
                    claims.Add(AddRole(tran.Transaction?.Name));
                }
            }
        }

        private string GenerateToken(
            List<Claim> claims,
            DateTime expirationDate,
            bool generateNew = true
        )
        {
            if (!generateNew)
            {
                return null;
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expirationDate,
                signingCredentials: signIn
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<(PhoneEntity phone, bool exist)> ValidatePhone(
            AuthenticateRequest request,
            UserEntity user
        )
        {
            PhoneEntity phoneResult = new PhoneEntity();
            bool phoneExist = false;
            var generalNumber = await dbContext
                                        .GeneralParameters
                                        .FirstOrDefaultAsync(x =>
                                                                x.NameAbrev == Generals.SMSLimit.ToString());

            int.TryParse(generalNumber.Value, out int numberSmsSent);
            var phone = await dbContext.Phones.FirstOrDefaultAsync(
                x =>
                    x.Telephone == request.pTelefono && x.Active
            );

            if (phone != null)
            {
                if (phone.NumberSmsSent >= numberSmsSent)
                {
                    throw new MemberAccessException();
                }
                phoneExist = true;
                phoneResult = phone;
            }
            if (phoneResult.MDCodeClient == "0" || string.IsNullOrEmpty(user.Pin))
            {
                await ValidatePhoneMDS(request, phoneResult, user);
            }

            return (phoneResult, phoneExist);
        }

        private async Task ValidatePhoneMDS(
            AuthenticateRequest request,
            PhoneEntity phone,
            UserEntity user
        )
        {
            phone.MDCodeClient = phone.MDCodeClient ?? "0";

            baseService.transactionName = "ValidateUser";
            // TODO get the value days from database to expirate the token
            var parameter = await dbContext.GeneralParameters.FirstOrDefaultAsync(
                x => x.NameAbrev == "ExpitationDays"
            );
            int.TryParse(parameter.Value, out int dayExpiration);
            DateTime expitarionDate = DateTime.Now.AddDays(dayExpiration);

            phone.CreatedDate = DateTime.Now;
            phone.ExpitarationDate = expitarionDate;
            userPin = phone.MDCodeClient != "0" ? request.pCedula : string.Empty;
            phone.NumDocument = phone.MDCodeClient == "0" ? request.pCedula : phone.NumDocument;
            phone.Telephone = request.pTelefono;
            request.pCedula = phone.NumDocument;

            // TODO: Call the MDS
            PhoneXmlResponse response = new PhoneXmlResponse();

            response = await baseService.ExecuteTransactionMDSAsync<AuthenticateRequest, PhoneXmlResponse>(
                request,
                response,
                "SMSValidate",
                "ExtremeMsgReply",
                false,
                ""
            );

            // TODO: If the user is valid insert user
            string clientId = response?.ClientId;
            if (!string.IsNullOrEmpty(clientId))
            {
                await ValidateUser(clientId, request, phone, user);
                phone.MDCodeClient = clientId;
                phone.UserdId = user.Id;
            }
            else
            {
                throw Utils.UtilsMethods.throwError(response?.Message);
            }
        }

        private async Task ValidateUser(
            string clientId,
            AuthenticateRequest request,
            PhoneEntity phone,
            UserEntity user
        )
        {
            bool create = string.IsNullOrEmpty(request.pCedula) || string.IsNullOrEmpty(phone.NumDocument);

            if (create && (user == null || string.IsNullOrEmpty(user.Name)))
            {
                string name = string.IsNullOrEmpty(request.pCedula) ? request.pTelefono : request.pCedula;
                var rolId = Utils.UtilsMethods.GetRolId(dbContext);
                user.MDCodeClient = clientId;
                user.Email = name;
                user.UserName = name;
                user.Name = $"external-{name}";
                user.Password = request.pTelefono;
                user.RolId = rolId;
                user.CreatedDate = DateTime.Now;
                user.IsEncriptPassword = false;
                user.Active = true;

                await dbContext.Users.AddAsync(user);
            }
            else
            {
                if (string.IsNullOrEmpty(phone.NumDocument))
                {
                    phone.NumDocument = request.pCedula;
                    user.MDCodeClient = clientId;
                }
                else if (string.IsNullOrEmpty(user.Pin))
                {
                    user.Pin = userPin;
                }
                else
                {
                    return;
                }

                dbContext.Entry(user).State = EntityState.Modified;
            }
        }

        private Claim AddRole(string rol)
        {
            return new Claim(ClaimTypes.Role, rol);
        }

        private async Task<UserEntity> GetUserAsync(string numDocument, string telephone)
        {
            UserEntity result = null;

            var currentUser = await dbContext.Users
                .Include(x => x.Rol)
                .Include(x => x.UserTransactions)
                .FirstOrDefaultAsync(
                    x => (x.UserName == numDocument || x.Email == numDocument) && x.Active
                );

            if (currentUser != null)
            {
                string passwordEncript = currentUser.IsEncriptPassword
                    ? UtilsMethods.EncodeBase64(telephone)
                    : telephone;

                if (currentUser.Password == passwordEncript)
                {
                    result = currentUser;
                }
            }

            return result;
        }

        public async Task<AuthenticateResponse> PostInternalAsync(
            AuthenticateInternalRequest request
        )
        {
            AuthenticateResponse result = new AuthenticateResponse();
            try
            {
                if (request != null && request.pPassword != null)
                {
                    bool userExists = false;

                    IEnumerable<UserEntity> ListcurrentUser = dbContext.Users.Where(
                        x => x.UserName == "BMSC-AFE" && x.Active
                    );
                    string password = GetPassWord(
                        ListcurrentUser?.FirstOrDefault()?.IsEncriptPassword,
                        ListcurrentUser?.FirstOrDefault()?.Password
                    );
                    var currentUser = ListcurrentUser.FirstOrDefault(
                        x => password == request.pPassword
                    );
                    userExists = currentUser != null;

                    if (currentUser != null)
                    {
                        //create claims details based on the user information
                        var claims = new List<Claim>();
                        claims.AddRange(
                            new List<Claim>()
                            {
                                new Claim(
                                    JwtRegisteredClaimNames.Sub,
                                    _configuration["Jwt:Subject"]
                                ),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                new Claim(
                                    "UserName",
                                    FunctionsHelper.Encryptor(currentUser.UserName)
                                ),
                                new Claim("Email", FunctionsHelper.Encryptor(currentUser.Email))
                            }
                        );
                        // Add all roles to user
                        AddRoles(claims, currentUser);

                        var parameter = await dbContext.GeneralParameters.FirstOrDefaultAsync(
                            x => x.NameAbrev == "ExpitationDays"
                        );
                        int.TryParse(parameter.Value, out int dayExpiration);
                        DateTime? currentDate = DateTime.Now.AddDays(dayExpiration);
                        DateTime expirationDate = currentDate ?? DateTime.Now.AddDays(1);

                        string _token = GenerateToken(claims, expirationDate, userExists);
                        bool validToken = _token != null;

                        result = new AuthenticateResponse()
                        {
                            Token = _token,
                            IsValid = validToken,
                            ExpirationDate = validToken ? expirationDate : null,
                            Message = string.Empty
                        };
                    }
                    else
                    {
                        result = new AuthenticateResponse()
                        {
                            Token = string.Empty,
                            IsValid = false,
                            ExpirationDate = null,
                            Message = UtilsResponse.clienteNoRegistrado
                        };
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new Exception(UtilsResponse.clienteConError);
            }

            return result;
        }

        private string GetPassWord(bool? isEncriptPassword, string password)
        {
            string result = string.Empty;
            if (isEncriptPassword != null)
            {
                result = isEncriptPassword.Value ? FunctionsHelper.Decrypt(password) : password;
            }

            return result;
        }

        public async Task<(string message, bool next)> ValidateUserExist(AuthenticateRequest request)
        {
            (string message, bool next) = (UtilsResponse.clienteConError, false);
            bool isNext = true;


            var Uservalid = await dbContext
                                     .Phones
                                     .Include(x => x.User)
                                     .FirstOrDefaultAsync(x => x.Telephone == request.pTelefono);

            if (Uservalid == null)
            {
                throw new Exception(UtilsResponse.clienteNoRegistrado);
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == Uservalid.UserdId);
            string pin = user.Pin;
            bool needValidPin = (string.IsNullOrEmpty(pin) && !string.IsNullOrEmpty(Uservalid?.NumDocument)) || (request.pCedula != pin && !string.IsNullOrEmpty(Uservalid?.NumDocument));
            string valuePin = request.pCedula; // If the value is a text like ----> balance
            string document = Uservalid?.NumDocument;
            bool isNew = request.pCedula == document && !string.IsNullOrEmpty(pin);

            if (Uservalid?.MDCodeClient != "0" && !needValidPin && string.IsNullOrEmpty(valuePin))
            {
                next = true;
                isNext = false;
            }
            else if (isNext && (string.IsNullOrEmpty(document) || Uservalid?.MDCodeClient == "0" || needValidPin))
            {
                message = UtilsResponse.clienteSinCedulaRegistrada;
                if (needValidPin)
                {
                    if (string.IsNullOrEmpty(document))
                    {
                        var isValid = await SetDocument(request, Uservalid.UserdId);
                        if (isValid)
                        {
                            message = UtilsResponse.clienteSinPin;
                            isNext = false;
                        }
                    }
                    else if (valuePin == document && !string.IsNullOrEmpty(pin))
                    {
                        message = UtilsResponse.clienteSinPin;
                        isNext = false;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(document) && valuePin == document)
                    {
                        message = UtilsResponse.clienteSinPin;
                    }
                    else
                    {
                        message = UtilsResponse.clienteSinCedulaRegistrada;
                    }

                    isNext = false;
                }
            }

            // TODO validate if the user have cedula and dont have pin
            if (isNext && needValidPin)
            {
                message = UtilsResponse.clienteSinPin;

                if (needValidPin)
                {
                    if (string.IsNullOrEmpty(document))
                    {
                        next = await SetDocument(request, Uservalid.UserdId);
                    }
                    else if (valuePin == pin)
                    {
                        next = true;
                    }
                    else if (request.pCedula == document && !string.IsNullOrEmpty(pin))
                    {
                        next = true;
                    }
                    else if (valuePin != pin && !string.IsNullOrEmpty(valuePin) && string.IsNullOrEmpty(pin))
                    {
                        if (valuePin != document)
                        {
                            message = UtilsResponse.clienteConCedulaIncorrecta; // BEFORE clienteConPinIncorrecto
                        }
                        isNext = false;
                    }
                    else if (!string.IsNullOrEmpty(pin) && string.IsNullOrEmpty(valuePin))
                    {
                        message = UtilsResponse.clienteValidarPin;
                        isNext = false;
                    }
                    else if (!string.IsNullOrEmpty(pin) && !string.IsNullOrEmpty(valuePin))
                    {
                        if (valuePin.ToString().Contains("ba"))
                        {
                            message = UtilsResponse.clienteValidarPin;
                        }
                        else if (valuePin != pin)
                        {
                            message = UtilsResponse.clienteConPinIncorrecto;
                        }
                        isNext = false;
                    }
                    else
                    {
                        isNext = false;
                    }
                }
            }
            else if (isNew)
            {
                isNext = true;
            }
            else if (isNext && pin != valuePin)
            {
                message = UtilsResponse.clienteSinPin;
                isNext = false;
            }

            if (isNext || next)
            {
                message = Uservalid.MDCodeClient;
                next = true;
            }

            message = $"{message}-{isNew.ToString()}";

            return (message, next);
        }

        private async Task<bool> SetDocument(AuthenticateRequest request, Guid userId)
        {
            var response = await PostAsync(request);
            return response.IsValid;
        }
    }
}
