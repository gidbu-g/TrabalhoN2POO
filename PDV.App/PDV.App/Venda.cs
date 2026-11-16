namespace PDV.App;

public class Venda
{
    private readonly List<ItemVenda> _itens = new();

    public int Codigo { get; }
    public bool Cancelada { get; private set; }
    public DadosPagamento? Pagamento { get; set; }

    public IReadOnlyList<ItemVenda> Itens => _itens;
    public decimal Total => _itens.Sum(item => item.TotalItem);
    public bool Paga => Pagamento != null;
    public bool PodeSerAlterada => !Cancelada && !Paga;
    public bool PodeReceberPagamento => PodeSerAlterada && _itens.Count > 0;

    public Venda(int codigo)
    {
        Codigo = codigo;
    }

    public ItemVenda? BuscarItem(int codigoProduto)
    {
        return _itens.FirstOrDefault(i => i.CodigoProduto == codigoProduto);
    }

    public void DefinirQuantidade(Produto produto, int quantidade)
    {
        if (quantidade < 0)
            throw new ArgumentException("A quantidade não pode ser negativa.");

        if (quantidade == 0)
        {
            RemoverItem(produto.Codigo);
            return;
        }

        ItemVenda? item = BuscarItem(produto.Codigo);

        if (item == null)
            _itens.Add(new ItemVenda(produto.Codigo, produto.Nome, produto.Preco, quantidade));
        else
            item.Quantidade = quantidade;
    }

    public bool RemoverItem(int codigoProduto)
    {
        ItemVenda? item = BuscarItem(codigoProduto);
        if (item == null)
            return false;

        _itens.Remove(item);
        return true;
    }
    public void CarregarItem(ItemVenda item)
    {
        _itens.Add(item);
    }
    public void Cancelar()
    {
        Cancelada = true;
    }
}