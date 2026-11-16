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

    public void SalvarProdutos(string caminho)
    {
        IEnumerable<string> linhas = _produtos.Select(FormatarLinha);
        File.WriteAllLines(caminho, linhas);
    }

    public void CarregarProdutos(string caminho)
    {
        if (!File.Exists(caminho))
            return;

        var carregados = new List<Produto>();

        foreach (string linha in File.ReadLines(caminho))
        {
            if (string.IsNullOrWhiteSpace(linha))
                continue;

            Produto produto = LerLinha(linha);

            if (carregados.Any(p => p.Codigo == produto.Codigo))
                throw new InvalidDataException($"Código de produto repetido no arquivo: {produto.Codigo}.");

            carregados.Add(produto);
        }

        _produtos.Clear();
        _produtos.AddRange(carregados);
    }

    private static string FormatarLinha(Produto produto)
    {
        string preco = FormatoArquivo.Formatar(produto.Preco);
        return $"{produto.Codigo};{produto.Nome};{preco};{produto.Estoque}";
    }

    private static Produto LerLinha(string linha)
    {
        string[] campos = linha.Split(';');

        if (campos.Length != 4)
            throw new InvalidDataException($"Linha de produto inválida: {linha}");

        try
        {
            return new Produto(
                FormatoArquivo.LerInteiro(campos[0]),
                campos[1],
                FormatoArquivo.LerDecimal(campos[2]),
                FormatoArquivo.LerInteiro(campos[3]));
        }
        catch (Exception erro) when (erro is FormatException or OverflowException or ArgumentException)
        {
            throw new InvalidDataException($"Linha de produto inválida: {linha}", erro);
        }
    }
}