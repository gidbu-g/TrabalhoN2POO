namespace PDV.App;

public class Armazenamento
{
    private readonly CadastroProduto _produtos;
    private readonly CadastroVenda _vendas;
    private readonly string _caminhoProdutos;
    private readonly string _caminhoVendas;

    public Armazenamento(CadastroProduto produtos, CadastroVenda vendas, string pasta)
    {
        _produtos = produtos;
        _vendas = vendas;
        _caminhoProdutos = Path.Combine(pasta, "produtos.txt");
        _caminhoVendas = Path.Combine(pasta, "vendas.txt");
    }

    public void CarregarTudo()
    {
        _produtos.CarregarProdutos(_caminhoProdutos);
        _vendas.CarregarVendas(_caminhoVendas);
    }

    public void SalvarProdutos()
    {
        Salvar(() => _produtos.SalvarProdutos(_caminhoProdutos));
    }

    public void SalvarVendas()
    {
        Salvar(() => _vendas.SalvarVendas(_caminhoVendas));
    }

    public void SalvarTudo()
    {
        SalvarProdutos();
        SalvarVendas();
    }

    private static void Salvar(Action salvamento)
    {
        try
        {
            salvamento();
        }
        catch (Exception erro) when (erro is IOException or UnauthorizedAccessException)
        {
            Console.WriteLine($"Aviso: não foi possível salvar os dados ({erro.Message}).");
        }
    }
}