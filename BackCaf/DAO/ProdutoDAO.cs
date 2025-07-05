using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BackCaf.Models;

namespace BackCaf.DAO
{
    public class ProdutoDAO
    {
        private readonly string _caminhoArquivo;

        public ProdutoDAO()
        {
            var pastaDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            _caminhoArquivo = Path.Combine(pastaDownloads, "pedidos.txt");
            if (!File.Exists(_caminhoArquivo))
                File.Create(_caminhoArquivo).Close();
        }

        public int AdicionarPedido(string usuario, List<ProdutoPedidoDTO> produtos)
        {
            int novoId = 1;
            var pedidos = ListarPedidos().ToList();
            if (pedidos.Any())
                novoId = pedidos.Max(p => p.Id) + 1;

            var status = "Pendente";
            var produtosJson = JsonSerializer.Serialize(produtos);

            using (var writer = File.AppendText(_caminhoArquivo))
            {
                writer.WriteLine($"{novoId};{usuario};{status};{produtosJson}");
            }
            return novoId;
        }

        public PedidoDTO ObterPedido(int id)
        {
            return ListarPedidos().FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<PedidoDTO> ListarPedidos()
        {
            var linhas = File.ReadAllLines(_caminhoArquivo);
            foreach (var linha in linhas)
            {
                var partes = linha.Split(';');
                if (partes.Length == 4 &&
                    int.TryParse(partes[0], out int id))
                {
                    var produtos = JsonSerializer.Deserialize<List<ProdutoPedidoDTO>>(partes[3]);
                    yield return new PedidoDTO
                    {
                        Id = id,
                        Usuario = partes[1],
                        Status = partes[2],
                        Produtos = produtos
                    };
                }
            }
        }

        public bool AtualizarPedido(int pedidoId, List<ProdutoPedidoDTO> novosProdutos)
        {
            var pedidos = ListarPedidos().ToList();
            var index = pedidos.FindIndex(p => p.Id == pedidoId);
            if (index == -1) return false;

            var pedido = pedidos[index];
            pedidos[index] = new PedidoDTO
            {
                Id = pedido.Id,
                Usuario = pedido.Usuario,
                Status = pedido.Status,
                Produtos = novosProdutos
            };

            SalvarTodosPedidos(pedidos);
            return true;
        }

        public bool RemoverPedido(int pedidoId)
        {
            var pedidos = ListarPedidos().ToList();
            var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoId);
            if (pedido == null) return false;

            pedidos.Remove(pedido);
            SalvarTodosPedidos(pedidos);
            return true;
        }

        public bool AtualizarStatusPedido(int pedidoId, string novoStatus)
        {
            var pedidos = ListarPedidos().ToList();
            var index = pedidos.FindIndex(p => p.Id == pedidoId);
            if (index == -1) return false;

            var pedido = pedidos[index];
            pedidos[index] = new PedidoDTO
            {
                Id = pedido.Id,
                Usuario = pedido.Usuario,
                Status = novoStatus,
                Produtos = pedido.Produtos
            };

            SalvarTodosPedidos(pedidos);
            return true;
        }

        // Utilitário para sobrescrever o arquivo de pedidos
        private void SalvarTodosPedidos(List<PedidoDTO> pedidos)
        {
            var linhas = pedidos.Select(p =>
                $"{p.Id};{p.Usuario};{p.Status};{JsonSerializer.Serialize(p.Produtos)}"
            );
            File.WriteAllLines(_caminhoArquivo, linhas);
        }

        public IEnumerable<(int Id, string Descricao, decimal Preco, string Usuario, string Status)> Listar()
        {
            var linhas = File.ReadAllLines(_caminhoArquivo);
            foreach (var linha in linhas)
            {
                var partes = linha.Split(';');
                if (partes.Length == 5 &&
                    int.TryParse(partes[0], out int id) &&
                    decimal.TryParse(partes[2], out decimal preco))
                {
                    yield return (id, partes[1], preco, partes[3], partes[4]);
                }
            }
        }

        public (int, string, decimal, string, string)? Obter(int id)
        {
            return Listar().FirstOrDefault(p => p.Id == id);
        }

        public (int, string, decimal, string, string) Adicionar(Bebida bebida, string usuario)
        {
            int novoId = 1;
            var produtos = Listar().ToList();
            if (produtos.Any())
                novoId = produtos.Max(p => p.Id) + 1;

            string status = "Pendente";
            using (var writer = File.AppendText(_caminhoArquivo))
            {
                writer.WriteLine($"{novoId};{bebida.Descricao};{bebida.Preco};{usuario};{status}");
            }
            return (novoId, bebida.Descricao, bebida.Preco, usuario, status);
        }

        public bool Remover(int id)
        {
            var produtos = Listar().ToList();
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto == default) return false;

            produtos.Remove(produto);
            File.WriteAllLines(_caminhoArquivo, produtos.Select(p => $"{p.Id};{p.Descricao};{p.Preco};{p.Usuario};{p.Status}"));
            return true;
        }

        public bool Atualizar(int id, Bebida bebida)
        {
            var produtos = Listar().ToList();
            var index = produtos.FindIndex(p => p.Id == id);
            if (index == -1) return false;

            var usuario = produtos[index].Usuario;
            var status = produtos[index].Status;
            produtos[index] = (id, bebida.Descricao, bebida.Preco, usuario, status);
            File.WriteAllLines(_caminhoArquivo, produtos.Select(p => $"{p.Id};{p.Descricao};{p.Preco};{p.Usuario};{p.Status}"));
            return true;
        }

        public bool AtualizarStatus(int id, string novoStatus)
        {
            var produtos = Listar().ToList();
            var index = produtos.FindIndex(p => p.Id == id);
            if (index == -1) return false;

            var produto = produtos[index];
            produtos[index] = (produto.Id, produto.Descricao, produto.Preco, produto.Usuario, novoStatus);
            File.WriteAllLines(_caminhoArquivo, produtos.Select(p => $"{p.Id};{p.Descricao};{p.Preco};{p.Usuario};{p.Status}"));
            return true;
        }

        public void RemoverTodosDoUsuario(string usuario)
        {
            var produtos = Listar().ToList();
            produtos.RemoveAll(p => p.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase));
            File.WriteAllLines(_caminhoArquivo, produtos.Select(p => $"{p.Id};{p.Descricao};{p.Preco};{p.Usuario};{p.Status}"));
        }
    }
}