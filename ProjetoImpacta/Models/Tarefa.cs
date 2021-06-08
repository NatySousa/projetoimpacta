using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoImpacta.Models
{
    public class Tarefa
    {
        [Key]
        public Guid IdTarefa { get; set; }

        [Required(ErrorMessage = "É necessário descrever a tarefa")]
        [DisplayName("Descrição da Tarefa")]
        public string Descricao { get; set; }
        public bool Realizado { get; set; }
        public DateTime DataCadastro { get; set; }

    }
}
