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
        public async Task<List<Meme>> GetAll() =>
            await _memesService.GetAsync();

        //Search an id with 24 caracters!!
        [HttpGet("id/{id:length(24)}")]
        public async Task<ActionResult<Meme>> GetById(string id)
        {
            var meme = await _memesService.GetIdAsync(id);

            if (meme is null)
            {
                return NotFound();
            }

            return meme;
        }


        [HttpGet("name/{name}")]
        public async Task<ActionResult<Meme>> GetByName(string name)
        {
            var meme = await _memesService.GetNameAsync(name);

            if (meme is null)
            {
                return NotFound();
            }

            return meme;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Meme newMeme)
        {
            await _memesService.CreateAsync(newMeme);

            return CreatedAtAction(nameof(GetById), new { id = newMeme.Id }, newMeme);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Meme updatedMeme)
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

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var meme = await _memesService.GetIdAsync(id);

            if (meme is null)
            {
                return NotFound();
            }

            await _memesService.RemoveAsync(id);

            return NoContent();

        }



        //Implementation a paginator
        [HttpGet("search")]
        public async Task<SearchByNamePaginationResponse> SearchByNameAsync([FromQuery] SearchByNameDescriptionFilter filter)
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


    }
}