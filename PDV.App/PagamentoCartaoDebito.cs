namespace PDV.App;

public class PagamentoCartaoDebito : IPagamento
{
    private readonly bool _aprovada;

    public PagamentoCartaoDebito(bool aprovada)
    {
        _aprovada = aprovada;
    }

    public bool RealizarPagamento(Venda venda)
    {
        if (!venda.PodeReceberPagamento || !_aprovada)
            return false;

        venda.Pagamento = new DadosPagamento
        {
            Forma = "Cartão de Débito"
        };
        return true;
    }
}