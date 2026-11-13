namespace PDV.App;

public class Produto
{
    private string _nome = string.Empty;
    private decimal _preco;
    private int _estoque;

    public int Codigo { get; }

    public string Nome
    {
        get => _nome;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O nome do produto é obrigatório.");
            _nome = value;
        }
    }

    public decimal Preco
    {
        get => _preco;
        set
        {
            if (value < 0)
                throw new ArgumentException("O preço não pode ser negativo.");
            _preco = value;
        }
    }

    public int Estoque
    {
        get => _estoque;
        set
        {
            if (value < 0)
                throw new ArgumentException("O estoque não pode ser negativo.");
            _estoque = value;
        }
    }

    public Produto(int codigo, string nome, decimal preco, int estoque)
    {
        Codigo = codigo;
        Nome = nome;
        Preco = preco;
        Estoque = estoque;
    }

    public override string ToString()
    {
        return $"{Codigo} - {Nome} | Preço: {Preco:C} | Estoque: {Estoque}";
    }
}