using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name.Equals(name));
            if (celestialObjects.Count() == 0)
            {
                return NotFound();
            }
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var objToUpdate = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (objToUpdate == null)
            {
                return NotFound();
            }

            objToUpdate.Name = celestialObject.Name;
            objToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            objToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(objToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var objToRename = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (objToRename == null)
            {
                return NotFound();
            }

            objToRename.Name = name;

            _context.Update(objToRename);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objsToDelete = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (objsToDelete.Count() == 0)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(objsToDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
