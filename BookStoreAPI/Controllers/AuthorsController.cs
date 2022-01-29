#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;
using BookStoreAPI.DTOs.Author;
using AutoMapper;
using BookStoreAPI.Static;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly NET6MSIContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(NET6MSIContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            // in order to log info : logger.LogInformation($"Request to {nameof(GetAuthors)}");
            try
            {
                var authors = mapper.Map<IEnumerable<AuthorReadOnlyDto>>(await _context.Authors.ToListAsync());

                //Ok is code 200
                return Ok(authors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Perfoming GET in {nameof(GetAuthors)}");
                return StatusCode(500, Messages.error500Message);
                // throw kills the app 
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorReadOnlyDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    logger.LogWarning($"Record Not Found!: {nameof(GetAuthor)} - ID: {id}");
                    return NotFound();
                }

                var authorDto = mapper.Map<AuthorReadOnlyDto>(author);
                return Ok(author);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Perfoming GET in {nameof(GetAuthors)}");
                return StatusCode(500, Messages.error500Message);
                // throw kills the app 
            }

        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorDto)
        {
            //If author id exists ?
            if (id != authorDto.Id)
            {
                logger.LogWarning($"Update ID invalid in {nameof(GetAuthor)} - ID: {id}");
                //Bad Request is code 400
                return BadRequest();
            }

            //if author exists
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                logger.LogWarning($"{nameof(Author)} record was not found in {nameof(PutAuthor)} - ID: {id}");
                return NotFound();
            }

            mapper.Map(authorDto, author);
            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    logger.LogError(ex, $"Error Perfoming GET in {nameof(PutAuthor)}");
                    return StatusCode(500, Messages.error500Message);
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorCreateDto>> PostAuthor(AuthorCreateDto authorDto)
        {
            try
            {
                var author = mapper.Map<Author>(authorDto);
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Perfoming Post in {nameof(PostAuthor)}", authorDto);
                return StatusCode(500, Messages.error500Message);
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    logger.LogWarning($"{nameof(Author)} Record was not found in {nameof(DeleteAuthor)} ID: {id}");
                    return NotFound();
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Perfoming DELETE in {nameof(DeleteAuthor)}");
                return StatusCode(500, Messages.error500Message);
            }
        }

        //This was transformed to async method.
        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        }
    }
}



/* Down sides to auto gen data:
 * -Entity class could be use for taxi data, entity class should comunicate with database by actions, not endpoints.*/