using DevJBackend.Data;
using DevJBackend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevJBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly DevDbContext _dbcontext;

        public CourseController(DevDbContext devDbContext)
        {
            _dbcontext = devDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Getallcourses()
        {
            var crs_list = await _dbcontext.CrsList.ToListAsync();
            return Ok(crs_list);
        }


        [HttpGet("GetcrsI/{crsid}")]
        public IActionResult GetByCrsId(int crsid)
        {
            var courseinfo = _dbcontext.CrsInfo.Include(x => x.Modules).Where(t => t.CrsId == crsid).ToList();

            if (courseinfo == null || courseinfo.Count == 0)
            {
                return BadRequest("Course Not Found");
            }

            return Ok(courseinfo);
        }


        //ModuleDetail api code
        //[HttpGet("GetcrsTopicI/{crsTopicId}")]
        //public async Task<IActionResult> GetcrsTopicbyCourse(int crsTopicId)
        //{
        //    var crstopic = await _dbcontext.CrsInfo.Include(x => x.Modules).Where(x => x.CrsId == crsTopicId).ToListAsync();

        //    if(crstopic == null || crstopic.Count ==0 )
        //    {
        //        return NotFound("Course Topic Data Not Found");
        //    }

        //    return Ok(crstopic);
        //}


        [HttpPost("AddMOduleTopic")]
        public async Task<IActionResult> AddModuleTopic([FromBody] CrsTopicModel topicData)
        {
            if(topicData == null)
            {
                return BadRequest("Data Is Empty");
            }

            var existTopic= await _dbcontext.CrsInfo.FirstOrDefaultAsync(x => x.CrsId == topicData.CrsId);
            if (existTopic != null)
            {
                existTopic.Title = topicData.Title;
                existTopic.Description = topicData.Description;
                existTopic.crsPrice = topicData.crsPrice;
                existTopic.crsDurationInDays = topicData.crsDurationInDays;

                await _dbcontext.SaveChangesAsync();
                return Ok(new { message = "Topic updated successfully" });
            }else{
                _dbcontext.CrsInfo.Add(topicData);
                await _dbcontext.SaveChangesAsync();
                return Ok(new { message = "Topic added sucessfully" });
            }
        }


        [HttpPost("AddMOduleOnly/{crsId}")]
        public async Task<IActionResult> AddModuleOnly(int crsId,[FromBody] List<ModuleDetail> newmodules)
        {

            var existictpicId = await _dbcontext.CrsInfo.Include(x => x.Modules).FirstOrDefaultAsync(x => x.CrsId == crsId);

            if (existictpicId == null)
            {
                return NotFound("Course Data Not Found");
            }

            if(newmodules != null && newmodules.Any())
            {
                existictpicId.Modules.AddRange(newmodules);
            }

            foreach (var item in newmodules){
                bool isExist = await _dbcontext.CrsInfo.AnyAsync(x => x.Modules.Any(m => m.ModuleName == item.ModuleName && m.ModuleDescription == item.ModuleDescription));
                if (isExist)
                {
                    return BadRequest("Module with name and description already exists.");
                }

            }

            existictpicId.Modules.AddRange(newmodules);
            //_dbcontext.CrsInfo.AddRange(newmodules)
            await _dbcontext.SaveChangesAsync();
            return Ok(new { message = "Topic added sucessfully" });
        }

    }
}