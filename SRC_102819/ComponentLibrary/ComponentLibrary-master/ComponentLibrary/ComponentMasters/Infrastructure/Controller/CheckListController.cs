using System;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
    /// <summary>
    /// Represents a check list controller.
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("check-lists")]
    public class CheckListController : BaseController
    {
        private readonly ICheckListRepository _checkListRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListController"/> class.
        /// </summary>
        /// <param name="checkListRepository">The check list repository.</param>
        public CheckListController(ICheckListRepository checkListRepository)
        {
            _checkListRepository = checkListRepository;
        }

        /// <summary>
        /// Posts the specified check list.
        /// </summary>
        /// <param name="checkList">The check list.</param>
        /// <returns>Returns a checklist that was inserted.</returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CheckList checkList)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                return Created("", await _checkListRepository.Add(checkList));
            }
            catch (ArgumentException)
            {
                return Conflict();
            }
        }

        /// <summary>
        /// Gets the specified check list identifier.
        /// </summary>
        /// <param name="checkListId">The check list identifier.</param>
        /// <returns>Returns a checklist with given id.</returns>
        [Route("{checkListId}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string checkListId)
        {
            try
            {
                return Ok(await _checkListRepository.GetById(checkListId));
            }
            catch (ResourceNotFoundException e)
            {
                return NotFound();
            }
        }
    }
}