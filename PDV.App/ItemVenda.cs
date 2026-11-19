namespace PDV.App;

public class ItemVenda
{
    public int CodigoProduto { get; }
    public string NomeProduto { get; }
    public decimal PrecoUnitario { get; }
    public int Quantidade { get; set; }

    public decimal TotalItem => PrecoUnitario * Quantidade;

    public ItemVenda(int codigoProduto, string nomeProduto, decimal precoUnitario, int quantidade)
    {
        CodigoProduto = codigoProduto;
        NomeProduto = nomeProduto;
        PrecoUnitario = precoUnitario;
        Quantidade = quantidade;
    }

    public override string ToString()
    {
        return $"{CodigoProduto} - {NomeProduto} | {Quantidade} x {PrecoUnitario:C} = {TotalItem:C}";
    }
}