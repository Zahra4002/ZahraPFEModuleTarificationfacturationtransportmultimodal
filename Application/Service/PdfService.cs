using Application.Interfaces;
using Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Initialiser QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // En-tête
                        page.Header().Element(ComposeHeader);

                        // Contenu
                        page.Content().Element(container => ComposeContent(container, invoice));

                        // Pied de page
                        page.Footer().Element(ComposeFooter);
                    });
                });

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                return stream.ToArray();
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("FACTURE")
                        .FontSize(20).Bold().FontColor(Colors.Blue.Medium);

                    column.Item().Text($"Date d'émission: {DateTime.Now:dd/MM/yyyy}")
                        .FontSize(10);
                });

                //row.ConstantItem(150).Image(QuestPDF.Infrastructure.Image.FromFile("logo.png"));
            });
        }

        private void ComposeContent(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                // Informations client/fournisseur
                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeClientInfo(c, invoice));
                    row.ConstantItem(20);
                    row.RelativeItem().Element(c => ComposeInvoiceInfo(c, invoice));
                });

                column.Item().PaddingVertical(20).Element(c => ComposeInvoiceTable(c, invoice));

                // Totaux
                column.Item().AlignRight().Element(c => ComposeTotals(c, invoice));

                // Notes
                if (!string.IsNullOrEmpty(invoice.Notes))
                {
                    column.Item().PaddingVertical(10).Element(c => ComposeNotes(c, invoice));
                }
            });
        }

        private void ComposeClientInfo(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("CLIENT").Bold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                if (invoice.Client != null)
                {
                    column.Item().Text(invoice.Client.Name);
                    column.Item().Text(invoice.Client.Email ?? "");
                    column.Item().Text(invoice.Client.PhoneNumber ?? "");
                }
                else
                {
                    column.Item().Text("Client non spécifié");
                }
            });
        }

        private void ComposeInvoiceInfo(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("FACTURE").Bold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Text($"N°: {invoice.InvoiceNumber}");
                column.Item().Text($"Date: {invoice.InvoiceDate:dd/MM/yyyy}");
                column.Item().Text($"Échéance: {invoice.DueDate:dd/MM/yyyy}");
                column.Item().Text($"Statut: {invoice.Status}");
            });
        }

        private void ComposeInvoiceTable(IContainer container, Invoice invoice)
        {
            container.Table(table =>
            {
                // Définition des colonnes
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Description
                    columns.ConstantColumn(50); // Quantité
                    columns.ConstantColumn(80); // Prix unitaire
                    columns.ConstantColumn(50); // TVA
                    columns.ConstantColumn(80); // Total
                });

                // En-tête du tableau
                table.Header(header =>
                {
                    header.Cell().Text("Description").Bold();
                    header.Cell().Text("Qté").Bold();
                    header.Cell().Text("Prix unitaire").Bold();
                    header.Cell().Text("TVA%").Bold();
                    header.Cell().Text("Total HT").Bold();

                    header.Cell().ColumnSpan(5).PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                // Lignes du tableau
                if (invoice.Lines != null && invoice.Lines.Any())
                {
                    foreach (var line in invoice.Lines)
                    {
                        table.Cell().Text(line.Description ?? "-");
                        table.Cell().Text(line.Quantity.ToString());
                        table.Cell().Text($"{line.UnitPriceHT:C}");
                        table.Cell().Text($"{line.VATRate}%");
                        table.Cell().Text($"{(line.Quantity * line.UnitPriceHT):C}");
                    }
                }
                else
                {
                    table.Cell().ColumnSpan(5).Text("Aucune ligne").AlignCenter();
                }
            });
        }

        private void ComposeTotals(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(150).Text("Total HT:").Bold();
                    row.ConstantItem(100).Text($"{invoice.TotalHT:C}").AlignRight();
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(150).Text("TVA:").Bold();
                    row.ConstantItem(100).Text($"{invoice.TotalVAT:C}").AlignRight();
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(150).Text("Total TTC:").Bold();
                    row.ConstantItem(100).Text($"{invoice.TotalTTC:C}").AlignRight();
                });

                if (invoice.AmountPaid > 0)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem();
                        row.ConstantItem(150).Text("Déjà payé:").Bold();
                        row.ConstantItem(100).Text($"{invoice.AmountPaid:C}").AlignRight();
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem();
                        row.ConstantItem(150).Text("Solde:").Bold();
                        row.ConstantItem(100).Text($"{invoice.Balance:C}").AlignRight();
                    });
                }
            });
        }

        private void ComposeNotes(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("NOTES:").Bold();
                column.Item().Text(invoice.Notes);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(20).LineHorizontal(1);
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text("Merci de votre confiance").FontSize(8);
                    row.RelativeItem().Text($"Page {1}").FontSize(8).AlignRight();
                });
            });
        }
    }
}