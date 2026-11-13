namespace PDV.App;

public class PagamentoCartaoCredito : IPagamento
{
    private readonly int _quantidadeParcelas;

    public PagamentoCartaoCredito(int quantidadeParcelas)
    {
        if (quantidadeParcelas < 1)
            throw new ArgumentException("A quantidade de parcelas deve ser no mínimo 1.");

        _quantidadeParcelas = quantidadeParcelas;
    }

    public decimal CalcularValorParcela(Venda venda)
    {
        return Math.Round(venda.Total / _quantidadeParcelas, 2);
    }

    public bool RealizarPagamento(Venda venda)
    {
        if (!venda.PodeReceberPagamento)
            return false;

        venda.Pagamento = new DadosPagamento
        {
            Forma = "Cartão de Crédito",
            QuantidadeParcelas = _quantidadeParcelas,
            ValorParcela = CalcularValorParcela(venda)
        };
        return true;
    }
}