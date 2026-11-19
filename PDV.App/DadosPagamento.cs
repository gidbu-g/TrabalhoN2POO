namespace PDV.App;

public class DadosPagamento
{
    public string Forma { get; set; } = string.Empty;
    public decimal ValorRecebido { get; set; }
    public decimal Troco { get; set; }
    public int QuantidadeParcelas { get; set; }
    public decimal ValorParcela { get; set; }

    public override string ToString()
    {
        return Forma switch
        {
            "Dinheiro" => $"Dinheiro | Recebido: {ValorRecebido:C} | Troco: {Troco:C}",
            "Cartão de Crédito" => $"Cartão de Crédito | {QuantidadeParcelas}x de {ValorParcela:C}",
            _ => Forma
        };
    }
}