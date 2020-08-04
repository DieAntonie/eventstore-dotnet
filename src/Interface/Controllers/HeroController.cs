using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// using System.Linq;

namespace Interface.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeroController : ControllerBase
    {
        private List<Hero> Heroes = new List<Hero> {
            new Hero { id = 11, name = "Dr Nice" },
            new Hero { id= 12, name = "Narco" },
            new Hero { id= 13, name = "Bombasto" },
            new Hero { id= 14, name = "Celeritas" },
            new Hero { id= 15, name = "Magneta" },
            new Hero { id= 16, name = "RubberMan" },
            new Hero { id= 17, name = "Dynama" },
            new Hero { id= 18, name = "Dr IQ" },
            new Hero { id= 19, name = "Magma" },
            new Hero { id= 20, name = "Tornado" }
        };

        private readonly ILogger<HeroController> _logger;

        public HeroController(ILogger<HeroController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public Hero Get(int id)
        {
            for (int i = 0; i < Heroes.Count; i++)
            {
                if (Heroes[i].id == id)
                {
                    return Heroes[i];
                }
            }
            return null;
            // var rng = new Random();
            // return rng.Next(0, 2) == 1 ? Heroes.First(hero => hero.id.Equals(id)) : throw new Exception();
        }

        [HttpGet]
        public IEnumerable<Hero> Get(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return Heroes;
            return Heroes.FindAll(h => h.name.Contains(name));
        }

        [HttpPut("{id}")]
        public void Put(long id, Hero hero)
        {
            if (id != hero.id)
            {
                throw new Exception();
            }
            for (int i = 0; i < Heroes.Count; i++)
            {
                if (Heroes[i].id == id)
                {
                    Heroes[i].name = hero.name;
                    break;
                }
            }
        }

        [HttpPost]
        public void Post(Hero hero)
        {
            Heroes.Add(hero);
        }

        [HttpDelete]
        public void Delete(long id)
        {
            Heroes.Remove(Heroes.Find(h => h.id == id));
        }
    }
}
