namespace WebApiAutores.Entidades
{
    public class AutorLibro
    {
        //La FK es una composición de  LibroId y AutorId. Esto se hace con el ApiFluente
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set;}
        public Libro Libro { get; set;}
        public Autor Autor { get; set;}
    }
}
