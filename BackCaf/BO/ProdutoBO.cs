using System.Collections.Generic;
using System.Linq;
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
            return _dao.Listar().Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Descricao = p.Descricao,
                Preco = p.Preco,
                Usuario = p.Usuario,
                Status = p.Status
            });
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

        public (int, string, decimal, string, string) Criar(string tipo, bool leiteDeAveia, bool canela, bool semAcucar, string usuario)
        {
            // Remove todos os pedidos anteriores do usuário
            _dao.RemoverTodosDoUsuario(usuario);

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
    }
}