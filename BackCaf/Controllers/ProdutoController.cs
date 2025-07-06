using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BackCaf.BO;
using BackCaf.Models;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private ProdutoBO _bo = new();
        private readonly INotificationObserver _notificacaoArquivoObserver = new NotificacaoArquivoObserver();

        // --- MÉTODOS ANTIGOS (produtos individuais) ---
        /*
        [HttpGet]
        public IActionResult Get()
        {
            var produtos = _bo.Listar().ToList();
            if (!produtos.Any()) return NotFound();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var produto = _bo.Obter(id);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoRequest req)
        {
            var produto = _bo.Criar(req.Tipo, req.LeiteDeAveia, req.Canela, req.SemAcucar, req.Usuario);
            var idproduto = produto.Item1;
            return Ok(idproduto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProdutoRequest req)
        {
            if (!_bo.Atualizar(id, req.Tipo, req.LeiteDeAveia, req.Canela, req.SemAcucar))
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public IActionResult AtualizarStatus(int id, [FromBody] AtualizarStatusRequest req)
        {
            if (!_bo.AtualizarStatus(id, req.Status))
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_bo.Remover(id)) return NotFound();
            return NoContent();
        }
        */
        // --- FIM DOS MÉTODOS ANTIGOS ---

        // --- NOVA ARQUITETURA: PEDIDOS ---

        // Cria vários produtos em um pedido (padrão)
        //[HttpPost("Post")]
        //public IActionResult Post([FromBody] PedidoRequest req)
        //{
        //    var ids = _bo.CriarPedido(req.Produtos, req.Usuario);
        //    return Ok(ids);
        //}

        // Cria um pedido com vários produtos ou 1  (estrutura de pedido)
        [HttpPost("Post")]
        public IActionResult Post([FromBody] PedidoRequest req)
        {
            var pedidoId = _bo.CriarPedidoComProdutos(req.Usuario, req.Produtos);
            return Ok(new { PedidoId = pedidoId });
        }

        // GET: /api/produto/pedidos
        [HttpGet("pedidos")]
        public IActionResult GetPedidos()
        {
            var pedidos = _bo.ListarPedidos()
                .Select(p => new
                {
                    p.Id,
                    p.Usuario,
                    p.Status,
                    Produtos = p.Produtos,
                    Preco = p.Produtos?.Sum(prod => prod.Preco * prod.Quantidade) ?? 0m
                })
                .ToList();

            if (!pedidos.Any())
                return NotFound("Nenhum pedido encontrado.");

            return Ok(pedidos);
        }

        // Consulta um pedido por ID
        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            var pedido = _bo.ObterPedido(id);
            if (pedido == null) return NotFound();

            return Ok(new
            {
                pedido.Id,
                pedido.Usuario,
                pedido.Status,
                Produtos = pedido.Produtos,
                Preco = pedido.Produtos?.Sum(prod => prod.Preco * prod.Quantidade) ?? 0m
            });
        }

        // Consulta todos os pedidos de um usuário
        [HttpGet("Get/usuario/{usuario}")]
        public IActionResult GetPorUsuario(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return BadRequest("Usuário é obrigatório.");

            var pedidos = _bo.ListarPedidos()
                .Where(p => p.Usuario != null && p.Usuario.Equals(usuario, System.StringComparison.OrdinalIgnoreCase))
                .Select(p => new
                {
                    p.Id,
                    p.Usuario,
                    p.Status,
                    Produtos = p.Produtos,
                    Preco = p.Produtos?.Sum(prod => prod.Preco * prod.Quantidade) ?? 0m
                })
                .ToList();

            if (!pedidos.Any())
                return NotFound("Nenhum pedido encontrado para este usuário.");

            return Ok(pedidos);
        }

        // Atualiza todos os produtos de um pedido
        [HttpPut("pedido/{id}")]
        public IActionResult AtualizarPedido(int id, [FromBody] PedidoRequest req)
        {
            var pedido = _bo.ObterPedido(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            bool atualizado = _bo.AtualizarPedido(id, req.Produtos);
            if (!atualizado)
                return BadRequest("Não foi possível atualizar o pedido.");

            return NoContent();
        }

        // Remove um pedido inteiro (com todos os produtos)
        [HttpDelete("pedido/{id}")]
        public IActionResult DeletePedido(int id)
        {
            var pedido = _bo.ObterPedido(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            bool removido = _bo.RemoverPedido(id);
            if (!removido)
                return BadRequest("Não foi possível remover o pedido.");

            return NoContent();
        }

        // Atualiza o status do pedido
        [HttpPut("pedido/{id}/status")]
        public IActionResult AtualizarStatusPedido(int id, [FromBody] AtualizarStatusRequest req)
        {
            var pedido = _bo.ObterPedido(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            bool atualizado = _bo.AtualizarStatusPedido(id, req.Status);
            if (!atualizado)
                return BadRequest("Não foi possível atualizar o status do pedido.");

            return NoContent();
        }
    }

    public class ProdutoRequest
    {
        public string Tipo { get; set; }
        public bool LeiteDeAveia { get; set; }
        public bool Canela { get; set; }
        public bool SemAcucar { get; set; }
        public string Usuario { get; set; }
    }

    public class AtualizarStatusRequest
    {
        public string Status { get; set; }
    }

    public class PedidoRequest
    {
        public string Usuario { get; set; }
        public List<ProdutoItemRequest> Produtos { get; set; }
    }
}