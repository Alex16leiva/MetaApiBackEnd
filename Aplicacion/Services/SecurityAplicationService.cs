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

namespace Aplicacion.Services
{
    public class SecurityAplicationService : BaseDisposable
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private static int _idCounter = 0;
        public SecurityAplicationService(IGenericRepository<IDataContext> genericRepository, ITokenService tokenService, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public UsuarioDTO CrearUsuario(CreateUserRequest request)
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
                Nombre = request.Usuario.Apellido.ValueOrEmpty(),
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
            _idCounter = 0;
            return new SearchResult<UsuarioDTO>
            { 
                PageCount = usuarios.PageCount,
                ItemCount = usuarios.ItemCount,
                TotalItems = usuarios.TotalItems,
                PageIndex = usuarios.PageIndex,
                Items = (from qry in usuarios.Items as IEnumerable<Usuario> select MapUsuarioDto(qry)).ToList(),
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
                Id = _idCounter++,
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
