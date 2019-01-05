using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var celesticalObject = _context.CelestialObjects.Find(id);

            if (celesticalObject == null)
                return NotFound();
            celesticalObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            return Ok(celesticalObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celesticalObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (!celesticalObjects.Any())
                return NotFound();

            foreach (var celesticalObject in celesticalObjects)
            {
                celesticalObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celesticalObject.Id).ToList();
            }
            return Ok(celesticalObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celesticalObjects = _context.CelestialObjects.ToList();
            foreach (var celesticalObject in celesticalObjects)
            {
                celesticalObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celesticalObject.Id).ToList();
            }
            return Ok(celesticalObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.Find(id);
            if (existingObject == null)
                return NotFound();

            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(existingObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();

            celestialObject.Name = name;
            _context.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);
            if (!celestialObjects.Any())
                return NotFound();

            _context.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();

        }

    }
}
