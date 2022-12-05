using Test_backend.Models;
using Test_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Test_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemesController : ControllerBase
    {
        private readonly MemesService _memesService;
        public MemesController(MemesService booksService)
        {
            _memesService = booksService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Meme>>> GetAll()
        {
            try
            {
                return await _memesService.GetAsync();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            
        }

        //Search an id with 24 caracters!!
        [HttpGet("id/{id:length(24)}")]
        public async Task<ActionResult<Meme>> GetById(string id)
        {
            try
            {
                var meme = await _memesService.GetIdAsync(id);

                if (meme is null)
                {
                    return NotFound();
                }

                return meme;
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            
        }


        [HttpGet("name/{name}")]
        public async Task<ActionResult<Meme>> GetByName(string name)
        {
            try
            {
                var meme = await _memesService.GetNameAsync(name);

                if (meme is null)
                {
                    return NotFound();
                }

                return meme;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(Meme newMeme)
        {
            try
            {
                await _memesService.CreateAsync(newMeme);

                return CreatedAtAction(nameof(GetById), new { id = newMeme.Id }, newMeme);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Meme updatedMeme)
        {
            try
            {
                var meme = await _memesService.GetIdAsync(id);

                if (meme is null)
                {
                    return NotFound();
                }

                updatedMeme.Id = meme.Id;

                await _memesService.UpdateAsync(id, updatedMeme);

                return NoContent();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var meme = await _memesService.GetIdAsync(id);

                if (meme is null)
                {
                    return NotFound();
                }

                await _memesService.RemoveAsync(id);

                return NoContent();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

        }



        //Implementation a paginator
        [HttpGet("search")]
        public async Task<ActionResult<SearchByNamePaginationResponse>> SearchByNameAsync([FromQuery] SearchByNameDescriptionFilter filter)
        {
            try
            {
                var memesListed = new List<Meme>();
                var memesFiltered = await _memesService.PaginationFilteredByNameDescription(filter);

                for (var i = 0; i <= filter.ElementsInPageCount - 1; i++)
                {
                    var element = memesFiltered.ElementAtOrDefault(filter.ElementsInPageCount * (filter.Page - 1) + i);
                    if (element != null) memesListed.Add(element);
                }


                var total = (double)memesFiltered.Count / filter.ElementsInPageCount;
                var totalPages = (int)Math.Ceiling(total);

                return new SearchByNamePaginationResponse(filter.Page, totalPages, filter.ElementsInPageCount, memesListed);
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            
        }


    }
}