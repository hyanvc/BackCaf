using Microsoft.AspNetCore.Mvc;
using BackCaf.BO;
using BackCaf.Strategies;
using System.Linq;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly ProdutoBO _produtoBO = new();
        private readonly FidelidadeController _fidelidadeController = new();

        [HttpPost]
        public IActionResult Post([FromBody] PagamentoRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Usuario))
                return BadRequest("Usuário é obrigatório.");

            if (req.Valor <= 0)
                return BadRequest("Valor inválido.");

            // Verifica se o usuário tem fidelidade disponível
            int recompensasDisponiveis = 0;
            var fidelidadeResult = _fidelidadeController.GetFidelidade(req.Usuario) as OkObjectResult;
            if (fidelidadeResult != null)
            {
                var fidelidadeData = fidelidadeResult.Value?.GetType().GetProperty("RecompensasDisponiveis")?.GetValue(fidelidadeResult.Value, null);
                if (fidelidadeData != null && int.TryParse(fidelidadeData.ToString(), out int recompensas))
                    recompensasDisponiveis = recompensas;
            }

            // Seleciona a estratégia de desconto
            IDescontoStrategy descontoStrategy = req.TipoPagamento?.ToLower() switch
            {
                "fidelidade" when recompensasDisponiveis > 0 => new DescontoFidelidade(),
                "pix" => new DescontoPix(),
                _ => new SemDesconto()
            };

            decimal valorFinal = descontoStrategy.Calcular(req.Valor);

            // Se for fidelidade e houver recompensa, consome uma
            if (req.TipoPagamento?.ToLower() == "fidelidade" && recompensasDisponiveis > 0)
            {
                _fidelidadeController.ConsumirFidelidade(req.Usuario);
            }

            return Ok(new
            {
                Usuario = req.Usuario,
                ValorOriginal = req.Valor,
                TipoPagamento = req.TipoPagamento,
                DescontoAplicado = req.Valor - valorFinal,
                ValorFinal = valorFinal,
                FidelidadeUtilizada = req.TipoPagamento?.ToLower() == "fidelidade" && recompensasDisponiveis > 0
            });
        }
    }

    public class PagamentoRequest
    {
        public string Usuario { get; set; }
        public decimal Valor { get; set; }
        public string TipoPagamento { get; set; } // "fidelidade", "pix", "cartao", etc.
    }
}