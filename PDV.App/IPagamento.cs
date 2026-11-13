namespace PDV.App;

public interface IPagamento
{
    bool RealizarPagamento(Venda venda);
}