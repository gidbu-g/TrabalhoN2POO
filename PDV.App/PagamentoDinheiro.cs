namespace PDV.App;

public class PagamentoDinheiro : IPagamento
{
    private readonly decimal _valorRecebido;

    public PagamentoDinheiro(decimal valorRecebido)
    {
        _valorRecebido = valorRecebido;
    }

    public bool RealizarPagamento(Venda venda)
    {
        if (!venda.PodeReceberPagamento)
            return false;

        if (_valorRecebido < venda.Total)
            return false;

        venda.Pagamento = new DadosPagamento
        {
            Forma = "Dinheiro",
            ValorRecebido = _valorRecebido,
            Troco = _valorRecebido - venda.Total
        };
        return true;
    }
}