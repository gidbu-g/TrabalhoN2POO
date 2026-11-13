namespace PDV.App;

public class CadastroVenda
{
    private readonly CadastroProduto _cadastroProduto;
    private readonly List<Venda> _vendas = new();
    private int _proximoCodigo = 1;

    public CadastroVenda(CadastroProduto cadastroProduto)
    {
        _cadastroProduto = cadastroProduto;
    }

    public IReadOnlyList<Venda> Listar()
    {
        return _vendas;
    }

    public Venda? BuscarPorCodigo(int codigo)
    {
        return _vendas.FirstOrDefault(v => v.Codigo == codigo);
    }

    public Venda Criar()
    {
        var venda = new Venda(_proximoCodigo);
        _proximoCodigo++;
        _vendas.Add(venda);
        return venda;
    }

    public ResultadoItemVenda AdicionarItem(Venda venda, string codigoOuNome, int quantidade)
    {
        if (!venda.PodeSerAlterada)
            return ResultadoItemVenda.VendaNaoPodeSerAlterada;

        if (quantidade < 0)
            return ResultadoItemVenda.QuantidadeInvalida;

        Produto? produto = LocalizarProduto(codigoOuNome);
        if (produto == null)
            return ResultadoItemVenda.ProdutoNaoEncontrado;

        if (quantidade > produto.Estoque)
            return ResultadoItemVenda.EstoqueInsuficiente;

        venda.DefinirQuantidade(produto, quantidade);

        return quantidade == 0
            ? ResultadoItemVenda.ItemRemovido
            : ResultadoItemVenda.ItemDefinido;
    }

    public bool RemoverItem(Venda venda, int codigoProduto)
    {
        if (!venda.PodeSerAlterada)
            return false;

        return venda.RemoverItem(codigoProduto);
    }

    public bool PossuiEstoqueSuficiente(Venda venda)
    {
        foreach (ItemVenda item in venda.Itens)
        {
            Produto? produto = _cadastroProduto.BuscarPorCodigo(item.CodigoProduto);
            if (produto == null || produto.Estoque < item.Quantidade)
                return false;
        }
        return true;
    }

    public bool Pagar(Venda venda, IPagamento pagamento)
    {
        if (!venda.PodeReceberPagamento)
            return false;

        if (!PossuiEstoqueSuficiente(venda))
            return false;

        if (!pagamento.RealizarPagamento(venda))
            return false;

        foreach (ItemVenda item in venda.Itens)
        {
            Produto produto = _cadastroProduto.BuscarPorCodigo(item.CodigoProduto)!;
            produto.Estoque -= item.Quantidade;
        }
        return true;
    }

    public bool Cancelar(Venda venda)
    {
        if (venda.Cancelada)
            return false;

        if (venda.Paga)
            DevolverEstoque(venda);

        venda.Cancelar();
        return true;
    }

    private Produto? LocalizarProduto(string codigoOuNome)
    {
        Produto? produto = null;

        if (int.TryParse(codigoOuNome, out int codigo))
            produto = _cadastroProduto.BuscarPorCodigo(codigo);

        return produto ?? _cadastroProduto.BuscarPorNome(codigoOuNome);
    }

    private void DevolverEstoque(Venda venda)
    {
        foreach (ItemVenda item in venda.Itens)
        {
            Produto? produto = _cadastroProduto.BuscarPorCodigo(item.CodigoProduto);
            if (produto != null)
                produto.Estoque += item.Quantidade;
        }
    }
}