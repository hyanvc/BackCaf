namespace BackCaf.Strategies
{
    public class DescontoPix : IDescontoStrategy
    {
        public decimal Calcular(decimal valor)
        {
            // 5% de desconto
            return valor * 0.95m;
        }
    }
}