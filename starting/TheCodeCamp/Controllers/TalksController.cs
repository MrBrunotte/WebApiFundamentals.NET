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

        // use the :int constraint to make sure only integers are allowed.
        [Route("{id:int}", Name = "GetTalk")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<TalkModel>(result));
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
        [Route()]
        public async Task<IHttpActionResult> Post(string moniker, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var camp = await _repository.GetCampAsync(moniker);
                    if (camp != null)
                    {
                        var talk = _mapper.Map<Talk>(model);
                        talk.Camp = camp;


                        // Map the speaker if necessary
                        if (model.Speaker != null)
                        {
                            var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                            if (speaker != null)
                            {
                                talk.Speaker = speaker;
                            }
                        }

                        _repository.AddTalk(talk);

                        if (await _repository.SaveChangesAsync())
                        {
                            return CreatedAtRoute("GetTalk",
                                new { moniker = moniker, id = talk.TalkId },
                                _mapper.Map<TalkModel>(talk));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId, true);
                    if (talk == null) return NotFound();

                    // it's going to ignore the Speaker
                    _mapper.Map(model, talk);

                    // Change speaker if needed
                    if (talk.Speaker.SpeakerId != model.Speaker.SpeakerId)
                    {
                        var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                        if (speaker != null) talk.Speaker = speaker;
                    }


                    if (await _repository.SaveChangesAsync())
                    {
                        return Ok(_mapper.Map<TalkModel>(talk));
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Delete(string moniker, int talkId)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId);
                if (talk == null) return NotFound();

                _repository.DeleteTalk(talk);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }

            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}
