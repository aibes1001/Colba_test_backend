using Test_backend.Models;
using Test_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Test_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemesController : ControllerBase
    {
        private readonly MemesService _memesService;
        private JwtService _jwtService;

        public MemesController(MemesService booksService, JwtService jwtService)
        {
            _memesService = booksService;
            _jwtService = jwtService;
        }


        [HttpGet]
        public async Task<ActionResult<List<Meme>>> GetAll()
        {
            try
            {
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

                return await _memesService.GetAsync();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }


        //Search an id with 24 caracters!!
        [HttpGet("meme/{id:length(24)}")]
        public async Task<ActionResult<Uri>> GetById(string id)
        {
            try
            {
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

                var meme = await _memesService.GetIdAsync(id);

                if (meme is null)
                {
                    return NotFound();
                }

                meme.Count += 1;

                await _memesService.UpdateAsync(meme.Id, meme);

                return meme.Original;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

        }


        [HttpGet("name/{name}")]
        public async Task<ActionResult<Meme>> GetByName(string name)
        {
            try
            {
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

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
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

                if (token.result.UserRole != "premium") return StatusCode(StatusCodes.Status403Forbidden,
                    "The user has not permission to create a meme.");

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
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

                if (token.result.UserRole != "premium") return StatusCode(StatusCodes.Status403Forbidden,
                    "The user has not permission to modify a meme.");

                var meme = await _memesService.GetIdAsync(id);

                if (meme is null)
                {
                    return NotFound();
                }

                updatedMeme.Id = meme.Id;

                await _memesService.UpdateAsync(id, updatedMeme);

                return Ok("Meme has been updated");
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
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

                if (token.result.UserRole != "premium") return StatusCode(StatusCodes.Status403Forbidden,
                    "The user has not permission to delete a meme.");

                var meme = await _memesService.GetIdAsync(id);

                if (meme is null)
                {
                    return NotFound();
                }

                await _memesService.RemoveAsync(id);

                return Ok("Meme has been deleted");
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

        }


        //Implementation of a meme pagination
        [HttpGet("search")]
        public async Task<ActionResult<SearchByNamePaginationResponse>> SearchByNameAsync([FromQuery] SearchByNameDescriptionFilter filter)
        {
            try
            {
                var token = _jwtService.TokenValidation(this.HttpContext);

                if (!token.success) return StatusCode(StatusCodes.Status401Unauthorized,
                        token.msg);

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
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

        }


    }
}