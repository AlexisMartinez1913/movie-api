using ApiPeliculas.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApiPeliculas.Controllers
{
    //controllerBase -> se usa para las api

    
    [ApiController]
    //rutas
    [Route("api/Categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        //añadir la cache
        //[ResponseCache(Duration = 20)]
        [ResponseCache(CacheProfileName = "Default20Seconds")]
        //respuestas
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //metodo para mostrar todas las categorias
        public IActionResult GetCategories()
        {
            var listCategories = _categoryRepository.GetCategories();
            var listCategoriesDto = new List<CategoryDto>();

            foreach (var category in listCategories)
            {
                listCategoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            return Ok(listCategoriesDto);
        }


        [AllowAnonymous]
        //añadir la cache - controlar las peticiones repititivas
        [ResponseCache(CacheProfileName = "Default20Seconds")]
        [HttpGet("{categoryId:int}", Name = "GetCategory")]
        //respuestas
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 

        //metodo para traer categoria individual por id
        public IActionResult GetCategory(int categoryId)
        {
            var itemCategory = _categoryRepository.GetCategory(categoryId);
            

            if (itemCategory == null) 
            {
                return NotFound();
            }
            var itemCategoryDto = _mapper.Map<CategoryDto>(itemCategory);
            return Ok(itemCategoryDto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        //respuestas
        [ProducesResponseType (201, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //metodo para crear
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            //validar campos requeridos
           if (!ModelState.IsValid) 
           {
                return BadRequest(ModelState);
           }
           if (createCategoryDto == null) 
           {
                return BadRequest(ModelState);
           }

           if (_categoryRepository.ExistsCategory(createCategoryDto.Name))
           {
                ModelState.AddModelError("", "The category already exist");
                return StatusCode(404, ModelState);
           }

            var category = _mapper.Map<Category>(createCategoryDto);
            if (!_categoryRepository.CreateCategory(category)) 
            {
                ModelState.AddModelError("", $"The error { category.Name}");
                return StatusCode(500, ModelState);
            }
            //se retorna la creacion del recurso
            return CreatedAtRoute("GetCategory", new {categoryId = category.Id}, category);
        }

        //put -> enviar todos los campos -  patch -> actualizacion parcial
        //metodo para editar
        [Authorize(Roles = "admin")]
        [HttpPatch("{categoryId:int}", Name = "UpdateCategory")]
        //respuestas
        [ProducesResponseType(201, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (categoryDto == null || categoryId != categoryDto.Id)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Error update the register {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [Authorize(Roles = "admin")]
        //metodo para eliminar
        [HttpDelete("{categoryId:int}", Name = "DeleteCategory")]
        //respuestas
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.ExistsCategory(categoryId))
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategory(categoryId);

            if (!_categoryRepository.DeleteCategory(category)) {
                ModelState.AddModelError("", $"Error deleting the register {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        


    }
}
