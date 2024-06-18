using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PruebaApi.Config;
using PruebaApi.DTO;
using PruebaApi.Modelo;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace PruebaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IDbConnection _dbConection;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuariosController(IDbConnection dbConection, IMapper mapper, IConfiguration configuration)
        {
            _dbConection = dbConection;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpPost("register")]
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Usuario", loginDto.Usuario);

            var query = "EXEC Usuario_Obtener @Usuario";
            var user = await _dbConection.QueryFirstOrDefaultAsync<User>(query, parameters);

            // Verificar si el usuario es nulo o si la contraseña no coincide
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Contrasena, user.Contrasena))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            if (string.IsNullOrEmpty(user.Usuario))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User data is incomplete" });
            }

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Usuario),
                    new Claim(ClaimTypes.Role, "User")
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            { 
                token = tokenString,
                user = new 
                {
                    user.Id,
                    user.Nombre,
                    user.Correo,
                    user.IdCategoria
                }
            });
        }
    }
}
