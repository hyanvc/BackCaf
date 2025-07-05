namespace BackCaf.Strategies
{
    public class SemDesconto : IDescontoStrategy
    {
        public decimal Calcular(decimal valor)
        {
            // Sem desconto
            return valor;
        }
    }
}