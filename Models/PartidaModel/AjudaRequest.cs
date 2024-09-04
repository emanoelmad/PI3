namespace AppShowDoMilhao.Models.PartidaModel
{
    public class AjudaRequest
    {
        public int PartidaId { get; set; }
        public int PerguntaId { get; set; }
        public string TipoAjuda { get; set; }  // Tipo de ajuda solicitada (e.g., "EliminarDuas")
    }
}
