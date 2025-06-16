namespace BackCaf.Models
{
    public abstract class IngredienteDecorator : Bebida
    {
        protected Bebida _bebida;
        public IngredienteDecorator(Bebida bebida) => _bebida = bebida;
    }

    public class LeiteDeAveia : IngredienteDecorator
    {
        public LeiteDeAveia(Bebida bebida) : base(bebida) { }
        public override string Descricao => _bebida.Descricao + ", Leite de Aveia";
        public override decimal Preco => _bebida.Preco + 2.00m;
    }

    public class Canela : IngredienteDecorator
    {
        public Canela(Bebida bebida) : base(bebida) { }
        public override string Descricao => _bebida.Descricao + ", Canela";
        public override decimal Preco => _bebida.Preco + 0.50m;
    }

    public class SemAcucar : IngredienteDecorator
    {
        public SemAcucar(Bebida bebida) : base(bebida) { }
        public override string Descricao => _bebida.Descricao + ", Sem Açúcar";
        public override decimal Preco => _bebida.Preco;
    }
}