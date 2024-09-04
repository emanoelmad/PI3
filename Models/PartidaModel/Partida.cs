using AppShowDoMilhao.Models.UsuarioModel;
using System;
using System.ComponentModel.DataAnnotations;

public class Partida
{
        [Key]
        public int IdPartida { get; set; }

        public int IdUsuario { get; set; }

        public DateTime DataPartida { get; set; }

        public decimal PremioObtido { get; set; }

        // Status atual da partida (ex.: "Em Andamento", "Vitória", "Derrota")
        public string Status { get; set; }

        public int NumPerguntasRespondidas { get; set; } // Número de perguntas respondidas

        public int NumRespostasCertas { get; set; } // Número de respostas corretas

    public DateTime? DataDelecao { get; set; }
}
