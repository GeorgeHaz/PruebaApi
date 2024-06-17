using FluentValidation;
using PruebaApi.DTO;

namespace PruebaApi.Config
{
    public class UserRegistrationValidator:AbstractValidator<UserRegistrationDto>
    {
        public UserRegistrationValidator() 
        {
            RuleFor(x=> x.Id).GreaterThan(0);
            RuleFor(x=> x.Nombre).NotEmpty().MaximumLength(100);
            RuleFor(x=> x.Usuario).NotEmpty().MaximumLength(50);
            RuleFor(x=> x.Contrasena).NotEmpty().MaximumLength(500);
            RuleFor(x=> x.Correo).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Creado_Por).MaximumLength(50);
        }
    }
}
