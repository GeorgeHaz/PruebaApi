using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaApi.Config;
using PruebaApi.DTO;
using System.Data;

namespace PruebaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IDbConnection _dbConection;
        private readonly IMapper _mapper;

        public UsuariosController(IDbConnection dbConection, IMapper mapper)
        {
            _dbConection = dbConection;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationDto userDto)
        {
            //validar modelo
            var validator = new UserRegistrationValidator();
            var validationResult = validator.Validate(userDto);

            if(!validationResult.IsValid) 
            {
                return BadRequest(validationResult.Errors);
            }

            //Encriptar la contraseña
            userDto.Contrasena = BCrypt.Net.BCrypt.HashPassword(userDto.Contrasena);

            var parameters = new DynamicParameters();
            parameters.Add("Id", userDto.Id);
            parameters.Add("Nombre", userDto.Nombre);
            parameters.Add("Usuario", userDto.Usuario);
            parameters.Add("Contrasena", userDto.Contrasena);
            parameters.Add("Correo", userDto.Correo);
            parameters.Add("IdCategoria", userDto.IdCategoria);
            parameters.Add("Creado_Por", userDto.Creado_Por);
            parameters.Add("Inactivo", userDto.Inactivo);

            //Ejecutar procedimientos almacenados
            var query = "EXEC dbo.Usuario_Insert @Id, @Nombre, @Usuario, @Contrasena, @Correo, @IdCategoria, @Creado_Por, @Inactivo";
            await _dbConection.ExecuteAsync(query, parameters);

            return Ok(new { message = "User registered successfully"});
        }
    }
}
