using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BackCaf.BO;
using BackCaf.Models;

namespace BackCaf.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private ProdutoBO _bo = new();

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

        // Novo endpoint para criar v�rios produtos em um pedido
        //USA ESSE AGORA COMO PADRAO DE CRIA��O DE PRODUTOS : 
        [HttpPost("varios")]
        public IActionResult PostVarios([FromBody] PedidoRequest req)
        {
            var ids = _bo.CriarPedido(req.Produtos, req.Usuario);
            return Ok(ids);
        }

        [HttpPost("criarvarios")]
        public IActionResult CriarVarios([FromBody] PedidoRequest req)
        {
            var pedidoId = _bo.CriarPedidoComProdutos(req.Usuario, req.Produtos);
            return Ok(new { PedidoId = pedidoId });
        }

        [HttpGet("getvarios/{id}")]
        public IActionResult GetVarios(int id)
        {
            var pedido = _bo.ObterPedido(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        [HttpGet("getvarios/usuario/{usuario}")]
        public IActionResult GetVariosPorUsuario(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return BadRequest("Usu�rio � obrigat�rio.");

            var pedidos = _bo.ListarPedidos()
                .Where(p => p.Usuario != null && p.Usuario.Equals(usuario, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!pedidos.Any())
                return NotFound("Nenhum pedido encontrado para este usu�rio.");

            return Ok(pedidos);
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