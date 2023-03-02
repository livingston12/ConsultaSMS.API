using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Responses;
using WebConsultaSMS.Utils;

namespace ConsultaSMS.Test
{
    public static class SeedTest
    {
        public static AuthenticateInternalRequest AuthInternalRequestIncorrectPass =
        new AuthenticateInternalRequest() { pPassword = "MiPassIncorrect" };
        
        public static AuthenticateInternalRequest AuthInternalRequest  =
        new AuthenticateInternalRequest() { pPassword = "MiCorrectPass" };

        public static AuthenticateResponse AuthInternalResponse = new AuthenticateResponse()
        {
            IsValid = false,
            Message = UtilsResponse.clienteConError
        };
    }
}