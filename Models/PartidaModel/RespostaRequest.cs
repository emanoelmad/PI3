namespace AppShowDoMilhao.Models.PartidaModel
{
    public class RespostaRequest
    {
        public int PartidaId { get; set; }
        public int PerguntaId { get; set; }
        public string RespostaEscolhida { get; set; }  // Alternativa escolhida pelo usuário
    }
}
