namespace Domain.Enums
{
    public enum ShipmentStatus
    {
        Draft,
        Confirmed,
        InTransit,
        Delivered,
        Cancelled
    }
    public enum ContainerType
    {
        /// <summary>
        /// Conteneur 20 pieds standard
        /// </summary>
        Container20ft = 1,

        /// <summary>
        /// Conteneur 40 pieds standard
        /// </summary>
        Container40ft = 2,

        /// <summary>
        /// Conteneur 40 pieds High Cube
        /// </summary>
        Container40ftHC = 3,

        /// <summary>
        /// Conteneur réfrigéré 20 pieds
        /// </summary>
        Reefer20ft = 4,

        /// <summary>
        /// Conteneur réfrigéré 40 pieds
        /// </summary>
        Reefer40ft = 5
    }

}
