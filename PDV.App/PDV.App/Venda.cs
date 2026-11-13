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

        ItemVenda? item = BuscarItem(produto.Codigo);

        if (quantidade == 0)
        {
            if (item != null)
                _itens.Remove(item);
            return;
        }

        if (item == null)
            _itens.Add(new ItemVenda(produto.Codigo, produto.Nome, produto.Preco, quantidade));
        else
            item.Quantidade = quantidade;
    }

    public void Cancelar()
    {
        Cancelada = true;
    }
}