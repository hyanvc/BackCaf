namespace BackCaf.Models
{
    public class ProdutoItemRequest
    {
        public string Tipo { get; set; }
        public bool LeiteDeAveia { get; set; }
        public bool Canela { get; set; }
        public bool SemAcucar { get; set; }
        public int Quantidade { get; set; } = 1;
    }
}