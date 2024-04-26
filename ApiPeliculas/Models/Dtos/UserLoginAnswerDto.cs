namespace ApiPeliculas.Models.Dtos
{
    public class UserLoginAnswerDto
    {
        //una vez los datos sean correctos,
        //quiero obtener el usuario
        //mostrar el nombre del usuario autenticado
        //token de validaciob jwt
        public UserDataDto user {  get; set; }
        public string Token { get; set; }
    }
}
