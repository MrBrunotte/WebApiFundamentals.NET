using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Route()]
        public async Task<IHttpActionResult> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                // Get our results by awaiting on an asynchrounus call from the repository called GetTalksByMoniker
                // GetTalksByMoniker is going to get all of the talks for a specific camp.
                var result = await _repository.GetTalksByMonikerAsync(moniker, includeSpeakers);

                // map to an IEnumerable, or a collection of TalkModels
                return Ok(_mapper.Map<IEnumerable<TalkModel>>(result));
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}
