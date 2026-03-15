using Domain.Enums;

public class ContractSummaryDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ContractType Type { get; set; }
}