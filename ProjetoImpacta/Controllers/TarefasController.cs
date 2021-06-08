using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoImpacta.Data;
using ProjetoImpacta.Models;
using ProjetoImpacta.Reports;

namespace ProjetoImpacta.Controllers
{
    [Authorize]
    public class TarefasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarefasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tarefas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tarefas.ToListAsync());
        }

        // GET: Tarefas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(m => m.IdTarefa == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // GET: Tarefas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tarefas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTarefa,Descricao,Realizado,DataCadastro")] Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                tarefa.IdTarefa = Guid.NewGuid();
                tarefa.DataCadastro = DateTime.Now;
                _context.Add(tarefa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tarefa);
        }

        // GET: Tarefas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);
        }

        // POST: Tarefas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IdTarefa,Descricao,Realizado,DataCadastro")] Tarefa tarefa)
        {
            if (id != tarefa.IdTarefa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarefa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarefaExists(tarefa.IdTarefa))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarefa);
        }

        // GET: Tarefas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(m => m.IdTarefa == id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: Tarefas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarefaExists(Guid id)
        {
            return _context.Tarefas.Any(e => e.IdTarefa == id);
        }

        [AllowAnonymous]
        public JsonResult ObterDadosGrafico(Tarefa tarefa)
        {
            try
            {
                //retornar para o javascript, o conteudo 
                //da consulta feita no banco de dados..
                var lista = _context.Tarefas.ToList().Where(x => x.Realizado == true);
                var listaGrafico = new List<Grafico>();
                foreach (var item in lista)
                {
                    var grafico = new Grafico();
                    if (listaGrafico.Count > 0)
                    {
                        var verifica = listaGrafico.Where(x => x.DataRealizado == item.DataCadastro.ToString("dd/MM/yyyy"));
                        if (verifica.Count() > 0)
                        {
                            continue;
                        }
                    }

                    grafico.DataRealizado = item.DataCadastro.ToString("dd/MM/yyyy");
                    grafico.Total = lista.Count(x => x.DataCadastro.ToString("dd/MM/yyyy") == item.DataCadastro.ToString("dd/MM/yyyy"));

                    listaGrafico.Add(grafico);
                }


                return Json(listaGrafico);
            }

            catch (Exception e)
            {
                //retornando mensagem de erro..
                return Json(e.Message);
            }
        }

        public IActionResult Relatorio()
        {
            return View();
        }

        [HttpPost] //recebe os dados enviados pelo formulário
        public IActionResult Relatorio(Relatorio relatorio)
        {
            //verifica se todos os campos da model foram validados com sucesso!
            if (ModelState.IsValid)
            {
                try
                {
                    //capturando as datas informadas na página (formulario)
                    var filtroDataMin = Convert.ToDateTime(relatorio.DataMin);
                    var filtroDataMax = Convert.ToDateTime(relatorio.DataMax);

                    //executando a consulta de tarefas no banco de dados..
                    var tarefas = _context.Tarefas.ToList().Where(x => x.DataCadastro >= filtroDataMin && x.DataCadastro <= filtroDataMax).ToList();

                    //gerando o arquivo PDF..
                    var tarefaReport = new TarefaReport();
                    var pdf = tarefaReport.GerarPdf
                              (filtroDataMin, filtroDataMax, tarefas);

                    //fazer o download do arquivo..
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.Headers.Add("content-disposition",
                                         "attachment; filename=tarefas.pdf");
                    Response.Body.WriteAsync(pdf, 0, pdf.Length);
                    Response.Body.Flush();
                    Response.StatusCode = StatusCodes.Status200OK;
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = "Erro ao gerar relatório: "
                                           + e.Message;
                }

            }

            return View();
        }
    }
}
