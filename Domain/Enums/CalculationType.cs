namespace Domain.Enums
{
    public enum CalculationType
    {
        PerLine,    // Calculée par ligne
        PerInvoice, // Calculée sur la facture entière
        Custom ,     // Calcul personnalisé
        Percentage // Calculée en pourcentage du montant total
    }
}