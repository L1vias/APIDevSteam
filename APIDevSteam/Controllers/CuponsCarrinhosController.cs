using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIDevSteam.Models;
using APIDevSteamJau.Data;

namespace APIDevSteam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponsCarrinhosController : ControllerBase
    {
        private readonly APIContext _context;

        public CuponsCarrinhosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/CuponsCarrinhos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CupomCarrinho>>> GetCuponsCarrinhos()
        {
            return await _context.CuponsCarrinhos.ToListAsync();
        }

        // GET: api/CuponsCarrinhos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CupomCarrinho>> GetCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CuponsCarrinhos.FindAsync(id);

            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            return cupomCarrinho;
        }

        // PUT: api/CuponsCarrinhos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupomCarrinho(Guid id, CupomCarrinho cupomCarrinho)
        {
            if (id != cupomCarrinho.CupomCarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(cupomCarrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CupomCarrinhoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CuponsCarrinhos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CupomCarrinho>> PostCupomCarrinho(CupomCarrinho cupomCarrinho)
        {
            _context.CuponsCarrinhos.Add(cupomCarrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCupomCarrinho", new { id = cupomCarrinho.CupomCarrinhoId }, cupomCarrinho);
        }

        // DELETE: api/CuponsCarrinhos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CuponsCarrinhos.FindAsync(id);
            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            _context.CuponsCarrinhos.Remove(cupomCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CupomCarrinhoExists(Guid id)
        {
            return _context.CuponsCarrinhos.Any(e => e.CupomCarrinhoId == id);
        }

        [HttpPost("AplicarCupom")]
        public async Task<IActionResult> AplicarCupom(Guid itemCarrinhoId, Guid cupomId)
        {
            // Verifica se o item do carrinho existe
            var itemCarrinho = await _context.ItensCarrinhos
                .Include(i => i.Carrinho)
                .Include(i => i.Jogo)
                .FirstOrDefaultAsync(i => i.ItemCarrinhoId == itemCarrinhoId);

            if (itemCarrinho == null)
                return NotFound("Item do carrinho não encontrado.");

            // Verifica se o cupom existe
            var cupom = await _context.Cupons.FirstOrDefaultAsync(c => c.CupomId == cupomId);
            if (cupom == null)
                return NotFound("Cupom não encontrado.");

            // Verifica se o cupom está ativo
            if (cupom.Ativo != true)
                return BadRequest("Cupom inativo.");

            // Verifica se o cupom está dentro da validade
            if (cupom.DataValidade.HasValue && cupom.DataValidade.Value < DateTime.UtcNow)
                return BadRequest("Cupom expirado.");

            // Calcula o desconto no item do carrinho
            decimal desconto = (itemCarrinho.ValorUnitario * cupom.Desconto) / 100;
            decimal valorComDesconto = itemCarrinho.ValorUnitario - desconto;

            // Atualiza o valor total do item no carrinho
            itemCarrinho.ValorTotal = valorComDesconto * itemCarrinho.Quantidade;
            _context.Entry(itemCarrinho).State = EntityState.Modified;

            // Registra a aplicação do cupom no CupomCarrinho
            var cupomCarrinho = new CupomCarrinho
            {
                CupomCarrinhoId = Guid.NewGuid(),
                CarrinhoId = itemCarrinho.CarrinhoId ?? Guid.Empty, // Corrigido para lidar com valores nulos
                CupomId = cupom.CupomId,
                DataAplicacao = DateTime.UtcNow
            };
            _context.CuponsCarrinhos.Add(cupomCarrinho);

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = "Cupom aplicado com sucesso!",
                ItemCarrinho = itemCarrinhoId,
                ValorOriginal = itemCarrinho.ValorUnitario * itemCarrinho.Quantidade,
                Desconto = desconto * itemCarrinho.Quantidade,
                ValorFinal = itemCarrinho.ValorTotal
            });
        }
    }
}
