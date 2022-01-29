#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;
using AutoMapper;
using BookStoreAPI.DTOs.Author.BooksDTOs;
using BookStoreAPI.Static;
using BookStoreAPI.DTOs.Author;
using AutoMapper.QueryableExtensions;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly NET6MSIContext _context;
        private readonly IMapper mapper;
        //private readonly ILogger<AuthorsController> logger;


        public BooksController(NET6MSIContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
            // ILogger<AuthorsController> logger; this.logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetBooks()
        {
            
            try
            {
                var books = await _context.Books
                    .Include(q => q.Author)
                    .ProjectTo<BookReadOnlyDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
                //var bookDto = mapper.Map<IEnumerable<BookReadOnlyDto>>(books);
                return Ok(books);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, $"Error Perfoming GET in {nameof(GetBooks)}");
                return StatusCode(500, Messages.error500Message);
            }

        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadOnlyDto>> GetBook(int id)
        {
            try
            {
                //Include forbids to use FindAsync, that is why we use FirstOrDefaultAsync
                var book = await _context.Books
                    .Include(q => q.Author)
                    .ProjectTo<BookDetailsDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (book == null)
                {
                    return NotFound();
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, $"Record not found in GET {nameof(GetBook)} ID: {id}");
                return StatusCode(500, Messages.error500Message);
            }
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookUpdateDto)
        {
            if (id != bookUpdateDto.Id)
            {
                //logger.LogWarning($"Update ID invalid in {nameof(GetBook)} - ID: {id}");
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                //logger.LogWarning($"{nameof(Book)} record was not found in {nameof(PutBook)} - ID: {id}");
                return NotFound();
            }

            mapper.Map(bookUpdateDto, book);
            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await BookExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                   //logger.LogError(ex, $"Error Perfoming GET in {nameof(PutBook)}");
                    return StatusCode(500, Messages.error500Message);
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookCreateDto>> PostBook(BookCreateDto bookCreateDto)
        {
            try
            {
                var book = mapper.Map<Book>(bookCreateDto);
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBook", new { id = book.Id }, book);

            }
            catch (Exception ex)
            {
                //logger.LogError(ex, $"Error Perfoming Post in {nameof(PostBook)}", bookCreateDto);
                return StatusCode(500, Messages.error500Message);
            }
            

            
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    //logger.LogWarning($"{nameof(Book)} Record was not found in {nameof(DeleteBook)} ID: {id}");
                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent(); 
            }
            catch (Exception ex)
            {

                //logger.LogError(ex, $"Error Perfoming DELETE in {nameof(DeleteBook)}");
                return StatusCode(500, Messages.error500Message);
            }
            
        }

        private async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
    }
}
