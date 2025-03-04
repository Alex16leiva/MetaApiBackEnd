using Aplicacion.Core;
using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;
using Infraestructura.Core.Jwtoken;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Aplicacion.Services
{
    public class SecurityAplicationService : BaseDisposable
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public SecurityAplicationService(IGenericRepository<IDataContext> genericRepository, ITokenService tokenService, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public UsuarioDTO EditarUsuario(EdicionUsuarioRequest request)
        {
            string mensajeValidacion = request.Usuario.ValidarCampos();

            if (mensajeValidacion.HasValue())
            {
                return new UsuarioDTO
                {
                    Message = request.Usuario.Message,
                };
            }

            Usuario usuarioExiste = _genericRepository.GetSingle<Usuario>(r => r.UsuarioId == request.Usuario.UsuarioId);

            if (usuarioExiste.IsNull())
            {
                return new UsuarioDTO
                {
                    Message = "El usuario no existe"
                };
            }

            if (request.Usuario.EditarContrasena)
            {
                usuarioExiste.Contrasena = PasswordEncryptor.Encrypt(request.Usuario.Contrasena);
            }

            usuarioExiste.Nombre = request.Usuario.Nombre.ValueOrEmpty();
            usuarioExiste.Apellido = request.Usuario.Apellido.ValueOrEmpty();
            usuarioExiste.RolId = request.Usuario.RolId.ValueOrEmpty();

            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("EditarUsuario");
            _genericRepository.UnitOfWork.Commit(transactionInfo);
            return new UsuarioDTO();
        }

        public UsuarioDTO CrearUsuario(EdicionUsuarioRequest request)
        {
            string mensajeValidacion = request.Usuario.ValidarCampos();

            if (mensajeValidacion.HasValue())
            {
                return new UsuarioDTO
                {
                    Message = request.Usuario.Message,
                };
            }

            Usuario usuarioExiste = _genericRepository.GetSingle<Usuario>(r => r.UsuarioId == request.Usuario.UsuarioId);

            if (usuarioExiste.IsNotNull())
            {
                return new UsuarioDTO
                {
                    Message = "Usuario ya esta registrado"
                };
            }

            var usuario = new Usuario
            {
                Apellido = request.Usuario.Apellido.ValueOrEmpty(),
                Contrasena = PasswordEncryptor.Encrypt(request.Usuario.Contrasena),
                Nombre = request.Usuario.Nombre.ValueOrEmpty(),
                RolId = request.Usuario.RolId.ValueOrEmpty(),
                UsuarioId = request.Usuario.UsuarioId.ValueOrEmpty(),
            };
            
            _genericRepository.Add(usuario);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("AgregarUsuario");
            _genericRepository.UnitOfWork.Commit(transactionInfo);
            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public UsuarioDTO IniciarSesion(UserRequest request)
        {
            List<string> includes = ["Rol"];

            string passwordEncrypted = PasswordEncryptor.Encrypt(request?.Password);

            Usuario usuario = _genericRepository.GetSingle<Usuario>(r => r.UsuarioId == request.UsuarioId && r.Contrasena == passwordEncrypted);

            if (usuario.IsNotNull())
            {
                return new UsuarioDTO
                {
                    Apellido = usuario.Apellido,
                    Nombre = usuario.Nombre,
                    RolId = usuario.RolId,
                    Token = _tokenService.Generate(usuario),
                    UsuarioAutenticado = true,
                    UsuarioId = usuario.UsuarioId
                };
            }

            return new UsuarioDTO
            {
                Message = "Usuario o Contraseña no valido",
                UsuarioAutenticado = false
            };
        }

        public SearchResult<UsuarioDTO> ObtenerUsuario(GetUserRequest request)
        {
            var dynamicFilter = DynamicFilterFactory.CreateDynamicFilter(request.QueryInfo);
            var usuarios = _genericRepository.GetPagedAndFiltered<Usuario>(dynamicFilter);

            return new SearchResult<UsuarioDTO>
            { 
                PageCount = usuarios.PageCount,
                ItemCount = usuarios.ItemCount,
                TotalItems = usuarios.TotalItems,
                PageIndex = usuarios.PageIndex,
                Items = (from qry in usuarios.Items as IEnumerable<Usuario> select MapUsuarioDto(qry)).ToList(),
            };

        }

        public SearchResult<RolDTO> ObtenerRoles(GetRolRequest request)
        {
            var dynamicFilter = DynamicFilterFactory.CreateDynamicFilter(request.QueryInfo);
            var roles = _genericRepository.GetPagedAndFiltered<Rol>(dynamicFilter);

            return new SearchResult<RolDTO>
            {
                PageCount = roles.PageCount,
                ItemCount = roles.ItemCount,
                TotalItems = roles.TotalItems,
                PageIndex = roles.PageIndex,
                Items = (from qry in roles.Items as IEnumerable<Rol> select MapRolDto(qry)).ToList(),
            };
        }

        private static RolDTO MapRolDto(Rol qry)
        {
            return new RolDTO
            {
                Descripcion = qry.Descripcion,
                RolId = qry.RolId     
            };
        }

        private static UsuarioDTO MapUsuarioDto(Usuario qry)
        {
            return new UsuarioDTO
            {
                Apellido = qry.Apellido,
                Contrasena = qry.Contrasena,
                Nombre = qry.Nombre,
                RolId = qry.RolId,  
                UsuarioId = qry.UsuarioId,
                FechaTransaccion = qry.FechaTransaccion,
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_genericRepository.IsNotNull()) _genericRepository.Dispose();
                
            }

            base.Dispose(disposing);
        }
    }
}
