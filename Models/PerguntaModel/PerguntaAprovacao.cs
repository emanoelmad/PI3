using AppShowDoMilhao.Models.UsuarioModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models.PerguntaModel
{
    public class PerguntaAprovacao
    {
        [Key]
        public int IdAprovacao { get; set; }  // Chave primária
        public int IdPergunta { get; set; }  // Chave estrangeira para a pergunta
        public int IdUsuario { get; set; }  // Chave estrangeira para o usuário
        public bool? Aprovado { get; set; }  // Indica se a pergunta foi aprovada ou rejeitada (null inicialmente)
        public DateTime? DataAprovacao { get; set; }  // Data e hora da aprovação/rejeição

    }
}
