using APIDEMO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIDEMO.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly TestDBContext _context;
        public BooksController(TestDBContext context)
        {
            _context = context;
        }
        //get all books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Books>>> GetBooks(bool? stock, int? skip, int? take)
        {
            //  return await _context.Books.ToListAsync();
            var books = _context.Books.AsQueryable();
            if (stock != null)
            {
                books = _context.Books.Where(a => a.Quantity > 0);
            }
            if (skip != null)
            {
                books = books.Skip((int)skip);
            }
            if (take != null)
            {
                books = books.Take((int)take);
            }
            return await books.ToListAsync();
        }

        //get book by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Books>> GetBook(int id)
        {
            var book = _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return await book;
        }

        //edit book
        [HttpPut("{id}")]

        public async Task<ActionResult<Books>> EditBook(int id, Books book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }
            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BooksExist(id))
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

        private bool BooksExist(int id)
        {
            return _context.Books.Any(book => book.BookId == id);
        }

        //add book
        [HttpPost]
        public async Task<ActionResult<Books>> AddBook([FromBody] Books book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBooks", new { id = book.BookId }, book);
        }

        // delete 
        [HttpDelete("{id}")]
        public async Task<ActionResult<Books>> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return book;
        }

    }
}
