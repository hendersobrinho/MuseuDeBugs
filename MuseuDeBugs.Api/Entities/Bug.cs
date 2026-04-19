using MuseuDeBugs.Api.Enums;


namespace MuseuDeBugs.Api.Entities
{
    public class  Bug
    {
        public int Id { get; private set;}
        public string Titulo { get; private set;} = null!; // O operador null-forgiving (!) é usado para indicar que a propriedade Titulo não será nula, mesmo que o tipo string seja anulável por padrão. Isso é útil para evitar avisos de compilação relacionados a possíveis valores nulos.
        public string Linguagem { get; private set; } = null!;
        public string? MensagemErro { get; private set; } = null!;
        public string Descricao { get; private set; } = null!;
        public string? Causa { get; private set; } = null!;
        public string? Solucao {get; private set;} = null!;
        public StatusBug Status { get; private set;}
        public DateTime DataCriacao { get; private set;}
        public DateTime? DataAtualizacao { get; private set;}

        public Bug (
            string titulo, 
            string linguagem, 
            string? mensagemErro,
            string descricao, 
            string? causa,
            string? solucao)
        {
            Titulo = titulo;
            Linguagem = linguagem;
            MensagemErro = mensagemErro;
            Descricao = descricao;
            Causa = causa;
            Solucao = solucao;
            Status = StatusBug.Aberto;
            DataCriacao = DateTime.UtcNow;
        }
        private Bug() //Construtor vazio para persistencia
        {   
        }
        public void  MarcarComoResolvido()
        {
            if(Status == StatusBug.Resolvido)
            {
                return;;        
            }
            Status = StatusBug.Resolvido;
            DataAtualizacao = DateTime.UtcNow; //Retorna a data corrente 
        }   
            

    }
}