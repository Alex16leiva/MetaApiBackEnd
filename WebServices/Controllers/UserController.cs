using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;
using Aplicacion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SecurityAplicationService _securityAppService;
        public UserController(SecurityAplicationService securityAppService) 
        {
            _securityAppService = securityAppService;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public UsuarioDTO Login([FromForm] UserRequest request)
        {
            UsuarioDTO usuario = _securityAppService.IniciarSesion(request);

            return usuario;
        }

        [Authorize]
        [HttpPost("crear-usuario")]
        public UsuarioDTO CreateUser(CreateUserRequest request)
        {
            UsuarioDTO usuario = _securityAppService.CrearUsuario(request);

            return usuario ;
        }

        [Authorize]
        [HttpPost("obtener-usuarios")]
        public SearchResult<UsuarioDTO> ObtenerUsuarios(GetUserRequest request)
        {
            var usuarios = _securityAppService.ObtenerUsuario(request);
            return usuarios;
        }
    }
}
