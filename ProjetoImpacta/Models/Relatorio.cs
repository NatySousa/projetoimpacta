using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoImpacta.Models
{
    public class Relatorio
    {

        [Required(ErrorMessage = "Por favor, informe a data de início.")]
        public DateTime? DataMax { get; set; }

        [Required(ErrorMessage = "Por favor, informe a data de início.")]
        public DateTime? DataMin { get; set; }
    }
}
