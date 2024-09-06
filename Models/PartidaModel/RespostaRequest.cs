namespace AppShowDoMilhao.Models.PartidaModel
{
    public class RespostaRequest
    {
        public int PartidaId { get; set; }     
        public int PerguntaId { get; set; }     
        public string RespostaEscolhida { get; set; }  
        public bool DecidiuParar { get; set; }  
    }

}
