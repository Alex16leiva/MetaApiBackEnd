namespace Aplicacion.DTOs.Seguridad
{
    public class UserRequest
    {
        public string? UsuarioId { get; set; }
        public string? Password { get; set; }
    }

    public class EdicionUsuarioRequest : RequestBase
    {
        public UsuarioDTO? Usuario { get; set; }
    }

    public class GetUserRequest : RequestBase { }

    public class GetRolRequest : RequestBase { }
}
