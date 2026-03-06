namespace Domain.Enums
{
    /// <summary>
    /// Types de contrats
    /// </summary>
    public enum ContractType
    {
        /// <summary>
        /// Contrat cadre avec tarifs négociés
        /// </summary>
        ContratCadre = 1,

        /// <summary>
        /// Tarification spot ponctuelle
        /// </summary>
        Spot = 2,

        /// <summary>
        /// Accord spécifique client/partenaire
        /// </summary>
        AccordSpecifique = 3,


        /// <summary>
        /// Standard spécifique client/partenaire
        /// </summary>
        Standard = 4
    }
}
