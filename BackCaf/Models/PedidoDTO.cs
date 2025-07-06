using System.Collections.Generic;

namespace BackCaf.Models
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Status { get; set; }
        public List<ProdutoPedidoDTO> Produtos { get; set; }
        public string TipoPagamento { get; set; } // Novo campo
    }   

    public class ProdutoPedidoDTO
    {
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public bool LeiteDeAveia { get; set; }
        public bool Canela { get; set; }
        public bool SemAcucar { get; set; }
    }
}