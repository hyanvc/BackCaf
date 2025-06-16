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

        // Lista todos os produtos persistidos
        public IEnumerable<ProdutoDTO> Listar()
        {
            return _dao.Listar().Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Descricao = p.Descricao,
                Preco = p.Preco
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
                Preco = p.Value.Item3
            };
        }

        public (int, string, decimal) Criar(string tipo, bool leiteDeAveia, bool canela, bool semAcucar)
        {
            Bebida bebida = BebidaFactory.Criar(tipo);
            if (leiteDeAveia) bebida = new LeiteDeAveia(bebida);
            if (canela) bebida = new Canela(bebida);
            if (semAcucar) bebida = new SemAcucar(bebida);
        
            return _dao.Adicionar(bebida);
        }

        public bool Atualizar(int id, string tipo, bool leiteDeAveia, bool canela, bool semAcucar)
        {
            Bebida bebida = BebidaFactory.Criar(tipo);
            if (leiteDeAveia) bebida = new LeiteDeAveia(bebida);
            if (canela) bebida = new Canela(bebida);
            if (semAcucar) bebida = new SemAcucar(bebida);
            return _dao.Atualizar(id, bebida);
        }

        public bool Remover(int id) => _dao.Remover(id);
    }
}