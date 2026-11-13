namespace PDV.App;

public class CadastroProduto
{
    private readonly List<Produto> _produtos = new();

    public IReadOnlyList<Produto> Listar()
    {
        return _produtos;
    }

    public bool Adicionar(Produto produto)
    {
        if (BuscarPorCodigo(produto.Codigo) != null)
            return false;

        _produtos.Add(produto);
        return true;
    }

    public bool Alterar(int codigo, string nome, decimal preco, int estoque)
    {
        Produto? produto = BuscarPorCodigo(codigo);
        if (produto == null)
            return false;

        produto.Nome = nome;
        produto.Preco = preco;
        produto.Estoque = estoque;
        return true;
    }

    public bool Remover(int codigo)
    {
        Produto? produto = BuscarPorCodigo(codigo);
        if (produto == null)
            return false;

        _produtos.Remove(produto);
        return true;
    }

    public Produto? BuscarPorCodigo(int codigo)
    {
        return _produtos.FirstOrDefault(p => p.Codigo == codigo);
    }

    public Produto? BuscarPorNome(string nome)
    {
        return _produtos.FirstOrDefault(p =>
            p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }
}