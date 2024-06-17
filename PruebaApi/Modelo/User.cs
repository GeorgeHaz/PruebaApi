namespace PruebaApi.Modelo
{
    public class User
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public string Correo { get; set; }
        public int IdCategoria { get; set; } = 1;
        public string Creado_Por { get; set; } = "API";
        public DateTime Fecha { get; set; }
        public byte Inactivo { get; set; } = 1;
    }
}
