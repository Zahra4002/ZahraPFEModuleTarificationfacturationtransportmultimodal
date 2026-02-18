namespace Domain.Enums
{
    /// <summary>
    /// Méthodes de paiement
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Virement bancaire
        /// </summary>
        Virement = 1,

        /// <summary>
        /// Chèque
        /// </summary>
        Cheque = 2,

        /// <summary>
        /// Carte bancaire
        /// </summary>
        CarteBancaire = 3,

        /// <summary>
        /// Espèces
        /// </summary>
        Especes = 4,

        /// <summary>
        /// Lettre de crédit
        /// </summary>
        LettreCredit = 5
    }
}
