using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Responses;

namespace WebConsultaSMS.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        public IAuthenticateService _authenticateService;

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(AuthenticateInternalRequest request)
        {
            AuthenticateResponse response = new AuthenticateResponse();
            try
            {
                response = await _authenticateService.PostInternalAsync(request);
            }
            catch (System.Exception)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
