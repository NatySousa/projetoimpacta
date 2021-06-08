using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProjetoImpacta.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoImpacta.Reports
{
    public class TarefaReport
    {

        //método para gerar um relatorio em formato PDF
        //byte[] -> indica que o método retorna um arquivo..
        public byte[] GerarPdf(DateTime dataMin, DateTime dataMax,
    List<Tarefa> tarefas)
        {
            //abrindo o documento PDF..
            var memoryStream = new MemoryStream();
            var pdf = new PdfDocument(new PdfWriter(memoryStream));

            //escrevendo o conteudo do PDF..
            using (var document = new Document(pdf))
            {
                document.Add(ObterLogotipo);

                document.Add(new Paragraph("Relatório de Tarefa")
                    .AddStyle(FormatacaoTitulo)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph
                    ($"Tarefas cadastradas entre: { dataMin.ToString("dd/MM/yyyy") } e { dataMax.ToString("dd/MM/yyyy")} ")
                      .AddStyle(FormatacaoSubTitulo)
                      .SetTextAlignment(TextAlignment.CENTER));


                //escrever uma tabela contendo os jogos..
                var table = new Table(3); //3 -> numero de colunas da tabela
                table.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                table.AddHeaderCell("Descrição da Tarefa");
                table.AddHeaderCell("Realizado");
                table.AddHeaderCell("Data de Cadastro");

                //exibir os jogos dentro da tabela..
                foreach (var item in tarefas)
                {
                    table.AddCell(item.Descricao);
                    if (item.Realizado)
                    {
                        table.AddCell("Sim");
                    }
                    else
                    {
                        table.AddCell("Não");
                    }
                    table.AddCell(item.DataCadastro.ToString("dd/MM/yyyy"));
                }

                document.Add(table); //adicionando a tabela no documento PDF
            }

            //retornar o arquivo PDF:
            return memoryStream.ToArray();
        }
        //método para gerar o estilo do titulo do relatorio..
        private Style FormatacaoTitulo
        {
            get
            {

                var style = new Style();
                style.SetFont(PdfFontFactory.CreateFont
(StandardFonts.HELVETICA_BOLD));
                style.SetFontSize(26);
                style.SetFontColor(Color.ConvertRgbToCmyk
(new DeviceRgb(0, 102, 284)));

                return style;
            }
        }

        //método para gerar o estilo do subtitulo do relatorio..
        private Style FormatacaoSubTitulo
        {
            get
            {
                var style = new Style();
                style.SetFont(PdfFontFactory.CreateFont
(StandardFonts.HELVETICA));
                style.SetFontSize(13);
                style.SetFontColor(Color.ConvertRgbToCmyk
(new DeviceRgb(0, 0, 0)));

                return style;
            }
        }

        private Image ObterLogotipo
        {
            get
            {
                ImageData imageData = ImageDataFactory.Create(new Uri("https://www.hopencontabilidade.com.br/wp-content/uploads/2016/09/gest%C3%A3o-de-tarefas.jpg"));
                var logotipo = new Image(imageData);
                logotipo.SetWidth(200);
                logotipo.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                return logotipo;
            }
        }
    }
}
