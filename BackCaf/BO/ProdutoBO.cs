using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BackCaf.DAO;
using BackCaf.Models;
using BackCaf.Factories;

namespace BackCaf.BO
{
    public class ProdutoBO
    {
        private ProdutoDAO _dao = new();
        private readonly INotificationObserver _notificacaoArquivoObserver = new NotificacaoArquivoObserver();

        // Lista todos os produtos persistidos
        public IEnumerable<ProdutoDTO> Listar()
        {
            var lista = _dao.Listar().Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Descricao = p.Descricao,
                Preco = p.Preco,
                Usuario = p.Usuario,
                Status = p.Status
            });

            return lista;
        }

        // Obtém um produto pelo ID
        public ProdutoDTO Obter(int id)
        {
            var p = _dao.Obter(id);
            if (p == null) return null;
            return new ProdutoDTO
            {
                Id = p.Value.Item1,
                Descricao = p.Value.Item2,
                Preco = p.Value.Item3,
                Usuario = p.Value.Item4,
                Status = p.Value.Item5
            };
        }

        // Novo método: criar vários produtos em um pedido
        public List<int> CriarPedido(List<ProdutoItemRequest> produtos, string usuario)
        {
            var ids = new List<int>();
            foreach (var item in produtos)
            {
                for (int i = 0; i < item.Quantidade; i++)
                {
                    Bebida bebida = BebidaFactory.Criar(item.Tipo);
                    if (item.LeiteDeAveia) bebida = new LeiteDeAveia(bebida);
                    if (item.Canela) bebida = new Canela(bebida);
                    if (item.SemAcucar) bebida = new SemAcucar(bebida);
                    var result = _dao.Adicionar(bebida, usuario);
                    ids.Add(result.Item1);
                }
            }
            return ids;
        }

        // Mantido para compatibilidade, mas não remove mais pedidos antigos
        public (int, string, decimal, string, string) Criar(string tipo, bool leiteDeAveia, bool canela, bool semAcucar, string usuario)
        {
            Bebida bebida = BebidaFactory.Criar(tipo);
            if (leiteDeAveia) bebida = new LeiteDeAveia(bebida);
            if (canela) bebida = new Canela(bebida);
            if (semAcucar) bebida = new SemAcucar(bebida);
            return _dao.Adicionar(bebida, usuario);
        }

        public bool Atualizar(int id, string tipo, bool leiteDeAveia, bool canela, bool semAcucar)
        {
            Bebida bebida = BebidaFactory.Criar(tipo);
            if (leiteDeAveia) bebida = new LeiteDeAveia(bebida);
            if (canela) bebida = new Canela(bebida);
            if (semAcucar) bebida = new SemAcucar(bebida);
            return _dao.Atualizar(id, bebida);
        }

        public bool AtualizarStatus(int id, string novoStatus)
        {
            var produto = _dao.Obter(id);
            if (produto == null) return false;

            var atualizado = _dao.AtualizarStatus(id, novoStatus);
            if (atualizado)
            {
                // Notifica o usuário via Observer (arquivo)
                _notificacaoArquivoObserver.Notificar(
                    $"Status do pedido #{id} alterado para '{novoStatus}'", produto.Value.Item4
                );
            }
            return atualizado;
        }

        public bool Remover(int id) => _dao.Remover(id);

        public int CriarPedidoComProdutos(string usuario, List<ProdutoItemRequest> produtos)
        {
            // Monta lista de ProdutoPedidoDTO
            var produtosPedido = new List<ProdutoPedidoDTO>();
            foreach (var item in produtos)
            {
                Bebida bebida = BebidaFactory.Criar(item.Tipo);
                if (item.LeiteDeAveia) bebida = new LeiteDeAveia(bebida);
                if (item.Canela) bebida = new Canela(bebida);
                if (item.SemAcucar) bebida = new SemAcucar(bebida);

                produtosPedido.Add(new ProdutoPedidoDTO
                {
                    Tipo = item.Tipo,
                    Descricao = bebida.Descricao,
                    Preco = bebida.Preco,
                    Quantidade = item.Quantidade,
                    LeiteDeAveia = item.LeiteDeAveia,
                    Canela = item.Canela,
                    SemAcucar = item.SemAcucar
                });
            }
            return _dao.AdicionarPedido(usuario, produtosPedido);
        }

        public PedidoDTO ObterPedido(int id)
        {
            return _dao.ObterPedido(id);
        }

        public IEnumerable<PedidoDTO> ListarPedidos()
        {
            return _dao.ListarPedidos();
        }
    }
}