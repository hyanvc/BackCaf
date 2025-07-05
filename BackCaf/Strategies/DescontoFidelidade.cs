namespace BackCaf.Strategies
{
    public class DescontoFidelidade : IDescontoStrategy
    {
        public decimal Calcular(decimal valor)
        {
            // 10% de desconto
            return valor * 0.90m;
        }
    }
}